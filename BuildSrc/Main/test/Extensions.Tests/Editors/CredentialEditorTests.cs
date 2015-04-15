using Build.Extensions.Editors;
using Build.Extensions.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace Build.Extensions.Tests.Editors
{
    [TestClass]
    public class CredentialEditorTests : BaseUnitTest
    {
        [TestMethod]
        public void CredentialDialogTest()
        {
            Credential credential = new Credential { UserName = "abc@domain.com", Password = "abc123$" };

            using (CredentialDialog dialog = new CredentialDialog())
            {
                dialog.UserName = credential.UserName;
                dialog.Password = credential.Password;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    credential.UserName = dialog.UserName;
                    credential.Password = dialog.Password;
                }
            }

            TestContext.WriteLine("UserName: '{0}'", credential.UserName);
            TestContext.WriteLine("Password: '{0}'", credential.Password);
        }
    }
}
