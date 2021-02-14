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
    class WireConnectRenderItem : RenderItem, IDisposable
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

            UpdateParamProxy();
            target.OnPingDocument().SolutionEnd += WireConnectRenderItem_SolutionEnd;
        }

        private void WireConnectRenderItem_SolutionEnd(object sender, GH_SolutionEventArgs e)
        {
            UpdateParamProxy();
        }

        public void Dispose()
        {
            try
            {
                Target.OnPingDocument().SolutionEnd -= WireConnectRenderItem_SolutionEnd;
            }
            catch { }
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
            else if(channel == GH_CanvasChannel.Objects && canvas.Viewport.Zoom >= 0.5f)
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
            if (Owner.IsShowLabel)
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
                        dataStr += "...";
                    }
                    else if(datas.PathCount == 1)
                    {
                        for (int k = 0; k < Math.Min(datas.get_Branch(0).Count, Owner.TreeCount); k++)
                        {
                            var obj = datas.get_Branch(0)[k];
                            if(obj == null)
                            {
                                dataStr += "Null\n";
                            }
                            else
                            {
                                dataStr += obj.ToString() + "\n";
                            }
                        }
                        if (datas.get_Branch(0).Count > Owner.TreeCount)
                        {
                            dataStr += "...";
                        }
                    }
                    str += dataStr;
                }
                if (str == "")
                    return;
                SizeF size = graphics.MeasureString(str, font);
                PointF loc = new PointF(pivot.X, pivot.Y + size.Height / 2);
                TextBox.DrawTextBox(graphics, CanvasRenderEngine.MiddleDownRect(loc, size), Owner.LabelBackGroundColor, Owner.LabelBoundaryColor, str, font, Owner.LabelTextColor, isCenter:true);
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
            bool _isRender = false;
            if (param.Attributes.Selected)
            {
                _isRender = true;
            }
            else if (style != 0)
            {
                foreach (IGH_Param source in sources)
                {
                    if (source.Attributes.GetTopLevel.Selected)
                    {
                        _isRender = true;
                        break;
                    }
                }
            }
            else
            {
                _isRender = true;
            }

            int count = sources.Count();
            if (count != ParamProxies.Count)
                UpdateParamProxy();
            if (_isRender)
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
            Type findType = param.Type;

            if (param.VolatileData.AllData(true).Count() > 0)
            {
                switch (Owner.Accuracy)
                {
                    case 0:
                        findType = param.Type;
                        break;
                    case 1:
                        findType = param.VolatileData.AllData(true).ElementAt(0).GetType();
                        break;
                    case 2:
                        var relayList = param.VolatileData.AllData(true).GroupBy((x) => { return x.GetType(); });
                        int count = 0;
                        Type relayName = null;
                        foreach (var item in relayList)
                        {
                            if (item.Count() >= count)
                            {
                                relayName = item.Key;
                                count = item.Count();
                            }

                        }
                        findType = relayName ?? param.VolatileData.AllData(true).ElementAt(0).GetType();
                        break;
                    case 3:
                        List<Type> types = param.VolatileData.AllData(true).Select((x) => x.GetType()).ToList();
                        findType = MinFatherType(types);
                        break;
                    default:
                        findType = param.VolatileData.AllData(true).ElementAt(0).GetType();
                        break;
                }

            }

            foreach (var info in Owner.AllParamProxy)
            {
                if (info.DataType == findType)
                {
                    if (!Owner.ShowProxy.Contains(info))
                    {
                        Owner.ShowProxy.Add(info);
                    }
                    return info;
                }
            }
            GooTypeProxy newInfo = new GooTypeProxy(findType, Owner);
            Owner.AllParamProxy.Add(newInfo);
            Owner.ShowProxy.Add(newInfo);
            return newInfo;
        }

        private Type MinFatherType(List<Type> types)
        {
            List<Type> typesSet = new List<Type>();
            foreach (Type type in types)
            {
                if (!typesSet.Contains(type))
                {
                    typesSet.Add(type);
                }
            }

            while (typesSet.Count > 1)
            {
                typesSet[0] = MinFatherType(typesSet[0], typesSet[1]);
                typesSet.RemoveAt(1);
            }

            Type finalType = typesSet[0];
            if (finalType.IsGenericType)
            {
                Type[] interfaces = finalType.GetInterfaces();
                if (interfaces.Length > 0)
                {
                    finalType = interfaces[interfaces.Length - 1];
                }
            }
            return finalType;
        }

        private Type MinFatherType(Type type1, Type type2)
        {

            //if type1 is father, return father.
            if (IsSubclassOf(type1, type2)) return type1;

            //get type1's baseType.
            Type baseType = type1.BaseType;
            if (baseType.IsGenericType)
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            //return type1's father
            return MinFatherType(baseType, type2);
        }

        private bool IsSubclassOf(Type father, Type son)
        {
            //Check is object
            if (son == typeof(object))
                return false;
            //Check is father
            else if (father.IsAssignableFrom(son))
                return true;

            //get son's true father and check is father.
            Type baseType = son.BaseType;
            if (baseType.IsGenericType)
            {
                baseType = baseType.GetGenericTypeDefinition();
            }
            return IsSubclassOf(father, baseType);
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
                        float moveMent = (pointA.X - pointB.X) * (float)Owner.PolywireParam;
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
                        graphicsPath = GH_Painter.ConnectionPath(pointA, pointB, directionA, directionB);
                        break;
                }

                if (graphicsPath == null)
                {
                    graphicsPath = new GraphicsPath();
                    graphicsPath.AddLine(pointA, pointB);
                }

                Pen pen = GenerateWirePen(pointA, pointB, selectedA, selectedB, type, color, canvas);
                Pen Back = null;

                if (pen == null)
                {
                    pen = new Pen(Color.Black);
                }

                if (selectedA || selectedB)
                {
                    //pen.Width += (float)Owner.SelectWireThickness;
                    Back = new Pen(ColorExtension.OnColor, pen.Width + (float)Owner.SelectWireThickness);
                }

                try
                {
                    if(Back != null)
                    {
                        graphics.DrawPath(Back, graphicsPath);
                    }
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
