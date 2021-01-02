/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using InfoGlasses.WPF;

namespace InfoGlasses.WinformControls
{
    class ParamLegend : RenderItem
    {
        private ParamGlassesComponent Owner;

        public ParamLegend(ParamGlassesComponent owner, Func<bool> showFunc = null)
            : base(null, showFunc, true)
        {
            this.Owner = owner;
        }

        protected override RectangleF Layout(GH_Canvas canvas, Graphics graphics, RectangleF rect)
        {
            Rectangle recta = canvas.Viewport.ScreenPort;
            return new RectangleF(recta.Location, recta.Size);
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false) => true;

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Overlay)
            {
                DrawLegend(graphics);
            }
        }

        private void DrawLegend(Graphics graphics)
        {
            if (Owner.ShowProxy.Count == 0)
                return;

            float zoom = Grasshopper.Instances.ActiveCanvas.Viewport.Zoom;
            float size = (float)Owner.LegendSize / zoom;
            float spacing = (float)Owner.LegendSpacing / zoom;
            float mult = 0.8f;

            float width = 0f;
            foreach (var info in Owner.ShowProxy)
            {
                float newWidth = graphics.MeasureString(info.TypeName, new Font(GH_FontServer.Standard.FontFamily, size * mult / 2)).Width;
                if (newWidth > width)
                {
                    width = newWidth;
                }

            }
            float height = Owner.ShowProxy.Count * size;
            width += size;
            float oneWayWidth = width;
            RectangleF rect = Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion;
            int heightMult = (int)(height / (rect.Height - 2 * spacing));
            if (heightMult > 0)
            {
                height = rect.Height - 2 * spacing;
                width += heightMult * (width + size);
            }

            PointF pivot;
            switch (Owner.LegendLocation)
            {
                case 0:
                    pivot = new PointF(rect.X + spacing, rect.Y + height + spacing);
                    break;
                case 1:
                    pivot = new PointF(rect.X + spacing, rect.Y + rect.Height - spacing);
                    break;
                case 2:
                    pivot = new PointF(rect.Right - width - spacing, rect.Y + rect.Height - spacing);
                    break;
                case 3:
                    pivot = new PointF(rect.Right - width - spacing, rect.Y + height + spacing);
                    break;
                default:
                    pivot = new PointF(rect.X, rect.Y + height);
                    break;
            }

            RectangleF background = new RectangleF(pivot.X, pivot.Y - height, width, height);
            background.Inflate(size / 4, size / 4);
            GraphicsPath path = CanvasRenderEngine.GetRoundRectangle_Obsolete(background, size / 4);
            graphics.FillPath(new SolidBrush(Owner.LegendBackGroundColor), path);
            graphics.DrawPath(new Pen(Owner.LegendBoundaryColor, size / 15), path);

            float actHeight = 0;
            float startY = pivot.Y;
            foreach (var info in Owner.ShowProxy)
            {
                DrawOneLegend(graphics, pivot, info, size, mult);
                actHeight += size;
                if (actHeight > rect.Height - 2 * spacing)
                {
                    actHeight = 0;
                    pivot = new PointF(pivot.X + oneWayWidth + size, startY);

                }
                else
                {
                    pivot = new PointF(pivot.X, pivot.Y - size);
                }
            }

        }

        private void DrawOneLegend(Graphics graphics, PointF pivot, GooTypeProxy info, float size, float mult)
        {

            float height = size * mult;
            float padding = size * (1 - mult) / 2;

            RectangleF fillRect = new RectangleF(pivot.X, pivot.Y - size, size, size);
            fillRect.Inflate(-padding, -padding);
            graphics.FillPath(new SolidBrush(info.ShowColor), CanvasRenderEngine.GetRoundRectangle_Obsolete(fillRect, padding));

            graphics.DrawString(info.TypeName, new Font(GH_FontServer.Standard.FontFamily, height / 2), new SolidBrush(Owner.LegendTextColor), new PointF(pivot.X + size + padding, pivot.Y - height - padding));
        }
    }
}
