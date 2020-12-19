using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Microsoft.VisualBasic.CompilerServices;

namespace InfoGlasses.WinformControls
{
    class WireConnectRenderItem : RenderItem
    {
        #region Stastic Properties
        /// <summary>
        /// the wire width in the case of connected component is selected.
        /// </summary>
        private readonly static float _selectedWireWidth = 5f;

        private static WireColorSet _colorSet;
        /// <summary>
        /// Get the default color set about wire display.
        /// </summary>
        public static WireColorSet ColorSet

        {
            get { return _colorSet; }
            internal set { _colorSet = value; }
        }

        private static int _wireType = 0;
        /// <summary>
        /// Define which wire type should be displayed.
        /// </summary>
        public static int WireType
        {
            get { return _wireType; }
            internal set { _wireType = value; }
        }

        private static int _accuracy = 1;
        /// <summary>
        /// the accuracy of getting value type.
        /// </summary>
        public static int Accuracy
        {
            get { return _accuracy; }
            internal set { _accuracy = value; }
        }


        private static List<ParamTypeInfo> _allParamInfo = null;
        /// <summary>
        /// Alll Param that in grasshopper.
        /// </summary>
        public static List<ParamTypeInfo> AllParamInfo
        {
            //Need to set the Default Get AllParamType!!!
            get { return _allParamInfo; }
        }

        /// <summary>
        /// the param that used in the right document.
        /// </summary>
        public static List<ParamTypeInfo> LegendParamInfo { get; internal set; }
        #endregion

        public WireConnectRenderItem(IGH_Param target, Func<bool> showFunc = null, bool renderLittleZoom = false)
            : base(target, showFunc, renderLittleZoom)
        {
            if (!target.Attributes.HasInputGrip)
                throw new ArgumentOutOfRangeException("Target must has InputGrip!");
        }

        /// <summary>
        /// Get all wires Bounds.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="graphics"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        protected override RectangleF Layout(GH_Canvas canvas, Graphics graphics, RectangleF rect)
        {
            IGH_Param param = this.Target as IGH_Param;
            RectangleF rectF = new RectangleF(param.Attributes.InputGrip, new SizeF(0, 0));
            foreach (IGH_Param upParam in param.Sources)
            {
                RectangleF.Union(rectF, new RectangleF(upParam.Attributes.OutputGrip, new SizeF(0, 0)));
            }
            return rectF;
        }

        /// <summary>
        /// Draw Wire
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="graphics"></param>
        /// <param name="channel"></param>
        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if(channel == GH_CanvasChannel.Wires)
            {
                NewRenderIncomingWires(Target as IGH_Param, canvas, graphics);
            }
        }

