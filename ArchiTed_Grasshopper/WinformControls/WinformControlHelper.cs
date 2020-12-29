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
        #region Param
        
        #region ParamIO
        public static void SetData<T>(ITargetParam<T> owner, T value)
        {
            GH_PersistentParam<GH_Goo<T>> target = owner.Target;

            GH_Document.GH_ScheduleDelegate callback = new GH_Document.GH_ScheduleDelegate(ScheduleCallback);
            target.OnPingDocument().ScheduleSolution(20, callback);

            void ScheduleCallback(GH_Document doc)
            {
                target.RecordUndoEvent(typeof(T).Name + "Changed!");
                {
                    target.PersistentData.Clear();
                    target.SetPersistentData(value);
                }
            }
        }

        public static T GetData<T>(ITargetParam<T> owner, out GH_ParamAccess access)
        {
            access = GH_ParamAccess.item;
            if (owner.Target.Attributes.HasInputGrip)
            {
                owner.Enable = owner.Target.SourceCount == 0;
            }
            else
            {
                owner.Enable = false;
            }
           

            if (owner.Target.VolatileData.AllData(true).Count() > 0)
            {
                if(owner.Target.VolatileData.AllData(false).Count() > 1)
                {
                    access = owner.Target.VolatileData.PathCount == 1 ? GH_ParamAccess.list : GH_ParamAccess.tree;
                }
                GH_Goo<T> gh_color = owner.Target.VolatileData.AllData(true).ElementAt(0) as GH_Goo<T>;
                return gh_color.Value;

            }
            else
            {
                WinformControlHelper.SetData<T>(owner, owner.Default);
                return owner.Default;
            }
        }

        //public static string GetSuffix(GH_ParamAccess access)
        //{
        //    switch (access)
        //    {
        //        case GH_ParamAccess.item:
        //            return "I";
        //        case GH_ParamAccess.list:
        //            return "L";
        //        case GH_ParamAccess.tree:
        //            return "T";
        //        default:
        //            throw new Exception("access is invalid!");
        //    }
        //}
        #endregion

        #region ParamLayout

        public static RectangleF ParamLayoutBase(IGH_Attributes targetAtt, int Width, RectangleF innerRect, RectangleF outerRect)
        {
            int side = 10;

            //check whether input
            if (targetAtt.HasInputGrip)
            {
                return new RectangleF(outerRect.Left - Width - side, targetAtt.Bounds.Top, Width, targetAtt.Bounds.Height);
            }
            else
            {
                return new RectangleF(outerRect.Right + side, targetAtt.Bounds.Top, Width, targetAtt.Bounds.Height);
            }

        }

        #endregion
        #endregion

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

        public static Func<int, Func<RectangleF, RectangleF, RectangleF>> GetInnerRectRightFunc(int row, int column, SizeF size, int width = 3)
        {

            int[,] datas = GetIntArrays(row, column);

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
