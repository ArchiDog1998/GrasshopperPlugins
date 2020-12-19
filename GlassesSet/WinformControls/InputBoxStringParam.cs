﻿using Grasshopper.Kernel;
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
    class InputBoxStringParam : InputBoxString, ITargetParam<string>
    {
        public GH_PersistentParam<GH_Goo<string>> Target { get; }

        public GH_ParamAccess Access { get; set; }

        //public string Suffix => WinformControlHelper.GetSuffix(this.Access);

        public InputBoxStringParam(string valueName, ControllableComponent owner, GH_PersistentParam<GH_Goo<string>> target, bool enable,
            string @default, string[] tips = null, int tipsRelay =- 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, null, enable,@default, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Target = target;
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = WinformControlHelper.ParamLayoutBase(this.Target.Attributes, Width, innerRect, outerRect);
            this.Bounds.Inflate(-2, -2);
        }

        protected override string GetValue()
        {
            GH_ParamAccess access = GH_ParamAccess.item;
            var result = WinformControlHelper.GetData<string>(this, out access);
            this.Access = access;
            return result;
        }

        protected override void SetValue(string valueIn)
        {
            WinformControlHelper.SetData<string>(this, valueIn);
        }

    }
}