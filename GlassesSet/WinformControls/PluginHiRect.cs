/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InfoGlasses.WinformMenu.InfoGlassesMenuItem;

namespace InfoGlasses.WinformControls
{
    public class PluginHiRect : HighLightRect
    {
        public override Color Color => (Color)Settings.GetProperty(InfoGlassesProps.PluginColor);
        public override int Radius => (int)Settings.GetProperty(InfoGlassesProps.PluginRadius);

        public PluginHiRect(IGH_DocumentObject target)
            :base(target, Color.Black, 0)
        {

        }
    }
}
