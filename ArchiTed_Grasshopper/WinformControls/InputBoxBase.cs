/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Define a input box.
    /// </summary>
    /// <typeparam name="T">Type to set or get in the ControllableComponent.</typeparam>
    public abstract class InputBoxBase<T> : MenuControl<T>, INeedWidth
    {
        /// <summary>
        /// The function that define bounds with width. 
        /// </summary>
        public Func<int, RectangleF, RectangleF, RectangleF> LayoutWidth { get; }

        /// <summary>
        /// how to find the string's Width.
        /// </summary>
        public virtual int Width => GH_FontServer.StringWidth(WholeToString(GetValue()), GH_FontServer.StandardAdjusted);

        /// <summary>
        ///  Define a input box.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public InputBoxBase(string valueName, ControllableComponent owner, Func<int, RectangleF, RectangleF, RectangleF> layoutWidth, bool enable,
            string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, null, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.LayoutWidth = layoutWidth;
        }

        /// <summary>
        /// Change the valut to string which can be seen in the inputbox.
        /// </summary>
        /// <param name="value"> Value. </param>
        /// <returns> String that should be shown. </returns>
        protected abstract string WholeToString(T value);

        /// <summary>
        /// Get Value from input string.
        /// </summary>
        /// <param name="str"> input string. </param>
        /// <returns> Value. </returns>
        protected abstract T StringToT(string str);

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = LayoutWidth(this.Width, innerRect, outerRect);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Brush brush = new SolidBrush(this.Enable ? ColorExtension.OffColor : ColorExtension.UnableColor);
                graphics.DrawString(WholeToString(GetValue()), GH_FontServer.StandardAdjusted, brush, this.Bounds, format);
            }

        }

        protected override void RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                new InputBoxBalloon(this.Bounds, Write).ShowTextInputBox(sender, WholeToString(GetValue()), selectContent: true, limitToBoundary: true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl));
                void Write(string str)
                {
                    SetValue(StringToT(str));
                }
            }
        }

    }
}
