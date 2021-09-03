/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Orthoptera.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public class Class1 : GH_AssemblyPriority
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
