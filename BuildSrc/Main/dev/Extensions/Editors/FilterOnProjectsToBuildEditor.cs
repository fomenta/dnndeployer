using Build.Extensions.Controls;
using Microsoft.TeamFoundation.Build.Client;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;

namespace Build.Extensions.Editors
{
    public class FilterOnProjectsToBuildEditor : UITypeEditor
    {
        #region Properties
        private CheckedListView checkedList;
        #endregion

        #region Constructor
        public FilterOnProjectsToBuildEditor() { checkedList = new CheckedListView(); }
        #endregion

        #region Overriden
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                if (context == null || context.Instance == null || provider == null) { return value; }

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc == null) { return value; }

                // get projects to build
                string projectsToBuild = GetProjectsToBuild(provider);
                // display control
                return ShowUI(checkedList, projectsToBuild, (string)value, () => edSvc.DropDownControl(checkedList));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("[PE] ERROR: " + ex.ToString());
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        { return UITypeEditorEditStyle.DropDown; }
        #endregion

        #region Static
        public static object ShowUI(CheckedListView filteredList, string projectsToBuild, string selectedProjects, Action showDialog)
        {
            string[] headers = new[] { Resources.Column1_Number, Resources.Column2_Project, Resources.Column3_TfsParentPath };

            filteredList.FillFromCsv(headers, projectsToBuild, selectedProjects,
                                (index, item) =>
                                {
                                    int pos = item.LastIndexOf("/");
                                    string project = item.Substring(pos + 1);
                                    string parentTfsLocation = item.Substring(0, pos);
                                    var listItem = new ListViewItem(new[] { (index + 1).ToString(), project, parentTfsLocation });
                                    listItem.Tag = project.Replace(".sln", "");
                                    return listItem;
                                });
            showDialog();
            return filteredList.CheckedItemsCsv;
        }

        private static string GetProjectsToBuild(IServiceProvider provider)
        {
            const string PROPERTY = "ProjectsToBuild";
            if (provider == null) { throw new ArgumentNullException("provider is NULL."); }
            var buildDefinition = (IBuildDefinition)provider.GetService(typeof(IBuildDefinition));
            if (buildDefinition == null) { throw new ArgumentNullException("Cannot get buildDefinition."); }
            string parameters = buildDefinition.ProcessParameters;
            XElement paramXml = XElement.Parse(parameters);
            return paramXml.Elements().First().Attribute(PROPERTY).Value;

            // NOTE: this approach did not work, it returned a null object reference
            //var settings = (BuildSettings)provider.GetService(typeof(BuildSettings));
            //var projects = settings.ProjectsToBuild;
            //if (projects == null) { return string.Empty; }
            //return string.Join(",", settings.ProjectsToBuild.ToArray());
        }
        #endregion
    }
}
