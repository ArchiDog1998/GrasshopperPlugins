/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper;
using Grasshopper.Kernel.Special;

namespace InfoGlasses.WinformControls
{
    class InputBoxDoubleParam<TGoo> : InputBoxDouble, ITargetParam<TGoo, double>, IDisposable where TGoo : GH_Goo<double>
    {

        public GH_PersistentParam<TGoo> Target { get; }

        public GH_ParamAccess Access { get; set; }

        public RectangleF IconButtonLayout => AddObjectHelper.GetIconBound(this.Bounds);


        private Bitmap icon = new GH_NumberSlider().Icon_24x24;


        public InputBoxDoubleParam(GH_PersistentParam<TGoo> target, ControllableComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, bool renderLittleZoom = false)
            : base(null, owner, null, enable, 0, double.MinValue, double.MaxValue, tips, tipsRelay, null, renderLittleZoom)
        {
            this.Target = target;
            try
            {
                this.Default = ((TGoo)target.PersistentData.AllData(true).ElementAt(0)).Value;
            }
            catch
            {
                this.Default = 0;
                SetValue(this.Default);
            }
        }

        private void ActiveCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            GH_Viewport vp = Grasshopper.Instances.ActiveCanvas.Viewport;
            if (vp.Zoom >= 0.5f)
            {
                PointF mouseLoc = vp.UnprojectPoint(e.Location);
                if (this.Bounds.Contains(mouseLoc))
                {
                    this.RespondToMouseDoubleClick(Grasshopper.Instances.ActiveCanvas, new Grasshopper.GUI.GH_CanvasMouseEvent(vp, e));
                }
                else if (this.IconButtonLayout.Contains(mouseLoc))
                {
                    AddObjectHelper.CreateNewObject(new GH_NumberSlider(), this.Target, leftMove: 150, init: WholeToString( GetValue()));
                }
            }
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = ParamControlHelper.UpDownSmallRect( ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect, inflate: false));
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= ActiveCanvas_MouseClick;
            if (Target.SourceCount > 0)
            {
                return false;
            }
            else
            {
                Grasshopper.Instances.ActiveCanvas.MouseDown += ActiveCanvas_MouseClick;
            }
            Layout(new RectangleF(), Target.Attributes.Bounds);
            return base.IsRender(canvas, graphics, renderLittleZoom);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                ParamControlHelper.RenderParamButtonIcon(graphics, icon, IconButtonLayout);
            }
            base.Render(canvas, graphics, channel);
        }

        protected override double GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = ParamControlHelper.GetData<TGoo, double>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(double valueIn, bool record = true)
        {
            if (record)
            {
                Target.RecordUndoEvent("Set the Number");
            }
            ParamControlHelper.SetData<TGoo, double>(this, valueIn);
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= ActiveCanvas_MouseClick;
        }

    }
}
