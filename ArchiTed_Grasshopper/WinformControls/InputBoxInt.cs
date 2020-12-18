using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class InputBoxInt: InputBoxBase<int>
    {
        public int Min { get; }
        public int Max { get; }

        public InputBoxInt(string valueName, ControllableComponent owner, Func<int, RectangleF, RectangleF, RectangleF> layoutWidth, bool enable,
            int @default, int min = int.MinValue, int max = int.MaxValue, string[] tips = null, int tipsRelay = 1000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            : base(valueName, owner, layoutWidth, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Min = min;
            this.Default = @default;
            this.Max = max;
        }

        protected override string WholeToString(int value)
        {
            return value.ToString();
        }

        protected override int StringToT(string str)
        {
            int result;
            if (int.TryParse(str, out result))
            {
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
            }
            else
            {
                result = Default;
            }
            return result;
        }

        protected override int GetValue()
        {
            return Owner.GetValuePub(ValueName, Default);
        }

        protected override void SetValue(int valueIn)
        {
            Owner.SetValuePub(ValueName, valueIn);
        }
    }
}