        #region Wires
        protected void NewRenderIncomingWires(IGH_Param param, GH_Canvas canvas, Graphics graphics)
        {
            IEnumerable<IGH_Param> sources = param.Sources;
            GH_ParamWireDisplay style = param.WireDisplay;
            //Font font = new Font(GH_FontServer.StandardBold.FontFamily, Owner.GetValue("LabelSize", 5));

            if (!param.Attributes.HasInputGrip)
            {
                return;
            }
            bool flag = false;
            if (param.Attributes.Selected)
            {
                flag = true;
            }
            else if (style != 0)
            {
                foreach (IGH_Param source in sources)
                {
                    if (source.Attributes.GetTopLevel.Selected)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            else
            {
                flag = true;
            }

            if (flag)
            {
                if (CentralSettings.CanvasFancyWires)
                {
                    foreach (IGH_Param source2 in sources)
                    {
                        ParamTypeInfo info = FindOrCreateInfo(source2);

                        Color color = info.ShowColor;
                        GH_WireType type = GH_Painter.DetermineWireType(source2.VolatileData);
                        DrawConnection(param.Attributes.InputGrip, source2.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source2.Attributes.Selected, type, color, canvas, graphics);

                        //if (Owner.GetValue("ShowLabel", false))
                        //{
                        //    PointF pivot = new PointF((source2.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source2.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                        //    string str = info.Name;
                        //    PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                        //    CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245)), Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30)), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));

                        //}
                    }
                    return;
                }
                foreach (IGH_Param source3 in sources)
                {
                    ParamTypeInfo info = FindOrCreateInfo(source3);

                    Color color = info.ShowColor;
                    DrawConnection(param.Attributes.InputGrip, source3.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source3.Attributes.Selected, GH_WireType.generic, color, canvas, graphics);

                    //if (Owner.GetValue("ShowLabel", false))
                    //{
                    //    PointF pivot = new PointF((source3.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source3.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                    //    string str = info.Name;
                    //    PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                    //    CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245)), Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30)), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));
                    //}
                }
                return;
            }
            switch (style)
            {
                case GH_ParamWireDisplay.faint:
                    foreach (IGH_Param source4 in sources)
                    {
                        ParamTypeInfo info = FindOrCreateInfo(source4);

                        Color color = info.ShowColor;
                        DrawConnection(param.Attributes.InputGrip, source4.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source4.Attributes.Selected, GH_WireType.faint, color, canvas, graphics);

                        //if (Owner.GetValue("ShowLabel", false))
                        //{
                        //    PointF pivot = new PointF((source4.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source4.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                        //    string str = info.Name;
                        //    PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                        //    CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Color.FromArgb(50, Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245))),
                        //       Color.FromArgb(50, Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30))), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));

                        //}
                    }
                    break;
                 case GH_ParamWireDisplay.hidden:
                    {
                        break;
                    }
            }
        }

        private ParamTypeInfo FindOrCreateInfo(IGH_Param param)
        {
            string typeFullName = param.Type.FullName;

            if (param.VolatileData.AllData(true).Count() > 0)
            {
                switch (Accuracy)
                {
                    case 0:
                        typeFullName = param.Type.FullName;
                        break;
                    case 1:
                        typeFullName = param.VolatileData.AllData(true).ElementAt(0).GetType().FullName;
                        break;
                    case 2:
                        var relayList = param.VolatileData.AllData(true).GroupBy((x) => { return x.GetType(); });
                        int count = 0;
                        string relayName = "";
                        foreach (var item in relayList)
                        {
                            if (item.Count() > count)
                                relayName = item.Key.FullName;
                        }
                        typeFullName = relayName;
                        break;
                    case 3:
                        List<Type> types = new List<Type>();
                        param.VolatileData.AllData(true).GroupBy((x) => { return x.GetType(); }).ToList().ForEach((x) => { if (!types.Contains(x.Key)) types.Add(x.Key); });
                        typeFullName = MinFatherType(types).FullName;
                        break;
                    default:
                        typeFullName = param.VolatileData.AllData(true).ElementAt(0).GetType().FullName;
                        break;
                }

            }

            foreach (var info in AllParamInfo)
            {
                if (info.TypeFullName == typeFullName)
                {
                    if (!LegendParamInfo.Contains(info))
                    {
                        LegendParamInfo.Add(info);
                    }
                    return info;
                }
            }
            ParamTypeInfo newInfo = new ParamTypeInfo(param);
            AllParamInfo.Add(newInfo);
            LegendParamInfo.Add(newInfo);
            return newInfo;
        }

        private Type MinFatherType(List<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.IsAssignableFrom(typeof(IGH_Goo)))
                    return typeof(IGH_Goo);
            }
            foreach (Type type1 in types)
            {
                bool flag = true;
                foreach (Type type2 in types)
                {
                    if (!type1.IsAssignableFrom(type2))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return type1;
                }
            }
            List<Type> types1 = new List<Type>();
            foreach (Type type3 in types)
            {
                types1.Add(type3.BaseType);
            }
            return MinFatherType(types1);
        }

