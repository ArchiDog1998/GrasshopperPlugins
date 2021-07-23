using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.Unsafe;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using GH_Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;

namespace InfoGlasses.WinformMenu
{
    public class ParamGlassesMenuItem : ToolStripMenuItem
    {
        public enum ParamGlassesProps
        {
            IsUseParamGlasses,

            IsUseWireColor,
            WireColorsDict,
            WireCheckStrongth,
            WireDefaultColor,
            WireSelectedColor,
            WireEmptyColor,
            WireType,
            WirePolylineParam,
        }
        public static SaveableSettings<ParamGlassesProps> Settings { get; } = new SaveableSettings<ParamGlassesProps>(new SettingsPreset<ParamGlassesProps>[]
        {
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseParamGlasses, true, (value)=>
            {
                UseParamFunctionExchange();
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),

            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseWireColor, true, (value)=>
            {
                UseWireColorFuctEx();
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),

            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireColorsDict, new Dictionary<string, Color>()
            {
                {typeof(GH_Arc).FullName, Color.FromArgb(-1768283136)},
                {typeof(GH_Interval).FullName, Color.FromArgb(-1778351844)},
                {typeof(GH_Vector).FullName, Color.FromArgb(-1771730277)},
                {typeof(GH_Mesh).FullName, Color.FromArgb(-1771493440)},
                {typeof(GH_Colour).FullName, Color.FromArgb(-1776211299)},
                {typeof(GH_Material).FullName, Color.FromArgb(-1773379103)},
                {typeof(GH_Transform).FullName, Color.FromArgb(-1769826362)},
                {typeof(GH_Integer).FullName, Color.FromArgb(-1774941952)},
                {typeof(GH_Field).FullName, Color.FromArgb(-1765785043)},
                {typeof(GH_Number).FullName, Color.FromArgb(-1772182511)},
                {typeof(GH_String).FullName, Color.FromArgb(-1769709085)},
                {typeof(GH_ComplexNumber).FullName, Color.FromArgb(-1768619953)},
                {typeof(GH_Plane).FullName, Color.FromArgb(-1771079424)},
                {typeof(GH_StructurePath).FullName, Color.FromArgb(-1769347669)},
                {typeof(GH_Culture).FullName, Color.FromArgb(-1766827621)},
                {typeof(GH_Time).FullName, Color.FromArgb(-1768117833)},
                {typeof(GH_Line).FullName, Color.FromArgb(-1768665576)},
                {typeof(GH_LonLatCoordinate).FullName, Color.FromArgb(-1765315623)},
                {typeof(GH_Interval2D).FullName, Color.FromArgb(-1778352545)},
                {typeof(GH_Brep).FullName, Color.FromArgb(-1768136448)},
                {typeof(GH_Rectangle).FullName, Color.FromArgb(-1765318856)},
                {typeof(IGH_GeometricGoo).FullName, Color.FromArgb(-1765812870)},
                {typeof(GH_GeometryGroup).FullName, Color.FromArgb(-1772966912)},
                {typeof(GH_Matrix).FullName, Color.FromArgb(-1773424128)},
                {typeof(GH_MeshingParameters).FullName, Color.FromArgb(-1771877728)},
                {typeof(GH_Box).FullName, Color.FromArgb(-1769114368)},
                {typeof(GH_Boolean).FullName, Color.FromArgb(-1775009536)},
                {typeof(GH_Circle).FullName, Color.FromArgb(-1768675584)},
                {typeof(GH_Curve).FullName, Color.FromArgb(-1770258432)},
                {typeof(GH_Surface).FullName, Color.FromArgb(-1768062976)},
                {typeof(GH_MeshFace).FullName, Color.FromArgb(-1769647932)},
                {typeof(GH_Guid).FullName, Color.FromArgb(-1770759205)},
                {typeof(GH_Point).FullName, Color.FromArgb(-1771214177)},
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireCheckStrongth, 0.0, (value)=> Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireDefaultColor, Color.FromArgb(150, 0, 0, 0), (value)=>
            {
                //GH_Skin.wire_default = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireSelectedColor, Color.FromArgb(125, 210, 40), (value)=>                 
            {
                GH_Skin.wire_selected_a = GH_Skin.wire_selected_b = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireEmptyColor, Color.FromArgb(180, 255, 60, 0), (value) =>
            {
                GH_Skin.wire_empty = (Color)value;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireType, 0, (value) => Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WirePolylineParam, 0.5),

        }, Grasshopper.Instances.Settings);

        public ParamGlassesMenuItem() : base(Properties.Resources.WireGlasses)
        {
            this.SetItemLangChange(new string[] { "ParamGlasses", "参数眼镜" }, new string[]{"You can use it to see the extra infomation about parameters.",
                "你可以用它查看参数的更多信息。"});
            this.BindingAndCheckProperty(Settings, ParamGlassesProps.IsUseParamGlasses, (value) =>
            {
                //RemovePaintActions();
                //if (value)
                //    AddPaintActions();
                Grasshopper.Instances.ActiveCanvas.Refresh();
            });

            WinFormPlus.ItemSet<ParamGlassesProps>[] itemsets = new WinFormPlus.ItemSet<ParamGlassesProps>[]
            {
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Default Color", "默认颜色"}, null, null ,true ,ParamGlassesProps.WireDefaultColor),
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Selected Color", "选中颜色"}, null, null ,true ,ParamGlassesProps.WireSelectedColor),
                new WinFormPlus.ItemSet<ParamGlassesProps>(new string[]{ "Empty Color", "空值颜色"}, null, null ,true ,ParamGlassesProps.WireEmptyColor),

            };
            this.DropDown.Items.Add(WinFormPlus.CreateColorBoxItems(Settings,
                new string[] { "Default Wire Color", "默认连线颜色" }, null, null, true, itemsets));

            this.DropDown.Items.Add(WinFormPlus.CreateNumberBox(new string[] { "Wire Strongth", "选中加重" }, null, null,
                Settings, ParamGlassesProps.WireCheckStrongth, 20, 0));

            ToolStripMenuItem wireTypeItem = WinFormPlus.CreateComboBoxItemSingle(new string[] { "Wire Type", "连线类型" }, null, null, 
                Settings, ParamGlassesProps.WireType, new string[][]
            {
                new string[]{ "Bezier Curve", "贝塞尔曲线" },
                new string[] { "PolyLine", "多段线" },
                new string[] { "Line", "直线" },
            });
            ((ToolStripMenuItem)wireTypeItem.DropDown.Items[1]).DropDown.Items.Add(WinFormPlus.CreateNumberBox(new string[] { "Polyline Param", "多段线参数"},
                null, null , Settings, ParamGlassesProps.WirePolylineParam, 1, 0));
            this.DropDown.Items.Add(wireTypeItem);

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(GetShowWireColorItem());

            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += (canvas) => GH_Skin.wire_default = (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);
        }

        #region Department
        private ToolStripMenuItem GetShowWireColorItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Use Advanced Wire Color", "使用强化连线颜色" },
                new string[] { "Click to show use advanced wire color.", "单击以显示强化连线颜色。" },
                Properties.Resources.WireIcon, Settings, ParamGlassesProps.IsUseWireColor);

            item.DropDown.Items.Add(WinFormPlus.CreateClickItem(new string[] { "WireColor", "连线颜色" },
                new string[] { "Click to open the wirecolor window.", "单击以打开连线颜色窗口。" }, null, (x, y) =>
                {
                    //new ExceptionWindowPlus(this).Show();
                }));

            return item;
        }
        #endregion


        #region Function to overwrite
        private static void UseParamFunctionExchange()
        {
            MethodInfo oldFunc = typeof(GH_Painter).GetMethod(nameof(GH_Painter.ConnectionPath));
            MethodInfo newFunc = typeof(ParamGlassesMenuItem).GetMethod(nameof(MyConnectionPath));
            UnsafeHelper.ExchangeMethod(oldFunc, newFunc);
            UseParamControl();
        }

        private static void UseWireColorFuctEx()
        {
            MethodInfo oldFunc = typeof(GH_Painter).GetMethod(nameof(GH_Painter.DetermineWireType));
            MethodInfo newFunc = typeof(ParamGlassesMenuItem).GetMethod(nameof(MyDetermineWireType));

            UnsafeHelper.ExchangeMethod(oldFunc, newFunc);
        }

        private static void UseParamControl()
        {
            MethodInfo oldFunc = typeof(GH_ComponentAttributes).GetMethod(nameof(GH_ComponentAttributes.RenderComponentParameters));
            MethodInfo newFunc = typeof(ParamGlassesMenuItem).GetMethod(nameof(MyRenderComponentParameters));

            //UnsafeHelper.ExchangeMethod(oldFunc, newFunc);
            ChangeLayout();
        }

        private static void ChangeLayout()
        {
            MethodInfo oldFunc = typeof(GH_ComponentAttributes).GetMethod(nameof(GH_ComponentAttributes.LayoutInputParams));
            MethodInfo newFunc = typeof(ParamGlassesMenuItem).GetMethod(nameof(MyLayoutInputParams));

            //UnsafeHelper.ExchangeMethod(oldFunc, newFunc);
        }
        public static GH_WireType MyDetermineWireType(IGH_Structure target)
        {
            if ((bool)Settings.GetProperty(ParamGlassesProps.IsUseWireColor) && (bool)Settings.GetProperty(ParamGlassesProps.IsUseParamGlasses))
                GH_Skin.wire_default = FindColor(target);

            if (target == null || target.IsEmpty)
            {
                return GH_WireType.@null;
            }
            switch (target.PathCount)
            {
                case 0:
                    return GH_WireType.@null;
                case 1:
                    if (target.get_Branch(0).Count == 0)
                    {
                        return GH_WireType.@null;
                    }
                    if (target.get_Branch(0).Count == 1)
                    {
                        return GH_WireType.item;
                    }
                    return GH_WireType.list;
                default:
                    return GH_WireType.tree;
            }
        }

        private static Color FindColor(IGH_Structure target)
        {
            if (target == null) return (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);

            //Find DataType.
            if (target.AllData(true).Count() > 0)
            {
                Type findType = target.AllData(true).ElementAt(0).GetType();

                if (false)
                    findType = target.AllData(true).GroupBy((x) => { return x.GetType(); }).OrderBy((group) => group.Count()).Reverse().ElementAt(0).Key;

                try
                {
                    var dict = ((Dictionary<string, Color>)Settings.GetProperty(ParamGlassesProps.WireColorsDict));
                    Color color = dict[findType.FullName];
                    return color;

                }
                catch
                {
                    return (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);
                }
            }
            return (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);

        }

        /// <summary>
        /// For GH_Painter.ConnectionPath
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="directionA"></param>
        /// <param name="directionB"></param>
        /// <returns></returns>
        public static GraphicsPath MyConnectionPath(PointF pointA, PointF pointB, GH_WireDirection directionA, GH_WireDirection directionB)
        {
            float movemont = 2.5f;
            if(directionA == 0)
            {
                pointA = new PointF(pointA.X - movemont, pointA.Y);
                pointB = new PointF(pointB.X + movemont, pointB.Y);
            }
            else
            {
                pointA = new PointF(pointA.X + movemont, pointA.Y);
                pointB = new PointF(pointB.X - movemont, pointB.Y);
            }

            GraphicsPath graphicsPath = new GraphicsPath();
            switch ((int)Settings.GetProperty(ParamGlassesProps.WireType))
            {
                case 0:
                    BezierF bezierF = ((directionA != 0) ? GH_Painter.ConnectionPathBezier(pointA, pointB).Reverse() : GH_Painter.ConnectionPathBezier(pointB, pointA));
                    graphicsPath.AddBezier(bezierF.P3, bezierF.P2, bezierF.P1, bezierF.P0);

                    break;
                case 1:
                    float moveMent = (pointA.X - pointB.X) * (float)(double)Settings.GetProperty(ParamGlassesProps.WirePolylineParam);
                    moveMent = Math.Max(moveMent, 20);
                    PointF C = new PointF(pointA.X - moveMent, pointA.Y);
                    PointF D = new PointF(pointB.X + moveMent, pointB.Y);
                    graphicsPath.AddLine(pointA, C);
                    graphicsPath.AddLine(C, D);
                    graphicsPath.AddLine(D, pointB);
                    graphicsPath.Reverse();
                    break;
                case 2:
                    graphicsPath.AddLine(pointA, pointB);
                    graphicsPath.Reverse();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(ParamGlassesProps.WireType.ToString() + "is out of range!");

            }
            return graphicsPath;
        }


        #endregion

        #region ParamControl
        public static void MyRenderComponentParameters(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            float width = 50;

            int zoomFadeLow = GH_Canvas.ZoomFadeLow;
            if (zoomFadeLow < 5)
            {
                return;
            }

            FieldInfo field = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();

            StringFormat farCenter = GH_TextRenderingConstants.FarCenter;
            canvas.SetSmartTextRenderingHint();
            Color color = Color.FromArgb(zoomFadeLow, style.Text);
            SolidBrush solidBrush = new SolidBrush(color);
            foreach (IGH_Param item in owner.Params.Input)
            {
                RectangleF bounds = item.Attributes.Bounds;
                if (!(bounds.Width < 1f))
                {
                    graphics.DrawString(item.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds, farCenter);
                    graphics.DrawRectangle(new Pen(Color.Green, 3),GH_Convert.ToRectangle(GetControlRectangle(bounds, width)));
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)item.Attributes;
                    GH_StateTagList tags = (GH_StateTagList)field.GetValue(gH_LinkedParamAttributes);
                    if (tags != null)
                    {
                        tags.RenderStateTags(graphics);
                    }
                }
            }
            farCenter = GH_TextRenderingConstants.NearCenter;
            foreach (IGH_Param item2 in owner.Params.Output)
            {
                RectangleF bounds2 = item2.Attributes.Bounds;
                if (!(bounds2.Width < 1f))
                {
                    graphics.DrawString(item2.NickName, GH_FontServer.StandardAdjusted, solidBrush, bounds2, farCenter);
                    GH_LinkedParamAttributes gH_LinkedParamAttributes2 = (GH_LinkedParamAttributes)item2.Attributes;
                    GH_StateTagList tags = (GH_StateTagList)field.GetValue(gH_LinkedParamAttributes2);
                    if (tags != null)
                    {
                        tags.RenderStateTags(graphics);
                    }
                }
            }
            solidBrush.Dispose();
        }

