/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;

namespace InfoGlasses.WinformMenu
{
    public class ShowcaseToolsMenu: ToolStripMenuItem
    {
        public ShowcaseToolsMenu():base(Properties.Resources.ShowcaseTools)
        {
            this.SetItemLangChange(new string[] { "ShowcaseTools", "展示工具" }, null);

            this.DropDown.Items.Add(new FixCategoryMenuItem());

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(new InfoGlassesMenuItem());

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.AddRange(LanguageSetting.ExtraItems);
        }
    }
}
