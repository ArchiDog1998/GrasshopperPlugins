/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Set the text box like balloon.
    /// </summary>
    public class TextBox : RenderItem
    {
        /// <summary>
        /// the string that should be shown.
        /// </summary>
        public virtual string ShowName { get; }

        /// <summary>
        /// render settings.
        /// </summary>
        public TextBoxRenderSet RenderSet { get; }

        /// <summary>
        /// How to define the bounds.
        /// </summary>
        private Func<SizeF, RectangleF, RectangleF> Func { get; }

        /// <summary>
        /// get the string's bounds
        /// </summary>
        private Func<Graphics, string, Font, SizeF> MeansureString { get; }

        /// <summary>
        /// Set the text box like balloon.
        /// </summary>
        /// <param name="name"> the string that should be shown. </param>
        /// <param name="target"> the GH_DocumentObject that relay on.  </param>
        /// <param name="layout"> How to define the bounds. size for this label's size, rectangle for target's bounds. </param>
        /// <param name="renderSet"> render settings. </param>
        /// <param name="meansureString"> get the string's bounds. </param>
        /// <param name="showFunc"> whether to show. </param>
        /// <param name="renderLittleZoom"> whether render when zoom is less than 0.5. </param>
        public TextBox(string name, IGH_DocumentObject target, Func<SizeF, RectangleF, RectangleF> layout,
            TextBoxRenderSet renderSet, Func<Graphics, string, Font, SizeF> meansureString = null, Func<bool> showFunc = null, bool renderLittleZoom = false)
            : base(target, showFunc, renderLittleZoom)
        {
            this.ShowName = name;
            this.RenderSet = renderSet;
            this.Func = layout;

            this.MeansureString = meansureString ?? ((x, y, z) => { return x.MeasureString(y, z); });

        }

        protected override RectangleF Layout(GH_Canvas canvas, Graphics graphics, RectangleF rect)
        {
            return this.Func(this.MeansureString(graphics, this.ShowName, this.RenderSet.Font), this.Target.Attributes.Bounds);
        }

        /// <summary>
        /// make the rectangle had corners.
        /// </summary>
        /// <param name="rect"> rectangle </param>
        /// <param name="cornerRadius"> cornerRadius </param>
        /// <returns> round Rectangle. </returns>
        public static GraphicsPath GetRoundRectangle(RectangleF rect, float cornerRadius)
        {
            float _startMovementMult = 1f / 3f;
            float _controlMovementMult = 2f / 3f;

            if (cornerRadius <= 0)
                throw new ArgumentOutOfRangeException("cornerRadius", "cornerRadius must be larger than 0!");
            else if (Math.Min(rect.Width, rect.Height) < cornerRadius * (1 + _startMovementMult) * 2)
                throw new ArgumentOutOfRangeException("cornerRadius", "cornerRadius is too large!");

            float _startMovement = cornerRadius * (1 + _startMovementMult);
            float _controlMovement = cornerRadius * (1 - _controlMovementMult);

            PointF _cornerA = new PointF(rect.X, rect.Y);
            PointF _cornerB = new PointF(rect.X + rect.Width, rect.Y);
            PointF _cornerC = new PointF(rect.X + rect.Width, rect.Y + rect.Height);
            PointF _cornerD = new PointF(rect.X, rect.Y + rect.Height);

            PointF _cornerA1 = new PointF(_cornerA.X, _cornerA.Y + _startMovement);
            PointF _cornerA2 = new PointF(_cornerA.X, _cornerA.Y + _controlMovement);
            PointF _cornerA3 = new PointF(_cornerA.X + _controlMovement, _cornerA.Y);
            PointF _cornerA4 = new PointF(_cornerA.X + _startMovement, _cornerA.Y);

            PointF _cornerB1 = new PointF(_cornerB.X - _startMovement, _cornerB.Y);
            PointF _cornerB2 = new PointF(_cornerB.X - _controlMovement, _cornerB.Y);
            PointF _cornerB3 = new PointF(_cornerB.X, _cornerB.Y + _controlMovement);
            PointF _cornerB4 = new PointF(_cornerB.X, _cornerB.Y + _startMovement);

            PointF _cornerC1 = new PointF(_cornerC.X, _cornerC.Y - _startMovement);
            PointF _cornerC2 = new PointF(_cornerC.X, _cornerC.Y - _controlMovement);
            PointF _cornerC3 = new PointF(_cornerC.X - _controlMovement, _cornerC.Y);
            PointF _cornerC4 = new PointF(_cornerC.X - _startMovement, _cornerC.Y);

            PointF _cornerD1 = new PointF(_cornerD.X + _startMovement, _cornerD.Y);
            PointF _cornerD2 = new PointF(_cornerD.X + _controlMovement, _cornerD.Y);
            PointF _cornerD3 = new PointF(_cornerD.X, _cornerD.Y - _controlMovement);
            PointF _cornerD4 = new PointF(_cornerD.X, _cornerD.Y - _startMovement);

            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddBezier(_cornerA1, _cornerA2, _cornerA3, _cornerA4);
            roundedRect.AddBezier(_cornerB1, _cornerB2, _cornerB3, _cornerB4);
            roundedRect.AddBezier(_cornerC1, _cornerC2, _cornerC3, _cornerC4);
            roundedRect.AddBezier(_cornerD1, _cornerD2, _cornerD3, _cornerD4);

            roundedRect.CloseFigure();
            return roundedRect;
        }

        protected static void DrawRectangleBox(Graphics graphics, RectangleF rect, Color backgroundColor, Color boundaryColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3, InflateMode mode = InflateMode.Horizontal)
        {
            if (boundaryWidth <= 0)
                throw new ArgumentOutOfRangeException("boundaryWidth", "boundaryWidth must be larger than 0!");

            switch (mode)
            {
                case InflateMode.Horizontal:
                    rect.Inflate(cornerRadius, 0);
                    break;
                case InflateMode.Vertical:
                    rect.Inflate(0, cornerRadius);
                    break;
                case InflateMode.Both:
                    rect.Inflate(cornerRadius, cornerRadius);
                    break;
                default:
                    rect.Inflate(cornerRadius, 0);
                    break;
            }

            LinearGradientBrush brush = new LinearGradientBrush(rect, backgroundColor, backgroundColor.LightenColor(colorChange), LinearGradientMode.Vertical);
            if (cornerRadius == 0)
            {
                graphics.FillRectangle(brush, rect);
                graphics.DrawRectangle(new Pen(boundaryColor, boundaryWidth), rect.X, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                GraphicsPath path = GetRoundRectangle(rect, cornerRadius);
                graphics.FillPath(brush, path);
                graphics.DrawPath(new Pen(boundaryColor, boundaryWidth), path);
            }


        }


        public static void DrawTextBox(Graphics graphics, RectangleF rect, Color backgroundColor, Color boundaryColor, string text, Font font, Color textColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3, InflateMode mode = InflateMode.Horizontal, bool isCenter = false)
        {
            DrawRectangleBox(graphics, rect, backgroundColor, boundaryColor, boundaryWidth, colorChange, cornerRadius, mode);
            StringFormat format = new StringFormat();
            if (isCenter) format.Alignment = StringAlignment.Center;
            graphics.DrawString(text, font, new SolidBrush(textColor), rect, format);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
                DrawTextBox(graphics, this.Bounds,
                    this.RenderSet.BackGroundColor, this.RenderSet.BoundaryColor, this.ShowName, this.RenderSet.Font,
                    this.RenderSet.TextColor, this.RenderSet.BoundaryWidth, this.RenderSet.ColorChange, this.RenderSet.CornerRadius, this.RenderSet.InflateMode);
        }
    }


}
