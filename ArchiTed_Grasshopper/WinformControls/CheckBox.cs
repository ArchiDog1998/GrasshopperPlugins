/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Check box in grasshopper canvas.
    /// </summary>
    public class CheckBox: ClickButtonBase<LangWindow>
    {
        /// <summary>
        /// Check box in grasshopper canvas.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="default"> default states. </param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="isToggle"> is click like a toggle. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public CheckBox(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
            bool @default, string[] tips = null, int tipsRelay =1000,Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
            bool renderLittleZoom = false)
            : base(valueName, owner, layout, enable, @default, tips, tipsRelay, createMenu, isToggle, renderLittleZoom, renderSet:null)
        {

        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if(channel == GH_CanvasChannel.Objects)
            {
                Color showColor = this.Enable ? ColorExtension.OffColor : ColorExtension.UnableColor;
                graphics.DrawEllipse(new Pen(showColor, this.Bounds.Height / 6), this.Bounds);

                Color buttonColor = this.GetValue() ? ColorExtension.OnColor : ColorExtension.OffColor;
                RectangleF rect = this.Bounds;
                float inflate = -this.Bounds.Height / 3;
                rect.Inflate(inflate, inflate);
                graphics.DrawEllipse(new Pen(buttonColor, this.Bounds.Height / 3), rect);
            }
        }
    }
}
