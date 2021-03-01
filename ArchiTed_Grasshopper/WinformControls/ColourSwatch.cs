/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper.WinformControls
{
    public class ColourSwatch:ControlItem<Color>
    {

        public ColourSwatch(string valueName, ControllableComponent owner, Func<RectangleF, RectangleF, RectangleF> layout, bool enable, Color @default,
            string[] tips = null, int tipsRelay = 5000,
            bool renderLittleZoom = false)
            : base(valueName, owner, layout, enable, tips, tipsRelay, renderLittleZoom)
        {
            this.Default = @default;
        }

        public override Color GetValue(out bool isNull)
        {
            isNull = false;
            Color color;
            try
            {
                color = Owner.GetValuePub(ValueName, Default);
            }
            catch
            {
                color = Default;
            }
            return color;
        }

        public override void SetValue(Color valueIn, bool record = true)
        {
            Owner.SetValuePub(ValueName, valueIn, record);
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if(channel == GH_CanvasChannel.Objects)
            {
                //float width = this.Bounds.Height / 10;
                float width = 1;

                GraphicsPath path = TedTextBox.GetRoundRectangle(this.Bounds, this.Bounds.Height / 6);

                bool isNull;
                Color brushColor = GetValue(out isNull);
                brushColor = isNull ? Color.Transparent : brushColor;
                graphics.FillPath(new SolidBrush(brushColor), path);
                graphics.DrawPath(new Pen(this.Enable ? Color.DimGray : ColorExtension.UnableColor, width), path);
            }
            
        }

        protected override void RespondToMouseUp(GH_Canvas canvas, GH_CanvasMouseEvent @event)
        {

            bool flag = true;
            ToolStripDropDownMenu obj = new ToolStripDropDownMenu
            {
                ShowCheckMargin = false,
                ShowImageMargin = false
            };

            bool isNull;
            Color color = GetValue(out isNull);
            color = isNull ? Color.Transparent : color;
            GH_DocumentObject.Menu_AppendColourPicker(obj, color, ColourChanged);
            obj.Show(canvas, @event.ControlLocation);

            void ColourChanged(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
            {
                SetValue(e.Colour, flag);
                flag = false;

            }
        }


    }
}
