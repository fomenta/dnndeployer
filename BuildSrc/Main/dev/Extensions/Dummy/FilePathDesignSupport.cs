using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Build.Extensions.Editors
{
    public partial class FilePathDesignSupport : Form
    {
        public FilePathDesignSupport()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
        }

        private string _fileName = string.Empty;
        public string FileName
        {
            get
            {
                return _fileName;

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblFileSelected.Text = string.Empty;
            this._fileName = string.Empty;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(this.openFileDialog1.FileName) == false)
            {
                this.lblFileSelected.Text = this.openFileDialog1.FileName;
                this._fileName = this.lblFileSelected.Text;

            }
            else
            {
                this.lblFileSelected.Text = string.Empty;
                this._fileName = string.Empty;
            }
        }
    }
}
