using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WinformControls
{
    /// <summary>
    /// Set the wire default colors.
    /// </summary>
    internal struct WireColorSet
    {
        public Color DefaultColor { get; }
        public Color SelectedColor { get; }
        public Color UnselectColor { get; }
        public Color EmptyColor { get; }

        public WireColorSet(Color defaultColor, Color selectedColor,  Color unselectColor,Color emptyColor )
        {
            this.DefaultColor = defaultColor;
            this.SelectedColor = selectedColor;
            this.UnselectColor = unselectColor;
            this.EmptyColor = emptyColor;

        }
    }
}
