using Build.Extensions.Controls;
using System.Windows.Forms;
namespace Build.Extensions.Tests.Forms
{
    partial class CheckedListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.filteredList = new Build.Extensions.Controls.CheckedListView();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(719, 44);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(186, 422);
            this.textBox2.TabIndex = 1;
            this.textBox2.Click += new System.EventHandler(this.textBox2_Click);
            // 
            // filteredList
            // 
            this.filteredList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filteredList.CheckBoxes = true;
            this.filteredList.CheckedItemsCsv = "*";
            this.filteredList.FullRowSelect = true;
            this.filteredList.Location = new System.Drawing.Point(12, 44);
            this.filteredList.Name = "filteredList";
            this.filteredList.Size = new System.Drawing.Size(234, 119);
            this.filteredList.TabIndex = 0;
            this.filteredList.UseCompatibleStateImageBehavior = false;
            this.filteredList.View = System.Windows.Forms.View.Details;
            // 
            // CheckedListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 584);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.filteredList);
            this.Name = "CheckedListForm";
            this.Load += new System.EventHandler(this.CheckedListForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public CheckedListView filteredList;
        private TextBox textBox2;

    }
}