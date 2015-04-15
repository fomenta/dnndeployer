using Build.Extensions.Security;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Build.Extensions.Editors
{
    public class CredentialEditor : UITypeEditor
    {

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (editorService != null)
                {
                    Credential credential = value as Credential;

                    using (CredentialDialog dialog = new CredentialDialog())
                    {
                        dialog.UserName = credential.UserName;
                        dialog.Password = credential.Password;

                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            credential = new Credential { UserName = dialog.UserName, Password = dialog.Password };
                        }
                    }

                    /* when I change the Parameter setting the values that I set when queueing a new build are ignored 
                     * and the settings in the Build Definition Editor are kept when running the build. Either the 
                     * queuing new build settings are ignored, or they're overridden by the Edit Build Definition 
                     * settings. You need to return a new Credential object as opposed to "return value". 
                     * This tells TFS that the dialog has been updated, and also alerts the Build Definition Edit 
                     * screen to the fact that something has changed. */
                    return credential;
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }

}
