using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media.Imaging;

namespace ArchiTed_Grasshopper
{
    public static class CanvasRenderEngine
    {
        public static GraphicsPath GetRoundRectangle_Obsolete(RectangleF rect, float cornerRadius)
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

        public static void DrawRectangleBox_Obsolete(Graphics graphics, RectangleF rect, Color backgroundColor, Color boundaryColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3, InflateMode mode = InflateMode.Horizontal)
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
                GraphicsPath path = GetRoundRectangle_Obsolete(rect, cornerRadius);
                graphics.FillPath(brush, path);
                graphics.DrawPath(new Pen(boundaryColor, boundaryWidth), path);
            }
            
            
        }

        public static RectangleF DrawTextBox_Obsolete(Graphics graphics, PointF middleDownPivot, Color backgroundColor, Color boundaryColor, string text, Font font, Color textColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3)
        {
            SizeF size = graphics.MeasureString(text, font);
            RectangleF rect = MiddleDownRect(middleDownPivot, size);
            DrawRectangleBox_Obsolete(graphics, rect, backgroundColor, boundaryColor, boundaryWidth, colorChange, cornerRadius);
            graphics.DrawString(text, font, new SolidBrush(textColor), rect);
            return rect;
        }

        public static void DrawTextBox_Obsolete(Graphics graphics, RectangleF rect, Color backgroundColor, Color boundaryColor, string text, Font font, Color textColor, float boundaryWidth = 1, int colorChange = -40, float cornerRadius = 3)
        {
            DrawRectangleBox_Obsolete(graphics, rect, backgroundColor, boundaryColor, boundaryWidth, colorChange, cornerRadius);
            graphics.DrawString(text, font, new SolidBrush(textColor), rect);
        }

        public static RectangleF MiddleDownRect(PointF pivot, SizeF size)
        {
            PointF loc = new PointF(pivot.X - size.Width / 2, pivot.Y - size.Height);
            return new RectangleF(loc, size);
        }

        public static RectangleF LeftCenterRect(PointF pivot, SizeF size)
        {
            PointF loc = new PointF(pivot.X , pivot.Y - size.Height/2);
            return new RectangleF(loc, size);
        }

        public static RectangleF RightCenterRect(PointF pivot, SizeF size)
        {
            PointF loc = new PointF(pivot.X - size.Width, pivot.Y - size.Height / 2);
            return new RectangleF(loc, size);
        }

        public static RectangleF MiddleUpRect(PointF pivot, SizeF size)
        {
            PointF loc = new PointF(pivot.X - size.Width / 2, pivot.Y);
            return new RectangleF(loc, size);
        }

        public static RectangleF MaxSquare(RectangleF rect)
        {
            if(rect.Height > rect.Width)
            {
                return LeftCenterRect(new PointF(rect.Left, rect.Top + rect.Height / 2), new SizeF(rect.Width, rect.Width));
            }
            else
            {
                return MiddleUpRect(new PointF(rect.Left + rect.Width / 2, rect.Top), new SizeF(rect.Height, rect.Height));
            }
        }

        public static int MessageBoxHeight(string str, int boxWidth)
        {
            if (str == null || str == "")
            {
                return 0;
            }

            Font font = GH_FontServer.StandardAdjusted;
            Size size = GH_FontServer.MeasureString(str, font);
            boxWidth += 8;
            if (size.Width > boxWidth)
            {
                double num = (double)boxWidth / (double)size.Width;
                font = GH_FontServer.NewFont(font, Convert.ToSingle((double)font.SizeInPoints * num));
                size = GH_FontServer.MeasureString(str, font);
            }
            return Math.Max(size.Height, 6);
        }


        public static void HighLightObject(Graphics graphics, IGH_DocumentObject Owner, Color color, int offset = 8, int radius = 8)
        {
            Rectangle relayrect = GH_Convert.ToRectangle(Owner.Attributes.Bounds);
            relayrect.Inflate(offset, offset);

            int height = 0;
            if(Owner is GH_Component)
            {
                GH_Component com = Owner as GH_Component;
                height = MessageBoxHeight(com.Message, (int)Owner.Attributes.Bounds.Width);
            }

            Rectangle rect = new Rectangle(relayrect.X, relayrect.Y, relayrect.Width, relayrect.Height + height);
            GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(rect, GH_Palette.Pink, radius, 0);
            gH_Capsule.Render(graphics, color);
            gH_Capsule.Dispose();
        }

        public static void RenderButtonIcon_Obsolete(Graphics graphics, GH_Component Owner, RectangleF bound, bool on, Bitmap onMap, Bitmap offMap, int cornerRadius = 6, GH_Palette normalPalette = GH_Palette.Normal)
        {
            GH_Palette palette = Owner.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count > 0 ? GH_Palette.Error : Owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Count > 0 ? GH_Palette.Warning : normalPalette;
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(palette, Owner.Attributes.Selected, Owner.Locked, Owner.Hidden);
            GH_Capsule cap = GH_Capsule.CreateCapsule(bound, palette, cornerRadius, 0);
            cap.Render(graphics, on ? onMap : offMap, impliedStyle);
        }

        public static void RenderButtonText_Obsolete(Graphics graphics, GH_Component Owner, RectangleF bound, bool on, string text, Color onColor, Color offColor, Font font, int cornerRadius = 6, GH_Palette normalPalette = GH_Palette.Normal)
        {
            GH_Palette palette = Owner.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count > 0 ? GH_Palette.Error : Owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Count > 0 ? GH_Palette.Warning : normalPalette;
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(palette, Owner.Attributes.Selected, Owner.Locked, Owner.Hidden);
            GH_Capsule cap = GH_Capsule.CreateCapsule(bound, palette, cornerRadius, 0);
            cap.Render(graphics, impliedStyle);
            Brush brush = new SolidBrush(on ? onColor : offColor);
            graphics.DrawString(text, font, brush, bound);

        }

        public static void RenderButtonText_Obsolete(Graphics graphics, GH_Component Owner, RectangleF bound, bool on, string text, Color onColor, Color offColor, int cornerRadius = 6, GH_Palette normalPalette = GH_Palette.Normal)
        {
            RenderButtonText_Obsolete(graphics, Owner, bound, on, text, onColor, offColor, GH_FontServer.Standard, cornerRadius, normalPalette);
            
        }


        public enum InflateMode { Horizontal, Vertical, Both }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