        public void DrawConnection(PointF pointA, PointF pointB, GH_WireDirection directionA, GH_WireDirection directionB, bool selectedA, bool selectedB, GH_WireType type, Color color, GH_Canvas canvas, Graphics graphics)
        {
            if (ConnectionVisible(pointA, pointB, canvas))
            {
                GraphicsPath graphicsPath = new GraphicsPath();
                switch (WireType)
                {
                    case 0:
                        graphicsPath = GH_Painter.ConnectionPath(pointA, pointB, directionA, directionB);
                        break;
                    case 1:
                        PointF C = new PointF((pointA.X + pointB.X) / 2, pointA.Y);
                        PointF D = new PointF((pointA.X + pointB.X) / 2, pointB.Y);
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
                        graphicsPath = GH_Painter.ConnectionPath(pointA, pointB, directionA, directionB);
                        break;
                }

                if (graphicsPath == null)
                {
                    graphicsPath = new GraphicsPath();
                    graphicsPath.AddLine(pointA, pointB);
                }
                Pen pen = GenerateWirePen(pointA, pointB, selectedA, selectedB, type, color, canvas);
                if (pen == null)
                {
                    pen = new Pen(Color.Black);
                }
                try
                {
                    graphics.DrawPath(pen, graphicsPath);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    Tracing.Assert(new Guid("{72303320-11AD-484e-BE32-8BDAA7377BE0}"), "Connection could not be drawn:" + Environment.NewLine + ex2.Message + Environment.NewLine + Environment.NewLine + $"A: ({pointA.X}, {pointA.Y})" + Environment.NewLine + $"B: ({pointB.X}, {pointB.Y})" + Environment.NewLine + $"A_Dir: {directionA}" + Environment.NewLine + $"B_Dir: {directionB}" + Environment.NewLine + $"A_Selected: {selectedA}" + Environment.NewLine + $"B_Selected: {selectedB}" + Environment.NewLine + $"Type: {type}");
                    ProjectData.ClearProjectError();
                }
                graphicsPath.Dispose();
                pen.Dispose();
            }
        }


        public bool ConnectionVisible(PointF a, PointF b, GH_Canvas canvas)
        {
            if (Math.Abs(a.X - b.X) < 2f && Math.Abs(a.Y - b.Y) < 2f)
            {
                return false;
            }
            float val = Math.Abs(b.X - a.X);
            float val2 = Math.Abs(b.Y - a.Y);
            if (Math.Max(val, val2) * canvas.Viewport.Zoom < 8f)
            {
                return false;
            }
            RectangleF visibleRegion = canvas.Viewport.VisibleRegion;
            if (a.Y < visibleRegion.Top - 10f && b.Y < visibleRegion.Top - 10f)
            {
                return false;
            }
            if (a.Y > visibleRegion.Bottom + 10f && b.Y > visibleRegion.Bottom + 10f)
            {
                return false;
            }
            float num = 0.25f * Math.Abs(a.Y - b.Y);
            if (a.X < visibleRegion.Left - num && b.X < visibleRegion.Left - num)
            {
                return false;
            }
            if (a.X > visibleRegion.Right + num && b.X > visibleRegion.Right + num)
            {
                return false;
            }
            return true;
        }

        private Pen GenerateWirePen(PointF a, PointF b, bool asel, bool bsel, GH_WireType wiretype, Color color, GH_Canvas canvas)
        {
            switch (wiretype)
            {
                case GH_WireType.generic:
                    return GenerateWirePen_Static_Generic(a, b, asel, bsel, Empty: false, color);
                case GH_WireType.@null:
                    return GenerateWirePen_Static_Generic(a, b, asel, bsel, Empty: true, color);
                case GH_WireType.faint:
                    return GenerateWirePen_Static_Faint(color);
                case GH_WireType.item:
                    return GenerateWirePen_Static_Item(a, b, asel, bsel, Empty: false, color);
                case GH_WireType.list:
                    return GenerateWirePen_Static_List(a, b, asel, bsel, Empty: false, color, canvas);
                case GH_WireType.tree:
                    return GenerateWirePen_Static_Tree(a, b, asel, bsel, Empty: false, color, canvas);
                case GH_WireType.dynamic:
                    return GenerateWirePen_Dynamic(0, color);
                case GH_WireType.dynamicAlternative1:
                    return GenerateWirePen_Dynamic(1, color);
                case GH_WireType.wireless:
                    return GenerateWirePen_WireLess(a, b, asel, bsel, empty: false, color);
                default:
                    return null;
            }
        }
        private Pen GenerateWirePen_WireLess(PointF a, PointF b, bool asel, bool bsel, bool empty, Color color)
        {
            Pen pen = new Pen(GenerateWirePen_Fill(a, b, asel, bsel, empty, color), 4f);
            pen.DashCap = DashCap.Round;
            pen.DashPattern = new float[2]
            {
                    0.1f,
                    1f
            };
            pen.LineJoin = LineJoin.Round;
            return pen;
        }

        private Pen GenerateWirePen_Dynamic(int style, Color color)
        {
            Color baseColor = color;
            if (style == 1)
            {
                baseColor = Color.Crimson;
            }
            Pen pen = new Pen(Color.FromArgb(255, baseColor), 3f);
            pen.CustomStartCap = ArrowLineCap();
            pen.DashPattern = new float[2]
            {
                    2f,
                    1f
            };
            pen.LineJoin = LineJoin.Round;
            return pen;
        }

