using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    class InputBoxDoubleParam: InputBoxDouble, ITargetParam<double>
    {

        public GH_PersistentParam<GH_Goo<double>> Target { get; }

        public GH_ParamAccess Access { get; set; }

        //public string Suffix => WinformControlHelper.GetSuffix(this.Access);

        public InputBoxDoubleParam(string valueName, ControllableComponent owner, GH_PersistentParam<GH_Goo<double>>  target, bool enable,
            double @default, double min = double.MinValue, double max = double.MaxValue, string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, null, enable, @default, min, max, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Target = target;
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = WinformControlHelper.ParamLayoutBase(this.Target.Attributes, Width, innerRect, outerRect);
            this.Bounds.Inflate(-2, -2);
        }

        protected override double GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = WinformControlHelper.GetData<double>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(double valueIn)
        {
            WinformControlHelper.SetData<double>(this, valueIn);
        }

    }
}
