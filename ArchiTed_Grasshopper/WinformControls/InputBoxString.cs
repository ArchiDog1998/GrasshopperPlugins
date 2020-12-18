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
            string @default, string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, layoutName, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Default = @default;
        }

        protected override string GetValue()
        {
            return Owner.GetValuePub(ValueName, Default);
        }

        protected override void SetValue(string valueIn)
        {
            Owner.SetValuePub(ValueName, valueIn);
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
