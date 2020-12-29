/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Define a button that shows in string name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClickButtonName<T> : ClickButtonBase<T>, IButtonString where T : LangWindow
    {
        /// <summary>
        /// ShowName in different languages.
        /// </summary>
        private string[] _showName;

        /// <summary>
        /// Get the string's color.
        /// </summary>
        public Color RightColor
        {
            get
            {
                bool on = Owner.GetValuePub(ValueName, Default);
                return WinformControlHelper.GetRightColor(this.RenderSet.Colors, on, this.Enable);
            }
        }

        /// <summary>
        /// Button's show name.
        /// </summary>
        public string Name  => LanguagableComponent.GetTransLation(_showName);

        /// <summary>
        /// Define a button that shows in string name.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="name"> ShowName in different languages. </param>
        /// <param name="default"> default states. </param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="isToggle"> is click like a toggle. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        /// <param name="renderSet"> render settings. </param>
        public ClickButtonName(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
            string[]name, bool @default, string[] tips = null, int tipsRelay =1000, Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
            bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            : base(valueName, owner, layout, enable, @default, tips , tipsRelay, createMenu,isToggle, renderLittleZoom, renderSet)
        {
            this._showName = name;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            WinformControlHelper.ButtonStringRender(graphics, channel, this);
        }
    }
}