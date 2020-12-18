using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// the Controllable Item that has a right-click menu.
    /// </summary>
    /// <typeparam name="T">Type to set or get in the ControllableComponent.</typeparam>
    public abstract class MenuControl<T>: ControlItem<T> 
    {
        protected Func<ToolStripDropDownMenu> CreateMenu;
        protected virtual bool UnableMenu => false;

        /// <summary>
        /// the Controllable Item that has a right-click menu.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="createMenu"> how to create menu. </param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public MenuControl(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable,
            string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, layout, enable, tips, tipsRelay, renderLittleZoom)
        {

            this.CreateMenu = createMenu;
        }

        /// <summary>
        /// change the right click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == MouseButtons.Right && CreateMenu != null)
            {
                var menu = CreateMenu();
                if (UnableMenu)
                {
                    foreach (var item in menu.Items)
                    {
                        if(item is ToolStripItem)
                        {
                            ToolStripItem toolStrip = item as ToolStripItem;
                            toolStrip.Enabled = false;
                        }

                    }
                }
                menu.Show(sender, e.ControlLocation);
            }
        }
    }
}
