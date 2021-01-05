/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Some Controllable Item.
    /// </summary>
    /// <typeparam name="T">Type to set or get in the ControllableComponent.</typeparam>
    public abstract class ControlItem<T> : Renderable, IRespond, IControlState<T>
    {
        /// <summary>
        /// The name of the value in type T.
        /// </summary>
        public string ValueName { get; }

        /// <summary>
        /// This control item belong to.
        /// </summary>
        public ControllableComponent Owner { get; }

        /// <summary>
        /// Enable to use this.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// The function to find this item's bounds.
        /// </summary>
        protected Func<RectangleF, RectangleF, RectangleF> layoutFunc { get; set; }

        /// <summary>
        /// Tips to show when cursor move on and stop on this item.
        /// </summary>
        protected virtual string[] Tips { get; }

        /// <summary>
        /// The bounds that should respond.
        /// </summary>
        protected virtual RectangleF RespondBounds => this.Bounds;

        /// <summary>
        /// How long in millisecond to show the tips.
        /// </summary>
        protected int TipsRelay{get; }

        /// <summary>
        /// The default value.
        /// </summary>
        public T Default { get; set; }

        /// <summary>
        /// The constructor of some Controllable Item.
        /// </summary>
        /// <param name="valueName">The name of the value in type T.</param>
        /// <param name="owner">This control item belong to.</param>
        /// <param name="layout">The function to find this item's bounds.</param>
        /// <param name="enable">Enable to use this.</param>
        /// <param name="tips">Tips to show when cursor move on and stop on this item.</param>
        /// <param name="tipsRelay">How long in millisecond to show the tips.</param>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public ControlItem(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout,  bool enable,
            string[] tips = null, int tipsRelay = 5000,
            bool renderLittleZoom = false)
            :base(renderLittleZoom)
        {
            this.layoutFunc = layout;
            this.Owner = owner;
            this.ValueName = valueName;
            this.Enable = enable;
            this.Tips = tips;
            this.TipsRelay = tipsRelay;
        }

        /// <summary>
        /// To set the bounds.
        /// </summary>
        /// <param name="innerRect">Inside rectangle.</param>
        /// <param name="outerRect">Outside rectangle.</param>
        public virtual void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = layoutFunc(innerRect, outerRect);
        }

        #region ToolTip
        private Stopwatch stopwatch = new Stopwatch();
        private Point pivot;
        private bool showed = false;
        ToolTip tip = new ToolTip() { ShowAlways = true};

        /// <summary>
        /// Set the ToolTip when mouse move.
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        protected void SetToolTip(GH_Canvas sender, GH_CanvasMouseEvent e)
        {

            if (Tips == null)
            {
                return;
            }
            if (pivot == e.ControlLocation)
            {
                if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds > this.TipsRelay)
                {
                    if (!showed)
                    {
                        Point location = new Point(e.ControlLocation.X + 30, e.ControlLocation.Y - 30);
                        tip.Show(LanguagableComponent.GetTransLation(Tips), sender, location, this.TipsRelay);
                        showed = true;
                    }

                }
                else
                {
                    stopwatch.Start();
                }
            }
            else
            {
                showed = false;
                tip.Hide(sender);
                pivot = e.ControlLocation;
            }
        }


        #endregion

        #region Respond
        /// <summary>
        /// Check whether respond.
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        /// <returns></returns>
        protected bool IsRespond(GH_Canvas canvas, GH_CanvasMouseEvent e)
        {
            return this.RespondBounds.Contains(e.CanvasLocation) && canvas.Viewport.Zoom >= 0.5f;
        }

        #region PublicRespond
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        /// <returns>Is respond.</returns>
        public bool RespondToMouseDownPub(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if(IsRespond(sender, e) && this.Enable)
            {
                RespondToMouseDown(sender, e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        /// <returns>Is respond.</returns>
        public bool RespondToMouseMovePub(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (IsRespond(sender, e))
            {
                RespondToMouseMove(sender, e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        /// <returns>Is respond.</returns>
        public bool RespondToMouseUpPub(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (IsRespond(sender, e) && this.Enable)
            {
                RespondToMouseUp(sender, e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        /// <returns>Is respond.</returns>
        public bool RespondToMouseDoubleClickPub(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (IsRespond(sender, e) && this.Enable)
            {
                RespondToMouseDoubleClick(sender, e);
                return true;
            }
            return false;
        }
        #endregion

        #region PrivateRespond

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        protected virtual void RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        protected virtual void RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            SetToolTip(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        protected virtual void RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Canvas from grasshopper canvas.</param>
        /// <param name="e">CanvasMouseEvent</param>
        protected virtual void RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
        }

        #endregion

        #endregion

        /// <summary>
        /// Get value that in controllableComponent.
        /// </summary>
        /// <returns>Value.</returns>-
        public abstract T GetValue();

        /// <summary>
        /// Set value that in controllableComponent.
        /// </summary>
        /// <param name="valueIn">Value.</param>
        public abstract void SetValue(T valueIn, bool record = true);

    }
}
