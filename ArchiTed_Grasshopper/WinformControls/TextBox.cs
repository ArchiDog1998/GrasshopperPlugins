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
            if (cornerRadius <= 0)
                throw new ArgumentOutOfRangeException("cornerRadius", "cornerRadius must be larger than 0!");

            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
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
