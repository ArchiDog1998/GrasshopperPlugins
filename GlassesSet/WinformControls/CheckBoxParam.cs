/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheckBox = ArchiTed_Grasshopper.WinformControls.CheckBox;

namespace InfoGlasses.WinformControls
{
    class CheckBoxParam<TGoo>: CheckBox, ITargetParam<TGoo, bool>, IDisposable where TGoo: GH_Goo<bool>
    {
        public GH_PersistentParam<TGoo> Target { get; }

        public GH_ParamAccess Access { get; set; }

        public RectangleF IconButtonLayout => AddObjectHelper.GetIconBound(this.Bounds);

        private Bitmap icon = new GH_BooleanToggle().Icon_24x24;
        public int Width => 20;

        public CheckBoxParam(GH_PersistentParam<TGoo> target, ControllableComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
            bool renderLittleZoom = false)
            : base(null, owner, null, enable, false, tips, tipsRelay, createMenu, isToggle, renderLittleZoom)
        {
            this.Target = target;
            try
            {
                this.Default = ((TGoo)target.PersistentData.AllData(true).ElementAt(0)).Value;
            }
            catch
            {
                this.Default = false;
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
                    this.RespondToMouseUp(Grasshopper.Instances.ActiveCanvas, new Grasshopper.GUI.GH_CanvasMouseEvent(vp, e));
                }
                else if (this.IconButtonLayout.Contains(mouseLoc))
                {
                    GH_BooleanToggle toggle = new GH_BooleanToggle();
                    toggle.Value = GetValue();
                    AddObjectHelper.CreateNewObject(toggle, this.Target, leftMove: 150);
                }
            }
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            float small = -2;
            RectangleF rect = CanvasRenderEngine.MaxSquare(ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect));
            rect.Inflate(small, small);
            this.Bounds = rect;
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
            return  base.IsRender(canvas, graphics, renderLittleZoom);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                ParamControlHelper.RenderParamButtonIcon(graphics, icon, IconButtonLayout);
            }
            base.Render(canvas, graphics, channel);
        }

        protected override bool GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = ParamControlHelper.GetData<TGoo, bool>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(bool valueIn, bool record = true)
        {
            if (record)
            {
                Target.RecordUndoEvent("Set the boolean");
            }
            ParamControlHelper.SetData<TGoo, bool>(this, valueIn);
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= ActiveCanvas_MouseClick;
        }
    }
}
