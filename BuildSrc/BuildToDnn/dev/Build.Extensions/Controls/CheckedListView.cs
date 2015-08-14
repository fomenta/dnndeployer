using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Build.Extensions.Controls
{
    public class CheckedListView : ListView
    {
        #region Constants
        private const int HEADER_HEIGHT = 20;
        private const int ITEM_HEIGHT_OFFSET = 4;
        private const int CHECKBOX_PLUS_SCROLLBAR_WIDTH = 17;
        private const int VISIBLE_ITEMS = 25;
        #endregion

        #region Fields
        private ListViewColumnSorter lvwColumnSorter;
        //private bool isUpdatingCheckStates = false;
        #endregion

        #region Component UI
        private Container components = null;

        // This call is required by the Windows.Forms Form Designer.
        public CheckedListView()
        {
            InitializeComponent();

            lvwColumnSorter = new ListViewColumnSorter();
            this.ListViewItemSorter = lvwColumnSorter;

            this.CheckBoxes = true;
            this.FullRowSelect = true;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Details;
            this.Click += CheckedListView_Click;
            this.ColumnClick += CheckedListView_ColumnClick;
            this.KeyDown += CheckedListView_KeyDown;
        }

        void CheckedListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            { foreach (ListViewItem item in this.Items) { if (!item.Selected) { item.Selected = true; } } }
        }

        void CheckedListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.Sort();
        }

        void CheckedListView_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.SelectedItems)
            {
                if (Control.ModifierKeys == Keys.Shift)
                { if (!item.Checked) { item.Checked = true; } }
                else
                { item.Checked = !item.Checked; }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { if (components != null) { components.Dispose(); } }
            base.Dispose(disposing);
        }

        //Component Designer generated code
        private void InitializeComponent() { }
        #endregion

        #region Public Members
        // Gets the current bit value corresponding to all checked items
        public string CheckedItemsCsv
        {
            get
            {
                if (CheckedItems.Count == Items.Count) { return string.Empty; }
                //
                var checkedItemList = new List<string>();
                foreach (ListViewItem listItem in CheckedItems)
                {
                    var key = (string)listItem.Tag;
                    checkedItemList.Add(key);
                }
                return string.Join(",", checkedItemList.ToArray());
            }
            set
            {
                var selectedItemList = new List<string>(value == null ? new string[] { } : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                var lastCheckedIndex = -1;
                // Iterate over all items
                for (int i = 0; i < Items.Count; i++)
                {
                    ListViewItem listItem = Items[i];
                    var key = (string)listItem.Tag;
                    //listItem.Checked = selectedItemList.Any(o => key.IndexOf(o, StringComparison.OrdinalIgnoreCase) >= 0);
                    listItem.Checked = selectedItemList.Any(o => Regex.IsMatch(key, o, RegexOptions.IgnoreCase));

                    if (listItem.Checked)
                    {
                        Items.RemoveAt(listItem.Index);
                        Items.Insert(++lastCheckedIndex, listItem);
                    }
                }
            }
        }


        public void FillFromCsv(string[] headers, string csvList, string csvCheckedList, Func<int, string, ListViewItem> buildItem = null)
        {
            this.Clear(headers);
            this.AddFromCsv(csvList, buildItem);
            this.AutoResize();
            this.CheckedItemsCsv = csvCheckedList;
        }

        public void Clear(string[] headers = null)
        {
            // cleanup
            Items.Clear();
            Columns.Clear();

            if (headers == null || headers.Length == 0) { headers = new[] { "", "" }; }

            // setup columns
            for (int i = 0; i < headers.Length; i++) { Columns.Add(headers[i]); }
        }

        private void AddFromCsv(string csvList, Func<int, string, ListViewItem> buildItem)
        {
            List<string> allItems = new List<string>(csvList == null ? new string[] { } : csvList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            // trim leading/trailing whitespaces
            for (int i = 0; i < allItems.Count; i++)
            { var item = allItems[i]; if (!string.IsNullOrEmpty(item)) allItems[i] = item.Trim(); }

            // add items
            var index = 0;
            foreach (var item in allItems)
            {
                var listItem = buildItem == null ? new ListViewItem(item) : buildItem(index, item);
                Items.Add(listItem);
                index++;
            }
        }

        public void AutoResize()
        {
            // autoresize columns to new content
            if (this.Items.Count > 0)
            {

                this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                // resize height
                var totalColumnsWidth = 0;
                foreach (ColumnHeader col in this.Columns) { totalColumnsWidth += col.Width; }
                this.Width = totalColumnsWidth + CHECKBOX_PLUS_SCROLLBAR_WIDTH;
                this.Height = (this.FontHeight + ITEM_HEIGHT_OFFSET) * VISIBLE_ITEMS + HEADER_HEIGHT;
            }
        }
        #endregion
    }
}
