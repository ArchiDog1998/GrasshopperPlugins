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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Define a ClickButton whose value is boolean.
    /// </summary>
    /// <typeparam name="T">wpf Type.</typeparam>
    public abstract class ClickButtonBase<T>: MenuWPFControl<bool, T>, IButton where T :LangWindow
    {

        /// <summary>
        /// Define is click like a toggle.
        /// </summary>
        private bool _isToggle;

        /// <summary>
        /// Redner settings.
        /// </summary>
        public ButtonRenderSet RenderSet { get; }

        /// <summary>
        /// Define a ClickButton whose value is boolean.
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
        /// <param name="renderSet"> render settings. </param>
        public ClickButtonBase(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
             bool @default, string[] tips = null, int tipsRelay = 1000,Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
             bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            :base(valueName, owner, layout, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {

            this._isToggle = isToggle;
            this.Default = @default;
            this.RenderSet = renderSet ?? new ButtonRenderSet(null, GH_FontServer.StandardAdjusted, 6, 0, GH_Palette.Hidden);
        }

        #region Respond
        /// <summary>
        /// Define the menu when toggle is off.
        /// </summary>
        protected override bool UnableMenu => !GetValue();

        protected override void RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_isToggle)
                {
                    SetValue(!GetValue());
                }
                else
                {
                    SetValue(false);
                }
                Owner.ExpireSolution(true);
            }

            base.RespondToMouseUp(sender, e);
        }

        protected override void RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (!_isToggle && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SetValue(true);
                Owner.ExpireSolution(true);
            }
        }

        protected override void RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (GetValue())
            {
                base.RespondToMouseDoubleClick(sender, e);
            }
        }


        #endregion

        protected override bool GetValue()
        {
            return Owner.GetValuePub(ValueName, Default);
        }

        protected override void SetValue(bool valueIn)
        {
            Owner.SetValuePub(ValueName, valueIn);
        }


    }
}
