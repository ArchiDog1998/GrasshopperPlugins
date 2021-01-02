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
using InfoGlasses.WPF;
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
        public ParamGlassesComponent Owner { get; }

        #region Wire Properties

        /// <summary>
        /// the param that used in the right document.
        /// </summary>
        public static List<GooTypeProxy> LegendParamInfo { get; internal set; }
        #endregion
        public List<GooTypeProxy> ParamProxies { get; private set; }

        #endregion

        public WireConnectRenderItem(IGH_Param target, ParamGlassesComponent owner, Func<bool> showFunc = null)
            : base(target, showFunc, true)
        {
            if (!target.Attributes.HasInputGrip)
                throw new ArgumentOutOfRangeException("Target must has InputGrip!");
            ParamProxies = new List<GooTypeProxy>();
            this.Owner = owner;

            //target.OnPingDocument().SolutionEnd += WireConnectRenderItem_SolutionEnd;
        }

        private void WireConnectRenderItem_SolutionEnd(object sender, GH_SolutionEventArgs e)
        {
            UpdateParamProxy();
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
                rectF = RectangleF.Union(rectF, new RectangleF(upParam.Attributes.OutputGrip, new SizeF(0, 0)));
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
            if (channel == GH_CanvasChannel.Wires)
            {
                NewRenderIncomingWires(Target as IGH_Param, canvas, graphics);
            }
            else if(channel == GH_CanvasChannel.Objects)
            {
                Font font = new Font(GH_FontServer.StandardBold.FontFamily, (float)Owner.LabelFontSize);
                for (int i = 0; i < ((IGH_Param)this.Target).SourceCount; i++)
                {
                    RenderTextBox(graphics, font, i);
                }
            }
        }

        private void UpdateParamProxy()
        {
            var sources = ((IGH_Param)this.Target).Sources;
            if (ParamProxies.Count != sources.Count)
            {
                ParamProxies = new List<GooTypeProxy>();

                foreach (IGH_Param item in sources)
                {
                    ParamProxies.Add(FindOrCreateInfo(item));
                }
            }
        }

        private void RenderTextBox(Graphics graphics, Font font, int i)
        {
            if (Owner.IsShowLabel || Owner.IsShowTree)
            {
                string str = "";
                PointF pivot = new PointF((((IGH_Param)Target).Sources.ElementAt(i).Attributes.OutputGrip.X + ((IGH_Param)Target).Attributes.InputGrip.X) / 2, 
                    ((((IGH_Param)Target).Sources.ElementAt(i).Attributes.OutputGrip.Y + ((IGH_Param)Target).Attributes.InputGrip.Y) / 2));
                if (Owner.IsShowLabel) str += ParamProxies[i].TypeName + "\n";
                if (Owner.IsShowTree)
                {
                    string dataStr = "";
                    var datas = ((IGH_Param)Target).Sources.ElementAt(i).VolatileData;
                    for (int j = 0; j < Math.Min(datas.PathCount, Owner.TreeCount); j++)
                    {
                        dataStr += datas.get_Path(j).ToString() + "    N = " +
                        datas.get_Branch(j).Count.ToString() + "\n";
                    }
                    if (datas.PathCount > Owner.TreeCount)
                    {
                        dataStr += "\n...";
                    }
                    str += dataStr;
                }
                if (str == "")
                    return;
                SizeF size = graphics.MeasureString(str, font);
                PointF loc = new PointF(pivot.X, pivot.Y + size.Height / 2);
                TextBox.DrawTextBox(graphics, CanvasRenderEngine.MiddleDownRect(loc, size), Owner.LabelBackGroundColor, Owner.LabelBoundaryColor, str, font, Owner.LabelTextColor); ;
            }
        }

        #region Wires
        protected void NewRenderIncomingWires(IGH_Param param, GH_Canvas canvas, Graphics graphics)
        {
            IEnumerable<IGH_Param> sources = param.Sources;
            GH_ParamWireDisplay style = param.WireDisplay;

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

            int count = sources.Count();
            UpdateParamProxy();
            if (flag)
            {
                if (CentralSettings.CanvasFancyWires)
                {
                    for (int i = 0; i < count; i++)
                    {
                        GH_WireType type = GH_Painter.DetermineWireType(sources.ElementAt(i).VolatileData);
                        DrawConnection(param.Attributes.InputGrip, sources.ElementAt(i).Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, sources.ElementAt(i).Attributes.Selected, type, ParamProxies[i].ShowColor, canvas, graphics);
                    }
                    return;
                }
                for (int i = 0; i < count; i++)
                {
                    DrawConnection(param.Attributes.InputGrip, sources.ElementAt(i).Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, sources.ElementAt(i).Attributes.Selected, GH_WireType.generic, ParamProxies[i].ShowColor, canvas, graphics);
                }
                return;
            }
            switch (style)
            {
                case GH_ParamWireDisplay.faint:
                    for (int i = 0; i < count; i++)
                    {
                        DrawConnection(param.Attributes.InputGrip, sources.ElementAt(i).Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, sources.ElementAt(i).Attributes.Selected, GH_WireType.faint, ParamProxies[i].ShowColor, canvas, graphics);
                    }
                    break;
                case GH_ParamWireDisplay.hidden:
                    {
                        break;
                    }
            }
        }

        private GooTypeProxy FindOrCreateInfo(IGH_Param param)
        {
            string typeFullName = param.Type.FullName;

            if (param.VolatileData.AllData(true).Count() > 0)
            {
                switch (Owner.Accuracy)
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

            foreach (var info in Owner.AllParamProxy)
            {
                if (info.TypeFullName == typeFullName)
                {
                    if (!Owner.ShowProxy.Contains(info))
                    {
                        Owner.ShowProxy.Add(info);
                    }
                    return info;
                }
            }
            GooTypeProxy newInfo = new GooTypeProxy(param.Type, Owner);
            Owner.AllParamProxy.Add(newInfo);
            Owner.ShowProxy.Add(newInfo);
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

        public void DrawConnection(PointF pointA, PointF pointB, GH_WireDirection directionA, GH_WireDirection directionB, bool selectedA, bool selectedB, GH_WireType type, Color colorinput, GH_Canvas canvas, Graphics graphics)
        {
            if (ConnectionVisible(pointA, pointB, canvas))
            {
                Color color = colorinput;
                if (selectedA || selectedB)
                {
                    color.SolidenColor(Owner.SelectWireSolid);
                }

                GraphicsPath graphicsPath = new GraphicsPath();
                switch (Owner.WireType)
                {
                    case 0:
                        graphicsPath = GH_Painter.ConnectionPath(pointA, pointB, directionA, directionB);
                        break;
                    case 1:
                        float distance = pointB.X - pointA.X;
                        PointF C = new PointF(pointA.X + distance * (float)Owner.PolywireParam, pointA.Y);
                        PointF D = new PointF(pointB.X - distance * (float)Owner.PolywireParam, pointB.Y);
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

                if (selectedA || selectedB)
                    pen.Width += (float)Owner.SelectWireThickness;

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
            return new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), 3f);
        }

        public Brush GenerateWirePen_Fill(PointF a, PointF b, bool asel, bool bsel, bool empty, Color color)
        {
            if (asel && bsel)
            {
                return new SolidBrush(Owner.SelectedColor);
            }
            if (!asel && !bsel)
            {
                if (empty)
                {
                    return new SolidBrush(Owner.EmptyColor);
                }
                return new SolidBrush(color);
            }
            if (empty)
            {
                color = Owner.EmptyColor;
            }
            float num = Math.Abs(a.X - b.X);
            float num2 = Math.Abs(a.Y - b.Y);
            RectangleF rect = RectangleF.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
            rect.Inflate(2f, 2f);
            LinearGradientBrush linearGradientBrush = (num > num2) ? ((!((asel & (a.X < b.X)) | (bsel & (b.X < a.X)))) ? new LinearGradientBrush(rect, color, Owner.SelectedColor, LinearGradientMode.Horizontal) : new LinearGradientBrush(rect, Owner.SelectedColor, color, LinearGradientMode.Horizontal)) : ((!((asel & (a.Y < b.Y)) | (bsel & (b.Y < a.Y)))) ? new LinearGradientBrush(rect, color, Owner.SelectedColor, LinearGradientMode.Vertical) : new LinearGradientBrush(rect, Owner.SelectedColor, color, LinearGradientMode.Vertical));
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
