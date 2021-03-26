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
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseParamGlasses, true, (value)=>UseParamFunctionExchange()),

            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.IsUseWireColor, true, (value)=>
            {
            }),
            new SettingsPreset<ParamGlassesProps>(ParamGlassesProps.WireColorsDict, new Dictionary<string, Color>()),
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
        }


        //直接改GH_Painter.DrawConnection！！
        //用GH_Document.FindWireAt找到父对象。



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

            //Get the outputGrip
            PointF outputGrip = pointA;
            PointF inputGrip = pointB;
            if (directionA == GH_WireDirection.left)
            {
                outputGrip = pointB;
                inputGrip = pointA;
            }
            GH_Skin.wire_default = FindColor(FindParamByInOut(outputGrip, inputGrip));



            GraphicsPath graphicsPath = new GraphicsPath();
            switch ((int)Settings.GetProperty(ParamGlassesProps.WireType))
            {
                case 0:
                    BezierF bezierF = ((directionA != 0) ? GH_Painter.ConnectionPathBezier(pointA, pointB).Reverse() : GH_Painter.ConnectionPathBezier(pointB, pointA));
                    graphicsPath.AddBezier(bezierF.P3, bezierF.P2, bezierF.P1, bezierF.P0);

                    break;
                case 1:
                    float moveMent = (pointA.X - pointB.X) * (float)0.5;
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

        private static IGH_Param FindParamByInOut(PointF outputGrip, PointF inputGrip)
        {
            IGH_Param inputParam = null;
            IGH_Param[] leftParams = FindParamByInputorOutput(outputGrip, false);
            switch (leftParams.Length)
            {
                case 0:
                    //MessageBox.Show("Can't find!");
                    break;
                case 1:
                    inputParam = leftParams[0];
                    break;
                default:
                    IGH_Param[] rightParams = FindParamByInputorOutput(inputGrip, true);
                    if (rightParams.Length > 0)
                    {
                        IEnumerable<IGH_Param> results = leftParams.Where((param) => param.Recipients.Intersect(rightParams).Count() > 0);
                        if (results.Count() == 1) inputParam = results.ElementAt(0);
                    }
                    break;
            }
            return inputParam;
        }

        /// <summary>
        /// Find the Param on the pivot.
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="isInput"></param>
        /// <returns></returns>
        private static IGH_Param[] FindParamByInputorOutput(PointF pivot, bool isInput)
        {
            List<IGH_Param> _params = new List<IGH_Param>();

            foreach (IGH_Attributes att in Grasshopper.Instances.ActiveCanvas.Document.Attributes)
            {
                //Check whether is Param.
                
                IGH_Param param = att.DocObject as IGH_Param;
                if (param == null) continue;

                //Check if point is zhe same.
                if ((!isInput) && PointFIsTheSame(att.OutputGrip,pivot)) _params.Add(param);
                else if(isInput && PointFIsTheSame(att.InputGrip, pivot)) _params.Add(param);
            }
            return _params.ToArray();
        }

        private static bool PointFIsTheSame(PointF pt1, PointF pt2)
        {
            return (pt1.X == pt2.X) && (pt1.Y == pt2.Y);
        }

        private static Color FindColor(IGH_Param inputParam)
        {
            if(inputParam == null) return (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);

            //Find DataType.
            Type findType = inputParam.Type;

            //if (inputParam.VolatileData.AllData(true).Count() > 0)
            //{
            //    switch (Owner.Accuracy)
            //    {
            //        case 0:
            //            findType = inputParam.Type;
            //            break;
            //        case 1:
            //            findType = inputParam.VolatileData.AllData(true).ElementAt(0).GetType();
            //            break;
            //        case 2:
            //            var relayList = inputParam.VolatileData.AllData(true).GroupBy((x) => { return x.GetType(); });
            //            int count = 0;
            //            Type relayName = null;
            //            foreach (var item in relayList)
            //            {
            //                if (item.Count() >= count)
            //                {
            //                    relayName = item.Key;
            //                    count = item.Count();
            //                }

            //            }
            //            findType = relayName ?? inputParam.VolatileData.AllData(true).ElementAt(0).GetType();
            //            break;
            //        default:
            //            findType = inputParam.VolatileData.AllData(true).ElementAt(0).GetType();
            //            break;
            //    }
            //}


            try
            {
                return ((Dictionary<string, Color>)Settings.GetProperty(ParamGlassesProps.WireColorsDict))[findType.FullName];

            }
            catch
            {
                return (Color)Settings.GetProperty(ParamGlassesProps.WireDefaultColor);
            }
        }
        #endregion
    }
}
