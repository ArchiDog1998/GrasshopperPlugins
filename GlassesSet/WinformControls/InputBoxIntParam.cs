/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

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
    class InputBoxIntParam: InputBoxInt, ITargetParam<int>
    {
        public GH_PersistentParam<GH_Goo<int>> Target { get; }

        public GH_ParamAccess Access { get; set; }

        //public string Suffix => WinformControlHelper.GetSuffix(this.Access);

        public InputBoxIntParam(string valueName, ControllableComponent owner, GH_PersistentParam<GH_Goo<int>> target, bool enable,
            int @default, int min = int.MinValue, int max = int.MaxValue, string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
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

        protected override int GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = WinformControlHelper.GetData<int>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(int valueIn)
        {
            WinformControlHelper.SetData<int>(this, valueIn);
        }
    }
}
