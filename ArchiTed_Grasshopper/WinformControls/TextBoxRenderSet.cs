/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class TextBoxRenderSet
    {
        public virtual Color TextColor { get; } = Color.Black;
        public virtual Color BackGroundColor { get; } = Color.WhiteSmoke;
        public virtual Color BoundaryColor { get; } = Color.FromArgb(30, 30, 30);
        public virtual Font Font { get; } = GH_FontServer.StandardAdjusted;
        public virtual float BoundaryWidth { get; } = 1;
        public virtual int ColorChange { get; } = -40;
        public virtual float CornerRadius { get; } = 3;

        public virtual InflateMode InflateMode { get; } = InflateMode.Horizontal;

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

        public TextBoxRenderSet()
        {
        }
    }

    public enum InflateMode { Horizontal, Vertical, Both }
}