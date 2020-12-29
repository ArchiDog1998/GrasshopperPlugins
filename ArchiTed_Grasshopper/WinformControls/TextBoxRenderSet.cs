/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArchiTed_Grasshopper.WinformControls
{
    public struct TextBoxRenderSet
    {
        public Color TextColor { get; }
        public Color BackGroundColor { get; }
        public Color BoundaryColor { get; }
        public Font Font { get; }
        public float BoundaryWidth { get;}
        public int ColorChange { get;}
        public float CornerRadius { get;}

        public InflateMode InflateMode { get;}

        public TextBoxRenderSet(Color backgroundColor, Color boundaryColor, Font font, Color textColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3, InflateMode mode = InflateMode.Horizontal)
        {
            this.BackGroundColor = backgroundColor;
            this.BoundaryColor = boundaryColor;
            this.Font = font;
            this.TextColor = textColor;
            this.BoundaryWidth = boundaryWidth;
            this.ColorChange = colorChange;
            this.CornerRadius = cornerRadius;
            this.InflateMode = mode;
        }
    }

    public enum InflateMode { Horizontal, Vertical, Both }
}