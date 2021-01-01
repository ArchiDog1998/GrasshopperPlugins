/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Button showed in icons.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClickButtonIcon<T> : ClickButtonName<T>, IButtonIcon where T : LangWindow
    {
        /// <summary>
        /// icons in different button state.
        /// </summary>
        protected Iconable IconSet { get; }

        /// <summary>
        /// Get icon in right state.
        /// </summary>
        public Bitmap Icon => IconSet.GetIcon(this.Enable, Owner.GetValuePub(ValueName, Default));

        /// <summary>
        /// Button showed in icons.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="icon"> icon in show. </param>
        /// <param name="default"> default states. </param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="isToggle"> is click like a toggle. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        /// <param name="renderSet"> render settings. </param>
        public ClickButtonIcon(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
            Bitmap icon, bool @default, string[] tips = null, int tipsRelay = 5000,Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
             bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            :base(valueName, owner, layout, enable, null, @default, tips, tipsRelay, createMenu, isToggle, renderLittleZoom, renderSet)
        {
            this.IconSet = new Iconable(icon);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            WinformControlHelper.ButtonIconRender(graphics, channel, this);
        }
    }
}