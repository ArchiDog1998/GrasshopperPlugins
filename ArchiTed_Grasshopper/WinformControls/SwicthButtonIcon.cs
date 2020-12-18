using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Button render in icon.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SwicthButtonIcon<T> : SwitchButtonBase<T>, IButtonIcon where T : LangWindow
    {
        /// <summary>
        /// Icon in right state.
        /// </summary>
        public Bitmap Icon => Icons[GetValue()].GetIcon(this.Enable, true);

        /// <summary>
        /// All icons.
        /// </summary>
        protected Iconable[] Icons { get;}

        /// <summary>
        /// Button render in icon.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="icons">Icons in show</param>
        /// <param name="default"> default states. </param>
        /// <param name="allTips"> tips in differenc states and different languages. </param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        /// <param name="renderSet"> render settings. </param>
        public SwicthButtonIcon(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
             Bitmap[] icons, int @default, string[][] allTips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
             bool renderLittleZoom = false, ButtonRenderSet? renderSet = null)
            : base(valueName, owner, layout, enable, @default, allTips, tipsRelay, createMenu, renderLittleZoom, renderSet)
        {
            if (icons.Length != allTips.Length)
                throw new ArgumentOutOfRangeException("icons, allTips", "length must be the same!");

            Iconable[] iconables = new Iconable[icons.Length];
            for (int i = 0; i < icons.Length; i++)
            {
                iconables[i] = new Iconable(icons[i]);
            }
            this.Icons = iconables;
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            WinformControlHelper.ButtonIconRender(graphics, channel, this);
        }
    }
}