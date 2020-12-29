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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// the controllable Item that have a right-click menu and a double-click wpf window.
    /// </summary>
    /// <typeparam name="T">Type to set or get in the ControllableComponent.</typeparam>
    /// /// <typeparam name="Twpf"> wpf Type. </typeparam>
    public abstract class MenuWPFControl<T, Twpf> : MenuControl<T> where Twpf : LangWindow
    {
        /// <summary>
        /// the controllable Item that have a right-click menu and a double-click wpf window.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public MenuWPFControl(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
            string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, layout, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
        }

        /// <summary>
        /// override the doubleclick to show off the wpf window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinformControlHelper.CreateWindow<Twpf>(this.Owner);
            }
        }
    }
}
