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
    public class ColourSwatchParam<TGoo> : ColourSwatch, ITargetParam<TGoo, Color> where TGoo : GH_Goo<Color>

    {
        public GH_PersistentParam<TGoo> Target { get; }
        GH_Param<TGoo> IParamControlBase<TGoo>.Target => this.Target;
        public GH_ParamAccess Access { get; set; }

        public string initStr => GetValue(out _).ToString();


        private AddProxyParams[] _myProxies;
        public AddProxyParams[] MyProxies
        {
            get
            {
                _myProxies = _myProxies ?? ParamControlHelper.GetAddProxyInputParams(this);
                return _myProxies;
            }
        }
        public new ParamGlassesComponent Owner { get; }
        public RectangleF IconButtonBound => ParamControlHelper.GetIconBound(this.Bounds, true);
        public int Width => 20;

        public ColourSwatchParam(GH_PersistentParam<TGoo> target, ParamGlassesComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, bool renderLittleZoom = false)
            : base(null, owner, null, enable, Color.Black, tips, tipsRelay, renderLittleZoom)
        {
            this.Target = target;
            this.Owner = owner;
        }

        public void RespondToMouseDown(object sender, MouseEventArgs e)
        {
            ParamControlHelper.ParamMouseDown(this, this.RespondToMouseUp, sender, e, true,init: initStr);
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
            return ParamControlHelper.IsRender(this, canvas, graphics, renderLittleZoom) && base.IsRender(canvas, graphics, renderLittleZoom);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            ParamControlHelper.IconRender(this, canvas, graphics, channel);
            base.Render(canvas, graphics, channel);
        }

        public override Color GetValue(out bool isNull)
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            Color result;
            if (ParamControlHelper.GetData(this, out access, out result))
            {
                this.Access = access;
                isNull = false;
                return result;
            }
            else
            {
                this.Access = access;
                isNull = true;
                return Color.Transparent;
            }

        }

        public override void SetValue(Color valueIn, bool record = true)
        {
            if (record)
            {
                Target.RecordUndoEvent("Set the Colour");
            }
            ParamControlHelper.SetData<TGoo, Color>(this, valueIn);
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= RespondToMouseDown;
        }
    }
}

