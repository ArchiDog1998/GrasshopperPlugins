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
            string[] tips = null, int tipsRelay = 1000,
            bool renderLittleZoom = false)
            : base(valueName, owner, layout, enable, tips, tipsRelay, renderLittleZoom)
        {
            this.Default = @default;
        }

        protected override Color GetValue()
        {
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

        protected override void SetValue(Color valueIn)
        {
            Owner.SetValuePub(ValueName, valueIn);
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if(channel == GH_CanvasChannel.Objects)
            {
                GraphicsPath path = TextBox.GetRoundRectangle(this.Bounds, this.Bounds.Height / 6);
                
                graphics.FillPath(new SolidBrush(GetValue()), path);
                //graphics.DrawPath(new Pen(this.Enable ? Color.Black : ColorControl.UnableColor, this.Bounds.Height/10), path);
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
                GH_DocumentObject.Menu_AppendColourPicker(obj, GetValue(), ColourChanged);
                obj.Show(canvas, @event.ControlLocation);

                void ColourChanged(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    if (flag)
                    {
                    SetValue(e.Colour);
                    }
                    flag = false;
                    
                }
        }


    }
}
