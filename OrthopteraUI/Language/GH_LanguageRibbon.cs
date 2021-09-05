/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper;
using Grasshopper.GUI.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Grasshopper.Kernel;

namespace OrthopteraUI.Language
{
    public class GH_LanguageRibbon
    {

        private void GH_RibbonControl_MouseDown(object sender, MouseEventArgs e)
		{

            GH_Ribbon ribbon = (GH_Ribbon)Instances.DocumentEditor.Controls[3];
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ToolStripDropDownMenu menu = (ToolStripDropDownMenu)item.Owner;
            Point pivot = menu.Bounds.Location;

            GH_RibbonItem ribbonItem = null;
            foreach (GH_RibbonTab tab in ribbon.Tabs)
            {
                foreach (GH_RibbonPanel panel in tab.Panels)
                {
                    foreach (GH_RibbonItem allItem in panel.AllItems)
                    {
                        if (allItem.Proxy.Exposure == GH_Exposure.hidden)
                        {
                            continue;
                        }
                        if (allItem.Visible)
                        {
                            Rectangle iconbox = ribbon.RectangleToScreen(allItem.ContentRegion);
                            if (iconbox.Contains(pivot))
                            {
                                ribbonItem = allItem;
                                break;
                            }
                        }
                    }
                }
            }


            if (ribbonItem == null)
            {
                MessageBox.Show("Failed to find proxy.");
            }
            MessageBox.Show(ribbonItem.Proxy.GetType().Name);
        }
    }
}
