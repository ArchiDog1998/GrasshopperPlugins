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
    /// button that click can be swithed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SwitchButtonBase<T> : MenuWPFControl<int, T>, IButton where T : LangWindow
    {
        /// <summary>
        /// Tips in different states and in different languages.
        /// </summary>
        protected string[][] AllTips { get;}

        /// <summary>
        /// Tips in right states.
        /// </summary>
        protected override string[] Tips => AllTips[GetValue()];

        /// <summary>
        /// Render settings.
        /// </summary>
        public ButtonRenderSet RenderSet { get ; }

        /// <summary>
        /// button that click can be swithed.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="default">default state.</param>
        /// <param name="allTips">Tips in different states and in different languages.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        /// <param name="renderSet">render settings. </param>
        public SwitchButtonBase(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
             int @default, string[][] allTips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
             bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            : base(valueName, owner, layout, enable, null, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Default = @default;
            this.AllTips = allTips;
            this.RenderSet = renderSet ?? new ButtonRenderSet(null, GH_FontServer.StandardAdjusted, 6, 0, GH_Palette.Hidden);
        }

        protected override void RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SetValue((GetValue() + 1) % AllTips.Length);
                Owner.ExpireSolution(true);
            }
        }

        protected override int GetValue()
        {
            return Owner.GetValuePub(ValueName, Default);
        }
        protected override void SetValue(int valueIn)
        {
            Owner.SetValuePub(ValueName, valueIn);
        }
    }
}
