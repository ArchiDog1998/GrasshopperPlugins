/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InfoGlasses.WinformMenu.InfoGlassesMenuItem;

namespace InfoGlasses.WinformControls
{
    public class NameBoxRenderSet: TextBoxRenderSet
    {
        public override Font Font => (Font)Settings.GetProperty(InfoGlassesProps.ShowFont);

        public override Color TextColor => (Color)Settings.GetProperty(InfoGlassesProps.TextColor);

        public override Color BackGroundColor => (Color)Settings.GetProperty(InfoGlassesProps.BackgroundColor);

        public override Color BoundaryColor => (Color)Settings.GetProperty(InfoGlassesProps.BoundaryColor);

        public override float BoundaryWidth => (float)(double)Settings.GetProperty(InfoGlassesProps.BoundaryWidth);
        public override int ColorChange => -40;
        public override float CornerRadius => (float)(double)Settings.GetProperty(InfoGlassesProps.BoundaryRadius);

        public override InflateMode InflateMode => InflateMode.Horizontal;

        public NameBoxRenderSet()
        {
        }
    }
}
