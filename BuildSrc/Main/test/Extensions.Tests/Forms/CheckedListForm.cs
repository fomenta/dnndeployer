using System;
using System.Windows.Forms;

namespace Build.Extensions.Tests.Forms
{
    public partial class CheckedListForm : Form
    {
        public CheckedListForm()
        {
            InitializeComponent();
        }

        private void CheckedListForm_Load(object sender, EventArgs e)
        {
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Text =
                string.Format("[{0}]\r\n{1}",
                    filteredList.CheckedItems.Count,
                    filteredList.CheckedItemsCsv);
        }

    }
}