        private Pen GenerateWirePen_Static_Tree(PointF A, PointF B, bool A_Selected, bool B_Selected, bool Empty, Color color, GH_Canvas canvas)
        {
            if (canvas.Graphics.Transform.Elements[0] < 0.5f)
            {
                return GenerateWirePen_Static_Item(A, B, A_Selected, B_Selected, Empty, color);
            }
            Pen pen = new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), 5f);
            pen.DashCap = DashCap.Round;
            pen.DashPattern = new float[2]
            {
                    3f,
                   0.5f
            };
            pen.CompoundArray = new float[4]
            {
                   0f,
                    0.35f,
                  0.65f,
                   1f
            };
            return pen;
        }

        private Pen GenerateWirePen_Static_List(PointF A, PointF B, bool A_Selected, bool B_Selected, bool Empty, Color color, GH_Canvas canvas)

        {
            if (canvas.Graphics.Transform.Elements[0] < 0.5f)
            {
                return GenerateWirePen_Static_Item(A, B, A_Selected, B_Selected, Empty, color);
            }
            Pen pen = new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), 5f);
            pen.CompoundArray = new float[4]
            {
                    0f,
                    0.4f,
                    0.6f,
                    1f
            };
            return pen;
        }

        private Pen GenerateWirePen_Static_Item(PointF A, PointF B, bool A_Selected, bool B_Selected, bool Empty, Color color)
        {
            return new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), 3f);
        }

        private Pen GenerateWirePen_Static_Faint(Color color)
        {
            return new Pen(Color.FromArgb(50, color), 1.5f)
            {
                LineJoin = LineJoin.Round
            };
        }


        private Pen GenerateWirePen_Static_Generic(PointF A, PointF B, bool A_Selected, bool B_Selected, bool Empty, Color color)
        {
            float width = (A_Selected || B_Selected) ? _selectedWireWidth : 3f;
            return new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), width);
        }

        public Brush GenerateWirePen_Fill(PointF a, PointF b, bool asel, bool bsel, bool empty, Color color)
        {
            if (asel && bsel)
            {
                return new SolidBrush(ColorSet.SelectedColor);
            }
            if (!asel && !bsel)
            {
                if (empty)
                {
                    return new SolidBrush(ColorSet.EmptyColor);
                }
                return new SolidBrush(color);
            }
            if (empty)
            {
                color = ColorSet.EmptyColor;
            }
            float num = Math.Abs(a.X - b.X);
            float num2 = Math.Abs(a.Y - b.Y);
            RectangleF rect = RectangleF.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
            rect.Inflate(2f, 2f);
            LinearGradientBrush linearGradientBrush = (num > num2) ? ((!((asel & (a.X < b.X)) | (bsel & (b.X < a.X)))) ? new LinearGradientBrush(rect, color, ColorSet.SelectedColor, LinearGradientMode.Horizontal) : new LinearGradientBrush(rect, ColorSet.SelectedColor, color, LinearGradientMode.Horizontal)) : ((!((asel & (a.Y < b.Y)) | (bsel & (b.Y < a.Y)))) ? new LinearGradientBrush(rect, color, ColorSet.SelectedColor, LinearGradientMode.Vertical) : new LinearGradientBrush(rect, ColorSet.SelectedColor, color, LinearGradientMode.Vertical));
            if (linearGradientBrush != null)
            {
                linearGradientBrush.WrapMode = WrapMode.TileFlipXY;
                return linearGradientBrush;
            }
            return new SolidBrush(color);
        }

        private CustomLineCap ArrowLineCap()
        {
            float num = 1f;
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddLines(new PointF[5]
            {
                    new PointF(0f, 0f),
                    new PointF(-1f * num, 0f),
                    new PointF(0f, 3f * num),
                    new PointF(1f * num, 0f),
                    new PointF(0f, 0f)
            });
            graphicsPath.CloseAllFigures();
            CustomLineCap customLineCap = new CustomLineCap(null, graphicsPath, LineCap.ArrowAnchor);
            customLineCap.SetStrokeCaps(LineCap.Flat, LineCap.Flat);
            graphicsPath.Dispose();
            return customLineCap;
        }
        #endregion
    }
}
