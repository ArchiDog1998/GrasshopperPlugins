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

namespace InfoGlasses.WinformControls
{
    public class ColourSwatchParam<TGoo> : ColourSwatch, ITargetParam<TGoo, Color>, IDisposable where TGoo: GH_Goo<Color>

    {
        public GH_PersistentParam<TGoo> Target { get; }

        public int Width => 20;

        public GH_ParamAccess Access { get; set; }

        //public string Suffix => WinformControlHelper.GetSuffix(this.Access);

        public ColourSwatchParam(GH_PersistentParam<TGoo> target, ControllableComponent owner, bool enable, 
            string[] tips = null, int tipsRelay = 5000, bool renderLittleZoom = false)
            : base(null, owner, null, enable, Color.Black, tips, tipsRelay, renderLittleZoom)
        {
            this.Target = target;
            try
            {
                this.Default = ((TGoo)target.PersistentData.AllData(true).ElementAt(0)).Value;
            }
            catch
            {
                this.Default = Color.Black;
                SetValue(this.Default);
            }
        }

        private void ActiveCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            GH_Viewport vp = Grasshopper.Instances.ActiveCanvas.Viewport;
            if (vp.Zoom >= 0.5f && this.Bounds.Contains(vp.UnprojectPoint(e.Location)))
            {
                this.RespondToMouseUp(Grasshopper.Instances.ActiveCanvas, new Grasshopper.GUI.GH_CanvasMouseEvent(vp, e));
            }
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = CanvasRenderEngine.MaxSquare(ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect));
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            Grasshopper.Instances.ActiveCanvas.MouseClick -= ActiveCanvas_MouseClick;
            if (Target.SourceCount > 0)
            {
                return false;
            }
            else
            {
                Grasshopper.Instances.ActiveCanvas.MouseClick += ActiveCanvas_MouseClick;
            }
            Layout(new RectangleF(), Target.Attributes.Bounds);
            return base.IsRender(canvas, graphics, renderLittleZoom);
        }



        protected override Color GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = ParamControlHelper.GetData<TGoo, Color>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(Color valueIn, bool record = true)
        {
            if (record)
            {
                Target.RecordUndoEvent("Set the Colour");
            }
            ParamControlHelper.SetData<TGoo, Color>(this, valueIn);
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseClick -= ActiveCanvas_MouseClick;
        }
    }
}

