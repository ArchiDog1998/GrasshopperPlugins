/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class InputBoxString : InputBoxBase<string>
    {

        public InputBoxString(string valueName, ControllableComponent owner, Func<int, RectangleF, RectangleF, RectangleF> layoutName, bool enable,
            string @default, string[] tips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, layoutName, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Default = @default;
        }

        public override string GetValue()
        {
            return Owner.GetValuePub(ValueName, Default);
        }

        public override void SetValue(string valueIn, bool record)
        {
            Owner.SetValuePub(ValueName, valueIn, record);
        }

        protected override string StringToT(string str)
        {
            return str;
        }

        protected override string WholeToString(string value)
        {
            return value;
        }
    }


}
