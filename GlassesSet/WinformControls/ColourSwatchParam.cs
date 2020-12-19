using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class ColourSwatchParam : ColourSwatch, ITargetParam<Color>
    {
        public GH_PersistentParam<GH_Goo<Color>> Target { get; }

        public int Width => 20;

        public GH_ParamAccess Access { get; set; }

        //public string Suffix => WinformControlHelper.GetSuffix(this.Access);

        public ColourSwatchParam(string valueName, ControllableComponent owner, GH_PersistentParam<GH_Goo<Color>> target, bool enable, Color @default,
            string[] tips = null, int tipsRelay = 1000,
            bool renderLittleZoom = false)
            : base(valueName, owner, null, enable, @default, tips, tipsRelay, renderLittleZoom)
        {
            this.Target = target;
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = CanvasRenderEngine.MaxSquare(WinformControlHelper.ParamLayoutBase(this.Target.Attributes, Width, innerRect, outerRect));
            this.Bounds.Inflate(-2, -2);
        }



        protected override Color GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = WinformControlHelper.GetData<Color>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(Color valueIn)
        {
            WinformControlHelper.SetData<Color>(this, valueIn);
        }




    }
}

