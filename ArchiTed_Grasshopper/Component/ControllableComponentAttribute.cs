/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArchiTed_Grasshopper
{
    public class ControllableComponentAttribute : GH_ComponentAttributes
    {
        public new ControllableComponent Owner { get;set; }

        public ControllableComponentAttribute(ControllableComponent owner) : base(owner)
        {
            Owner = owner;
        }

        #region Layout
        public static RectangleF LayoutBoundsControl(ControllableComponent owner, RectangleF bounds)
        {
            bounds.Inflate(-2f, -2f);
            foreach (Renderable_Old control in owner.Controls)
            {
                bounds = RectangleF.Union(bounds, control.Bounds);
            }
            bounds.Inflate(2f, 2f);
            return bounds;
        }

        protected override void Layout()
        {
            Pivot = GH_Convert.ToPoint(Pivot);
            m_innerBounds = LayoutComponentBox(base.Owner);

            LayoutInputParams(base.Owner, m_innerBounds);
            LayoutOutputParams(base.Owner, m_innerBounds);
            Owner.ChangeParamsLayout();

            Bounds = LayoutBounds(this.Owner, m_innerBounds);
            
            foreach (IRespond control in Owner.Controls)
            {
                control.Layout(m_innerBounds, this.Bounds);
            }
            Bounds = LayoutBoundsControl(this.Owner, this.Bounds);
        }

        #endregion

        protected sealed override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            Owner.BeforeRender(canvas, graphics, channel);

            foreach (IRenderable renderable in Owner.RenderObjsUnderComponent)
            {
                renderable.RenderToCanvas(canvas, graphics, channel);
            }

            base.Render(canvas, graphics, channel);

            foreach (IRespond control in Owner.Controls)
            {
                control.RenderToCanvas(canvas, graphics, channel);
            }
            foreach (IRenderable renderObj in Owner.RenderObjs)
            {
                renderObj.RenderToCanvas(canvas, graphics, channel);
            }

            Owner.AfterRender(canvas, graphics, channel);
        }



        #region Respond
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            foreach (IRespond control in Owner.Controls)
            {
                if (control.RespondToMouseDownPub(sender, e))
                    return GH_ObjectResponse.Handled;
            }

            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            foreach (IRespond control in Owner.Controls)
            {
                if (control.RespondToMouseMovePub(sender, e))
                    return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseMove(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            foreach (IRespond control in Owner.Controls)
            {
                if (control.RespondToMouseUpPub(sender, e))
                    return GH_ObjectResponse.Handled;
            }
            return base.RespondToMouseUp(sender, e);
        }
        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            foreach (IRespond control in Owner.Controls)
            {
                if (control.RespondToMouseDoubleClickPub(sender, e))
                    return GH_ObjectResponse.Release;
            }
            if(m_innerBounds.Contains(e.CanvasLocation) && sender.Viewport.Zoom >= 0.5f && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Owner.CreateWindow();
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        #endregion


    }
}