/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

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
