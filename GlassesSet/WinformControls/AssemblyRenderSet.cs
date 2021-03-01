/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InfoGlasses.WinformMenu.InfoGlassesMenuItem;

namespace InfoGlasses.WinformControls
{
    public class AssemblyRenderSet : NameBoxRenderSet
    {
        public override Font Font => new Font( base.Font.FontFamily, (float)(double)Settings.GetProperty(InfoGlassesProps.AssemblyFontSize));

        public override Color BackGroundColor => Color.FromArgb(base.BackGroundColor.A / 2, base.BackGroundColor);
    }
}
