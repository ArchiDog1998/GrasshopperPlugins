/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
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
    /// Define a button that shows in string name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SwitchButtonString<T> : SwitchButtonBase<T>, IButtonString where T : LangWindow
    {
        /// <summary>
        /// Get name in right state and right languages.
        /// </summary>
        public string Name => LanguagableComponent.GetTransLation(Names[GetValue()]);

        /// <summary>
        /// Get color in right state and right languages.
        /// </summary>
        public Color RightColor => WinformControlHelper.GetRightColor(this.RenderSet.Colors, true, this.Enable);

        /// <summary>
        /// all Names in different states and different languages.
        /// </summary>
        public string[][] Names { get;}

        /// <summary>
        /// Define a button that shows in string name.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="names">showNames in different states and different languages.</param>
        /// <param name="default"> default states. </param>
        /// <param name="allTips">Tips in different states and in different languages.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        /// <param name="renderSet">render settings. </param>
        public SwitchButtonString(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
             string[][] names, int @default, string[][] allTips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null,
             bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            : base(valueName, owner, layout, enable, @default, allTips, tipsRelay, createMenu, renderLittleZoom, renderSet)
        {
            if (names.Length != allTips.Length)
                throw new ArgumentOutOfRangeException("names, allTips", "length must be the same!");
            this.Names = names;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            WinformControlHelper.ButtonStringRender(graphics, channel, this);
        }
    }
}
