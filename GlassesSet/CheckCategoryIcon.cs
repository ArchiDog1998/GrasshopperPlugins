/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using Grasshopper.GUI;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiTed_Grasshopper;

namespace InfoGlasses
{
    public class CheckCategoryIcon : GH_AssemblyPriority
    {
        public bool IsCheckCategoryIcon
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(IsCheckCategoryIcon), true);
            set
            {
                Grasshopper.Instances.Settings.SetValue(nameof(IsCheckCategoryIcon), value);
            }
        }

        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.CanvasCreated += Instances_CanvasCreated;

            return GH_LoadingInstruction.Proceed;
        }

        private void Instances_CanvasCreated(Grasshopper.GUI.Canvas.GH_Canvas canvas)
        {
            Grasshopper.Instances.CanvasCreated -= Instances_CanvasCreated;

            ToolStripMenuItem displayMenu = FindMenu("Display");
            ToolStripMenuItem item = WinFormPlus.CreateOneItem("Fix Catogory Icon", "Test", Grasshopper.Instances.ComponentServer.GetCategoryIcon("Params"));
            
            item.Checked = this.IsCheckCategoryIcon;
            item.Click += Item_Click;
            void Item_Click(object sender, EventArgs e)
            {
                this.IsCheckCategoryIcon = !this.IsCheckCategoryIcon;
                item.Checked = this.IsCheckCategoryIcon;
            }

            item.CheckedChanged += Item_CheckedChanged;
            void Item_CheckedChanged(object sender, EventArgs e)
            {
                switch (item.Checked)
                {
                    case true:
                        UseIconCheck();
                        break;
                    case false:
                        UnuserIconCheck();
                        break;
                }
            }

            displayMenu.DropDownItems.Insert(3, item);
        }

        private void UseIconCheck()
        {

        }

        private void UnuserIconCheck()
        {

        }

        public static ToolStripMenuItem FindMenu(string name)
        {
            GH_DocumentEditor editor = Grasshopper.Instances.DocumentEditor;
            if (editor == null) return null;

            ToolStripItem[] menus = editor.MainMenuStrip.Items.Find("mnu" + name, false);

            if (menus.Length == 0) return null;
            return (ToolStripMenuItem)menus[0];
        }
    }
}
