using Build.Extensions.Editors;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Build.Extensions.Dummy
{
    public class DriveEditor : UITypeEditor
    {
        // We will use a window for property editing. 
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        { return UITypeEditorEditStyle.Modal; }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null) { return value; }

            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null) { return value; }

            using (FilePathDesignSupport frm = new FilePathDesignSupport())
            {

                // TODO
                //frm.FileName = (string)value;

                // Return the the file name of the file selected. 
                if (editorService.ShowDialog(frm) == DialogResult.OK)
                { value = frm.FileName; }
            }
            return value;
        }

        // No special thumbnail will be shown for the grid. 
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        { return false; }

    }
}
