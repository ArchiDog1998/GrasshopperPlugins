﻿/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class InputBoxDouble: InputBoxBase<double>
    {
        public double Min { get;}
        public double Max { get; }

        public InputBoxDouble(string valueName, ControllableComponent owner, Func<int, RectangleF, RectangleF, RectangleF> layoutWidth, bool enable,
            double @default, double min = double.MinValue, double max = double.MaxValue, string[] tips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null,
            bool renderLittleZoom = false)
            :base(valueName, owner, layoutWidth, enable, tips, tipsRelay, createMenu, renderLittleZoom)
        {
            this.Min = min;
            this.Default = @default;
            this.Max = max;
        }

        protected override string WholeToString()
        {
            bool isNull;
            double value = this.GetValue(out isNull);
            if(isNull)
            {
                return "Null";
            }
            else if(value.ToString().Split('.').Length == 1)
            {
                return value.ToString("F1");
            }
            else
            {
                return value.ToString();
            }
            
        }

        protected override double StringToT(string str)
        {
            double result;
            if (double.TryParse(str, out result))
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

        public override double GetValue(out bool isNull)
        {
            isNull = false;
            return Owner.GetValuePub(ValueName, Default);
        }

        public override void SetValue(double valueIn, bool record)
        {
            Owner.SetValuePub(ValueName, valueIn, record);
        }
    }
}
