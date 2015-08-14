using Build.Extensions.Editors;
using Build.Extensions.Tests.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace Build.Extensions.Tests
{
    [TestClass]
    public class FilterOnProjectsToBuildTests : BaseUnitTest
    {
        [TestMethod]
        public void FilteredCheckedList()
        {
            var f = new CheckedListForm();

            //object value = "AnulacionDocumentos,Complementos".ToLower();
            //object value = "Permisos";
            object value = @"\."; // regex (all names with a dot (.)
            DialogResult dr = DialogResult.None;
            var newValue = FilterOnProjectsToBuildEditor.ShowUI(f.filteredList, SampleData.AllProjectsToBuild, (string)value,
                                                            () => { dr = f.ShowDialog(); });
            TestContext.WriteLine("newValue: {0}", newValue);
        }
    }
}
