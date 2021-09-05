using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace OrthopteraUI
{
    public class OrthopteraUIInfo : GH_AssemblyInfo
    {
        public override string Name => "OrthopteraUI";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("B530B126-CECD-4EA4-9BEE-E0BAA6B32737");

        //Return a string identifying you or your company.
        public override string AuthorName => "秋水";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "1123993881@qq.com";
    }

    public class OrthopteraAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            //MessageBox.Show("Hello");
            Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;
            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(GH_Canvas canvas)
        {
            Grasshopper.Instances.CanvasCreated -= Instances_CanvasCreated;

            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null)
            {
                Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;

            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null)
            {
                MessageBox.Show("Orthoptera can't find the menu!");
                return;
            }
            DoingSomethingFirst(editor);
        }

        private void DoingSomethingFirst(GH_DocumentEditor editor)
        {
            SetLanguage();
            SetMenu(editor);
        }

        private void SetLanguage()
        {
            CultureInfo rightLang = System.Threading.Thread.CurrentThread.CurrentUICulture;
            foreach (var lang in GH_DescriptionTable.Languages)
            {
                if (lang.Name == rightLang.Name)
                {
                    GH_DescriptionTable.Language = rightLang;
                    break;
                }
            }
        }

        private void SetMenu(GH_DocumentEditor editor)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Orthoptera");
            ToolStripMenuItem langItems = new ToolStripMenuItem("Language");

            foreach (var lang in GH_DescriptionTable.Languages)
            {
                ToolStripMenuItem langItem = new ToolStripMenuItem(lang.NativeName);
                langItem.Click += (sender, e) =>
                {
                    GH_DescriptionTable.Language = lang;
                };
                langItems.DropDownItems.Add(langItem);
            }

            menu.DropDownItems.Add(langItems);
            editor.MainMenuStrip.Items.Add(menu);
        }
    }
}