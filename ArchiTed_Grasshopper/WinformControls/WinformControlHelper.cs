/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace ArchiTed_Grasshopper.WinformControls
{
    public static class WinformControlHelper
    {


        #region ButtonRender

        public static void ButtonIconRender(Graphics graphics, GH_CanvasChannel channel, IButtonIcon button)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                GH_Palette palette = button.RenderSet.normalPalette;
                GH_PaletteStyle impliedStyle = GetPaletteStype(button, out palette);

                GH_Capsule cap = GH_Capsule.CreateCapsule(button.Bounds, palette, button.RenderSet.CornerRadius, button.RenderSet.HighLight);
                cap.Render(graphics, button.Icon, impliedStyle);
            }
            
        }

        public static void ButtonStringRender(Graphics graphics, GH_CanvasChannel channel, IButtonString owner)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                GH_Palette palette = owner.RenderSet.normalPalette;
                GH_PaletteStyle impliedStyle = GetPaletteStype(owner, out palette);

                GH_Capsule cap = GH_Capsule.CreateCapsule(owner.Bounds, palette, owner.RenderSet.CornerRadius, owner.RenderSet.HighLight);
                cap.Render(graphics, impliedStyle);
                Brush brush = new SolidBrush(owner.RightColor);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                graphics.DrawString(owner.Name, owner.RenderSet.Font, brush, owner.Bounds, format);
            }
        }

        public static GH_PaletteStyle GetPaletteStype(IButton button, out GH_Palette palette)
        {
            return GetPaletteStype(button.Owner, button.Enable, button.RenderSet.normalPalette, out palette);

        }

        public static GH_PaletteStyle GetPaletteStype(ControllableComponent Owner, bool enable, GH_Palette normal, out GH_Palette palette)
        {
            palette = Owner.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count > 0 ? GH_Palette.Error : Owner.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Count > 0 ? GH_Palette.Warning : normal;
            return GH_CapsuleRenderEngine.GetImpliedStyle(palette, Owner.Attributes.Selected, Owner.Locked || !enable, Owner.Hidden);
        }

        public static Color GetRightColor(Color?[] colors, bool on, bool enable)
        {
            if (colors == null)
            {
                if (enable)
                {
                    return on ? ColorExtension.OnColor : ColorExtension.OffColor;
                }
                else
                {
                    return ColorExtension.UnableColor;
                }
            }
            else
            {
                if (colors.Length != 3)
                    throw new ArgumentOutOfRangeException("colors", "colors' length must be 3!");

                if (enable)
                {
                    return on ? (colors[0] ?? ColorExtension.OnColor) : (colors[1] ?? ColorExtension.OffColor);
                }
                else
                {
                    return colors[2] ?? ColorExtension.UnableColor;
                }
            }
        }
        #endregion

        #region Window
        public static void CreateWindow(LangWindow window, ControllableComponent owner)
        {
            if(window.GetType() != typeof(LangWindow) && window != null)
            {
                LangWindow newWindow = Activator.CreateInstance(window.GetType(), owner) as LangWindow;
                CreateWindowHide(newWindow);
            }
        }



        public static void CreateWindow<T>(ControllableComponent owner) where T : LangWindow
        {
            if(typeof(T) != typeof(LangWindow))
            {
                LangWindow window = Activator.CreateInstance(typeof(T), owner) as LangWindow;
                CreateWindowHide(window);
            }

        }

        private static void CreateWindowHide(LangWindow window)
        {
            WindowInteropHelper ownerHelper = new WindowInteropHelper(window);
            ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;
            window.Show();
            LanguagableComponent.LanguageChanged += window.WindowLanguageChanged;
        }
        #endregion

        #region SetRect

        private static int[,] GetIntArrays(int row, int column)
        {
            int[,] datas = new int[row * column, 2];

            int index = 0;
            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < column; columnIndex++)
                {
                    datas[index, 0] = rowIndex;
                    datas[index, 1] = columnIndex;
                    index++;
                }
            }
            return datas;
        }

        public static Func<int, Func<RectangleF, RectangleF, RectangleF>> GetInnerRectRightFunc(int row, int column, SizeF size, out PointF outputLayoutMove,
            int width = 3)
        {

            int[,] datas = GetIntArrays(row, column);
            outputLayoutMove = new PointF((size.Width + 2 * width) * column, 0);

            return (i) =>
            {
                return (innerRect, outerRect) =>
                {
                    float yDis = innerRect.Height / row;
                    float y = innerRect.Top + yDis * (datas[i, 0] + 0.5f) - size.Height / 2;
                    float x = innerRect.Right + width + (size.Width + 2 * width) * datas[i, 1];

                    return new RectangleF(new PointF(x, y), size);
                };
            };
        }

        public static Func<int, Func<RectangleF, RectangleF, RectangleF>> GetInnerRectLeftFunc(int row, int column, SizeF size,out PointF inputLayoutMove,
            int width = 3)
        {

            int[,] datas = GetIntArrays(row, column);

            inputLayoutMove = new PointF(-(size.Width + 2 * width) * column, 0);

            return (i) =>
            {
                return (innerRect, outerRect) =>
                {
                    float yDis = innerRect.Height / row;
                    float y = innerRect.Top + yDis * (datas[i, 0] + 0.5f) - size.Height / 2;
                    float x = innerRect.Left - width - (size.Width + 2 * width) * datas[i, 1] - size.Width;

                    return new RectangleF(new PointF(x, y), size);
                };
            };
        }

        public static Func<int, Func<RectangleF, RectangleF, RectangleF>> GetBoundsBottonFunc(int row, int column, SizeF size, int height = 3)
        {
            int[,] datas = GetIntArrays(row, column);

            return (i) =>
            {
                return (innerRect, outerRect) =>
                {
                    float xDis = (outerRect.Width - 4) / column;
                    float x = outerRect.Left + 2 + xDis * (datas[i, 1] + 0.5f) - size.Width / 2;
                    float y = innerRect.Bottom + height + (size.Height + 2 * height) * datas[i, 0];


                    return new RectangleF(new PointF(x, y), size);
                };
            };
        }

        #endregion
    }
}
