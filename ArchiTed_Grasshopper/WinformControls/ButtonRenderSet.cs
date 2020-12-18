using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArchiTed_Grasshopper.WinformControls
{
    public struct ButtonRenderSet
    {
        public Color?[] Colors { get; }
        public Font Font { get; }
        public int CornerRadius { get; }
        public int HighLight { get; }

        public GH_Palette normalPalette { get; }

        public ButtonRenderSet(Color?[] colors, Font font, int cornerRadius = 6, int highLight = 0, GH_Palette normalPalette = GH_Palette.Hidden)
        {
            this.Colors = colors;
            this.Font = font;
            this.CornerRadius = cornerRadius;
            this.HighLight = highLight;
            this.normalPalette = normalPalette;
        }
    }
}