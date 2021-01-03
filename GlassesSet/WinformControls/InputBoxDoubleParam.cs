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
        GH_Param<TGoo> IParamControlBase<TGoo>.Target => this.Target;
        public GH_ParamAccess Access { get; set; }

        private AddProxyParams[] _myProxies;
        public AddProxyParams[] MyProxies
        {
            get
            {
                if (_myProxies == null)
                {
                    foreach (var set in Owner.CreateProxyDict)
                    {
                        if (set.Key == this.Target.Type.FullName)
                        {
                            _myProxies = set.Value;
                            return _myProxies;
                        }
                    }
                    _myProxies = new AddProxyParams[] { };
                }
                return _myProxies;
            }
        }
        public new ParamGlassesComponent Owner { get; }
        public RectangleF IconButtonBound => ParamControlHelper.GetIconBound(this.Bounds);

        public InputBoxDoubleParam(GH_PersistentParam<TGoo> target, ParamGlassesComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, bool renderLittleZoom = false)
            : base(null, owner, null, enable, 0, double.MinValue, double.MaxValue, tips, tipsRelay, null, renderLittleZoom)
        {
            this.Target = target;
            this.Owner = owner;
            ParamControlHelper.SetDefaultValue(this, 0);
        }

        public void RespondToMouseDown(object sender, MouseEventArgs e)
        {
            ParamControlHelper.ParamMouseDown(this, this.RespondToMouseDoubleClick, sender, e, init: GetValue().ToString());
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = ParamControlHelper.UpDownSmallRect(ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect, inflate: false));
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

        public override double GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = ParamControlHelper.GetData<TGoo, double>(this, out access);
            this.Access = access;
            return result;
        }

        public override void SetValue(double valueIn, bool record = true)
        {
            if (record)
            {
                Target.RecordUndoEvent("Set the Number");
            }
            ParamControlHelper.SetData<TGoo, double>(this, valueIn);
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= RespondToMouseDown;
        }

    }
}
