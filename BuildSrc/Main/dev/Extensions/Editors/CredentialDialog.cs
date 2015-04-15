using Build.Extensions.Security;
using System;
using System.Windows.Forms;

namespace Build.Extensions.Editors
{
    public partial class CredentialDialog : Form
    {
        private const string NON_EMPTY_PASSWORD_MASK = "********";
        private string realPassword;

        public CredentialDialog()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                if (PasswordMaskedTextbox.Text != NON_EMPTY_PASSWORD_MASK) { realPassword = PasswordMaskedTextbox.Text; }
                return realPassword;
            }
            set
            {
                realPassword = value;
                PasswordMaskedTextbox.Text = string.IsNullOrEmpty(value) ? value : NON_EMPTY_PASSWORD_MASK;
            }
        }

        public string UserName
        {
            get
            {
                return UserNameTextbox.Text;
            }
            set
            {
                UserNameTextbox.Text = value;
            }
        }

        private void TestButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                Credential credential = new Credential() { UserName = UserName, Password = Password };
                using (Impersonation impersonation = new Impersonation(credential)) { }

                MessageBox.Show(this, "Credentials are correct", "Test succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Test failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