        public static void MyLayoutInputParams(IGH_Component owner, RectangleF componentBox)
        {
            float movement = 50;

            FieldInfo field = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();

            int count = owner.Params.Input.Count;
            if (count == 0)
            {
                return;
            }
            int num = 0;
            foreach (IGH_Param item in owner.Params.Input)
            {
                num = Math.Max(num, GH_FontServer.StringWidth(item.NickName, GH_FontServer.StandardAdjusted));
            }
            num = Math.Max(num + 6, 12);
            float num2 = componentBox.Height / (float)count;
            int num3 = count - 1;
            for (int i = 0; i <= num3; i++)
            {
                IGH_Param iGH_Param = owner.Params.Input[i];
                if (iGH_Param.Attributes == null)
                {
                    iGH_Param.Attributes = new GH_LinkedParamAttributes(iGH_Param, owner.Attributes);
                }
                float num4 = componentBox.X - (float)num;
                float num5 = componentBox.Y + (float)i * num2;
                float width = num - 3;
                float height = num2;
                iGH_Param.Attributes.Pivot = new PointF(num4 + 0.5f * (float)num - movement, num5 + 0.5f * num2);
                iGH_Param.Attributes.Bounds = GH_Convert.ToRectangle(new RectangleF(num4 - movement, num5, width, height));
            }
            bool flag = false;
            int num6 = count - 1;
            for (int j = 0; j <= num6; j++)
            {
                IGH_Param iGH_Param2 = owner.Params.Input[j];
                GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)iGH_Param2.Attributes;
                field.SetValue(gH_LinkedParamAttributes, iGH_Param2.StateTags);
                GH_StateTagList tags = (GH_StateTagList)field.GetValue(gH_LinkedParamAttributes);
                
                //tags = iGH_Param2.StateTags;
                if (tags.Count == 0)
                {
                    tags = null;
                }
                if (tags != null)
                {
                    flag = true;
                    Rectangle box = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
                    tags.Layout(box, GH_StateTagLayoutDirection.Left);
                    box = tags.BoundingBox;
                    if (!box.IsEmpty)
                    {
                        gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, box);
                    }
                }
            }
            if (flag)
            {
                float num7 = float.MaxValue;
                int num8 = count - 1;
                for (int k = 0; k <= num8; k++)
                {
                    IGH_Attributes attributes = owner.Params.Input[k].Attributes;
                    num7 = Math.Min(num7, attributes.Bounds.X);
                }
                int num9 = count - 1;
                for (int l = 0; l <= num9; l++)
                {
                    IGH_Attributes attributes2 = owner.Params.Input[l].Attributes;
                    RectangleF bounds = attributes2.Bounds;
                    bounds.Width = bounds.Right - num7;
                    bounds.X = num7;
                    attributes2.Bounds = bounds;
                }
            }

        }

        private static RectangleF GetControlRectangle(RectangleF bounds, float width)
        {
            return new RectangleF(bounds.Right, bounds.Top, width, bounds.Height);
        }

        #endregion
    }
}
