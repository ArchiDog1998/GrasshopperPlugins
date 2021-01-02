/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextBox = ArchiTed_Grasshopper.WinformControls.TextBox;

namespace InfoGlasses.WinformControls
{
    public static class ParamControlHelper
    {
        #region Param

        #region ParamIO
        public static void SetData<TGoo, T>(ITargetParam<TGoo, T> owner, T value) where TGoo : GH_Goo<T>
        {
            GH_PersistentParam<TGoo> target = owner.Target;

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

        public static T GetData<TGoo, T>(ITargetParam<TGoo, T> owner, out GH_ParamAccess access) where TGoo : GH_Goo<T>
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
                if (owner.Target.VolatileData.AllData(false).Count() > 1)
                {
                    access = owner.Target.VolatileData.PathCount == 1 ? GH_ParamAccess.list : GH_ParamAccess.tree;
                }
                GH_Goo<T> gh_color = owner.Target.VolatileData.AllData(true).ElementAt(0) as GH_Goo<T>;
                return gh_color.Value;

            }
            else
            {
                ParamControlHelper.SetData<TGoo, T>(owner, owner.Default);
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

        public static RectangleF ParamLayoutBase(IGH_Attributes targetAtt, int Width, RectangleF bound, int sideDistance = 8)
        {

            //check whether input
            if (targetAtt.HasInputGrip)
            {
                RectangleF rect = new RectangleF(bound.Left - Width - sideDistance, targetAtt.Bounds.Top, Width, targetAtt.Bounds.Height);
                rect.Inflate(-2, -2);
                return rect;
            }
            else
            {
                RectangleF rect = new RectangleF(bound.Right + sideDistance, targetAtt.Bounds.Top, Width, targetAtt.Bounds.Height);
                rect.Inflate(-2, -2);
                return rect;
            }

        }

        public static RectangleF UpDownSmallRect(RectangleF rect, float dis = 2)
        {
            PointF pivot = new PointF(rect.X, rect.Y + dis);
            return new RectangleF(pivot, new SizeF(rect.Width, rect.Height - 2 * dis));
        }

        #endregion

        #region ParamRender
        public static void RenderParamButtonIcon(Graphics graphics, Bitmap icon, RectangleF bound, float size = 2)
        {

            RectangleF background = bound;
            background.Inflate(size, size);
            GraphicsPath path = TextBox.GetRoundRectangle(background, size);

            graphics.FillPath(new SolidBrush(Color.FromArgb(150, Color.WhiteSmoke)), path);
            graphics.DrawPath(new Pen(Color.DimGray, 1), path);
            graphics.DrawImage(icon, bound);
        }
        #endregion
        #endregion
    }
}
