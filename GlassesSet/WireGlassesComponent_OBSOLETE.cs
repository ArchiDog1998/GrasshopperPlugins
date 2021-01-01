using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using ArchiTed_Grasshopper;
using System.Windows.Forms;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic.CompilerServices;
using Grasshopper;
using Grasshopper.GUI;
using Rhino.Commands;
using GH_IO.Serialization;
using System.Windows.Media.Imaging;
using System.Windows;
using Grasshopper.Kernel.Special;
using System.Windows.Interop;
using Grasshopper.Kernel.Types;
using System.Linq;
using System.IO;
using System.Text;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace InfoGlasses
{
    public class WireGlassesComponent_OBSOLETE : LanguagableComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.hidden;

        public Color defaultColor;
        public Color UnselectedColor;
        public Color selectedColor;
        public Color emptyColor;

        public List<ParamTypeInfo> allParamType = null;
        public List<IGH_DocumentObject> allDocObjs = new List<IGH_DocumentObject>();

        private bool IsFirst = true;


        public class WireGlassesComponentAttributes : GH_ComponentAttributes
        {
            public new WireGlassesComponent_OBSOLETE Owner;
            RectangleF labelRect;
            RectangleF legendRect;
            List<ParamTypeInfo> showedInfo = new List<ParamTypeInfo>();

            public WireGlassesComponentAttributes(WireGlassesComponent_OBSOLETE owner) : base(owner)
            {
                Owner = owner;
            }

            protected override void Layout()
            {
                int width = 25;
                int heightThick = 10;
                int upDownPad = 12;
                int leftRightPad = 3;

                base.Layout();

                this.Bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width + width + 2 * leftRightPad, this.Bounds.Height);


                //this.Bounds = new RectangleF(this.Bounds.Location, new SizeF(this.Bounds.Width + width + 2 * leftRightPad, this.Bounds.Height));

                float x = this.m_innerBounds.Right + leftRightPad;
                float height = ((this.Bounds.Height - 2 * upDownPad - heightThick) / 2);
                labelRect = new RectangleF(x, this.Bounds.Y + upDownPad, width, height);
                legendRect = new RectangleF(x, this.Bounds.Y + upDownPad + height + heightThick, width, height);
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                base.Render(canvas, graphics, channel);
                if(channel == GH_CanvasChannel.Wires)
                {
                    showedInfo = new List<ParamTypeInfo>();

                    foreach (var item in Owner.allDocObjs)
                    {
                        if(item is IGH_Param)
                        {
                            IGH_Param param = item as IGH_Param;
                            if(param.SourceCount > 0)
                                NewRenderIncomingWires(param, canvas, graphics);

                        }
                        else if(item is IGH_Component)
                        {
                            IGH_Component component = item as IGH_Component;
                            foreach (var param in component.Params.Input)
                            {
                                if (param.SourceCount > 0)
                                    NewRenderIncomingWires(param, canvas, graphics);
                            }
                        }
                        else if(item is GH_Cluster)
                        {
                            GH_Cluster cluster = item as GH_Cluster;
                            foreach (var param in cluster.Params.Input)
                            {
                                if (param.SourceCount > 0)
                                    NewRenderIncomingWires(param, canvas, graphics);
                            }
                        }
                    }
                }
                if(channel == GH_CanvasChannel.Objects)
                {
                    CanvasRenderEngine.RenderButtonIcon_Obsolete(graphics, Owner, labelRect, !Owner.Locked && Owner.GetValue("ShowLabel", false), Properties.Resources.LabelIcon, Properties.Resources.LabelIcon_Locked, normalPalette: GH_Palette.Hidden);
                    CanvasRenderEngine.RenderButtonIcon_Obsolete(graphics, Owner, legendRect, !Owner.Locked && Owner.GetValue("ShowLegend", false), Properties.Resources.LegendIcon, Properties.Resources.LegendIcon_Locked, normalPalette: GH_Palette.Hidden);
                }
                if(channel == GH_CanvasChannel.Last)
                {
                    if(Owner.GetValue("ShowLegend", false))
                    {
                        DrawLegend(graphics);
                    }
                }
            }

            #region Wires
            protected void NewRenderIncomingWires(IGH_Param param, GH_Canvas canvas, Graphics graphics)
            {
                IEnumerable<IGH_Param> sources = param.Sources;
                GH_ParamWireDisplay style = param.WireDisplay;
                Font font = new Font(GH_FontServer.StandardBold.FontFamily, Owner.GetValue("LabelSize", 5));

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

                            Color color = Owner.GetColor(info);
                            GH_WireType type = GH_Painter.DetermineWireType(source2.VolatileData);
                            DrawConnection(param.Attributes.InputGrip, source2.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source2.Attributes.Selected, type, color, canvas, graphics);

                            if(Owner.GetValue("ShowLabel", false))
                            {
                                PointF pivot = new PointF((source2.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source2.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                                string str = info.Name;
                                PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245)), Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30)), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));

                            }
                        }
                        return;
                    }
                    foreach (IGH_Param source3 in sources)
                    {
                        ParamTypeInfo info = FindOrCreateInfo(source3);

                        Color color = Owner.GetColor(info);
                        DrawConnection(param.Attributes.InputGrip, source3.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source3.Attributes.Selected, GH_WireType.generic, color, canvas, graphics);
                        
                        if (Owner.GetValue("ShowLabel", false))
                        {
                            PointF pivot = new PointF((source3.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source3.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                            string str = info.Name;
                            PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                            CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245)), Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30)), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));
                        }
                    }
                    return;
                }
                switch (style)
                {
                    case GH_ParamWireDisplay.faint:
                        foreach (IGH_Param source4 in sources)
                        {
                            ParamTypeInfo info = FindOrCreateInfo(source4);

                            Color color = Owner.GetColor(info);
                            DrawConnection(param.Attributes.InputGrip, source4.Attributes.OutputGrip, GH_WireDirection.left, GH_WireDirection.right, param.Attributes.Selected, source4.Attributes.Selected, GH_WireType.faint, color, canvas, graphics);
                            
                            if (Owner.GetValue("ShowLabel", false))
                            {
                                PointF pivot = new PointF((source4.Attributes.OutputGrip.X + param.Attributes.InputGrip.X) / 2, (source4.Attributes.OutputGrip.Y + param.Attributes.InputGrip.Y) / 2);
                                string str = info.Name;
                                PointF loc = new PointF(pivot.X, pivot.Y + graphics.MeasureString(str, font).Height / 2);
                                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, loc, Color.FromArgb(50,  Owner.GetValue("BackgroundColor", Color.FromArgb(200, 245, 245, 245))),
                                   Color.FromArgb(50, Owner.GetValue("BoundaryColor", Color.FromArgb(200, 30, 30, 30))), str, font, Owner.GetValue("TextColor", Color.FromArgb(200, Color.Black)));

                            }
                        }
                        break;
                    case GH_ParamWireDisplay.hidden:
                        {
                            break;
                            //float num = graphics.Transform.Elements[0];
                            //if (num > 0.55f)
                            //{
                            //    System.Drawing.Point point = GH_Convert.ToPoint(InputGrip);
                            //    int alpha = GH_GraphicsUtil.BlendInteger(0.5, 1.0, 0, 80, num);
                            //    int num2 = 0;
                            //    do
                            //    {
                            //        int num3 = 6 + 3 * num2;
                            //        Rectangle rect = new Rectangle(point.X - num3, point.Y - num3, 2 * num3, 2 * num3);
                            //        LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(alpha, 0, 0, 0), LinearGradientMode.Vertical);
                            //        linearGradientBrush.WrapMode = WrapMode.TileFlipXY;
                            //        linearGradientBrush.SetSigmaBellShape(0.5f);
                            //        Pen pen = new Pen(linearGradientBrush, 1f);
                            //        graphics.DrawEllipse(pen, rect);
                            //        pen.Dispose();
                            //        linearGradientBrush.Dispose();
                            //        num2++;
                            //    }
                            //    while (num2 <= 3);
                            //}
                            //break;
                        }
                }
            }

            private ParamTypeInfo FindOrCreateInfo(IGH_Param param)
            {
                string typeFullName = param.Type.FullName;

                if(param.VolatileData.AllData(true).Count() > 0)
                {
                    switch (Owner.GetValue("Accuracy", 1))
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

                foreach (var info in Owner.allParamType)
                {
                    if (info.TypeFullName == typeFullName)
                    {
                        if (!showedInfo.Contains(info))
                        {
                            showedInfo.Add(info);
                        }
                        return info;
                    }
                }
                ParamTypeInfo newInfo = new ParamTypeInfo(param);
                Owner.allParamType.Add(newInfo);
                showedInfo.Add(newInfo);
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
                    switch(Owner.GetValue("WireType", 0))
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
                return new Pen(GenerateWirePen_Fill(A, B, A_Selected, B_Selected, Empty, color), 3f);
            }

            public Brush GenerateWirePen_Fill(PointF a, PointF b, bool asel, bool bsel, bool empty, Color color)
            {
                if (asel && bsel)
                {
                    return new SolidBrush(Owner.selectedColor);
                }
                if (!asel && !bsel)
                {
                    if (empty)
                    {
                        return new SolidBrush(Owner.emptyColor);
                    }
                    return new SolidBrush(color);
                }
                if (empty)
                {
                    color = Owner.emptyColor;
                }
                float num = Math.Abs(a.X - b.X);
                float num2 = Math.Abs(a.Y - b.Y);
                RectangleF rect = RectangleF.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
                rect.Inflate(2f, 2f);
                LinearGradientBrush linearGradientBrush = (num > num2) ? ((!((asel & (a.X < b.X)) | (bsel & (b.X < a.X)))) ? new LinearGradientBrush(rect, color, Owner.selectedColor, LinearGradientMode.Horizontal) : new LinearGradientBrush(rect, Owner.selectedColor, color, LinearGradientMode.Horizontal)) : ((!((asel & (a.Y < b.Y)) | (bsel & (b.Y < a.Y)))) ? new LinearGradientBrush(rect, color, Owner.selectedColor, LinearGradientMode.Vertical) : new LinearGradientBrush(rect, Owner.selectedColor, color, LinearGradientMode.Vertical));
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

            private void DrawLegend(Graphics graphics)
            {
                float zoom = Grasshopper.Instances.ActiveCanvas.Viewport.Zoom;
                float size = (float)Owner.GetValue("LegendSize", 20) / zoom;
                float spacing = (float)Owner.GetValue("LegendSpacing", 30) / zoom;
                float mult = 0.8f;
               
                float width = 0f;
                foreach (var info in showedInfo)
                {
                        float newWidth = graphics.MeasureString(info.Name, new Font(GH_FontServer.Standard.FontFamily, size * mult/2)).Width;
                        if (newWidth > width)
                        {
                            width = newWidth;
                        }
                        
                }
                float height = showedInfo.Count * size;
                width += size;
                float oneWayWidth = width;
                RectangleF rect = Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion;
                int heightMult = (int)(height / (rect.Height - 2 * spacing));
                if(heightMult > 0)
                {
                    height = rect.Height - 2 * spacing;
                    width += heightMult * (width + size);
                }

                PointF pivot;
                switch(Owner.GetValue("LegendMode", 2))
                {
                    case 0:
                        pivot = new PointF(rect.X + spacing, rect.Y + height + spacing);
                        break;
                    case 1:
                        pivot = new PointF(rect.X + spacing, rect.Y + rect.Height - spacing);
                        break;
                    case 2:
                        pivot = new PointF(rect.Right - width - spacing, rect.Y + rect.Height - spacing);
                        break;
                    case 3:
                        pivot = new PointF(rect.Right - width - spacing, rect.Y + height + spacing);
                        break;
                    default:
                        pivot = new PointF(rect.X, rect.Y + height);
                        break;
                }

                RectangleF background = new RectangleF(pivot.X, pivot.Y - height, width, height);
                background.Inflate(size / 4, size / 4);
                GraphicsPath path = CanvasRenderEngine.GetRoundRectangle_Obsolete(background, size/4);
                graphics.FillPath(new SolidBrush(Owner.GetValue("LegendBackGroundColor", Color.FromArgb(80, 255, 255, 255))), path);
                graphics.DrawPath(new Pen(Owner.GetValue("LegendBoundaryColor", Color.FromArgb(80, 0, 0, 0)), size/15), path);

                float actHeight = 0;
                float startY = pivot.Y;
                foreach (var info in showedInfo)
                {
                    DrawOneLegend(graphics, pivot, info, size, mult);
                    actHeight += size;
                    if(actHeight > rect.Height - 2 * spacing)
                    {
                        actHeight = 0;
                        pivot = new PointF(pivot.X + oneWayWidth + size, startY);

                    }
                    else
                    {
                        pivot = new PointF(pivot.X, pivot.Y - size);
                    }
                }
                
            }

            private void DrawOneLegend(Graphics graphics, PointF pivot, ParamTypeInfo info, float size, float mult)
            {

                float height = size * mult;
                float padding = size * (1 - mult) / 2;

                RectangleF fillRect = new RectangleF(pivot.X, pivot.Y - size, size, size);
                fillRect.Inflate(-padding, -padding);
                graphics.FillPath(new SolidBrush(Owner.GetColor(info)), CanvasRenderEngine.GetRoundRectangle_Obsolete(fillRect, padding));

                graphics.DrawString(info.Name, new Font(GH_FontServer.Standard.FontFamily, height/2), new SolidBrush(Owner.GetValue("LegendTextColor", Color.FromArgb(200, Color.Black))), new PointF(pivot.X + size + padding, pivot.Y - height - padding));
            }
            #region Respond

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (m_innerBounds.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    WireColors_OBSOLETE window = new WireColors_OBSOLETE(Owner);
                    WindowInteropHelper ownerHelper = new WindowInteropHelper(window);
                    ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;
                    window.Show();
                    LanguageChanged += window.WindowLanguageChanged;

                    return GH_ObjectResponse.Release;
                    
                }
                return base.RespondToMouseDoubleClick(sender, e);

            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if ((labelRect.Contains(e.CanvasLocation) || legendRect.Contains(e.CanvasLocation)) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    return GH_ObjectResponse.Handled;
                }
                return base.RespondToMouseDown(sender, e);
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (labelRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.SetValue("ShowLabel", !Owner.GetValue("ShowLabel", false));
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                else if (legendRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                     Owner.SetValue("ShowLegend", !Owner.GetValue("ShowLegend", false));
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                return base.RespondToMouseUp(sender, e);
            }

            #endregion
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new WireGlassesComponentAttributes(this);
        }


/// <summary>
/// Each implementation of GH_Component must provide a public 
/// constructor without any arguments.
/// Category represents the Tab in which the component will appear, 
/// Subcategory the panel. If you use non-existing tab or panel names, 
/// new tabs/panels will automatically be created.
/// </summary>
        public WireGlassesComponent_OBSOLETE()
          : base(GetTransLation(new string[] { "WireGlasses", "连线眼镜" }), GetTransLation(new string[] { "Wire", "连线" }),
               GetTransLation(new string[] { "To show the wire' advances information.Right click or double click to have advanced options.", "显示连线的高级信息。右键或者双击可以获得更多选项。" }),
              "Params", "Showcase Tools")
        {
            LanguageChanged += ResponseToLanguageChanged;
            ResponseToLanguageChanged(this, new EventArgs());
        }

        private void SetTranslateColor()
        {
            GH_Skin.wire_default = Color.Transparent;
            GH_Skin.wire_empty = Color.Transparent;
            GH_Skin.wire_selected_a = Color.Transparent;
            GH_Skin.wire_selected_b = Color.Transparent;
        }

        private void SetDefaultColor()
        {
            GH_Skin.wire_default = defaultColor;
            GH_Skin.wire_empty = emptyColor;
            GH_Skin.wire_selected_a = selectedColor;
            GH_Skin.wire_selected_b = UnselectedColor;
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            LanguageChanged -= ResponseToLanguageChanged;
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires -= ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;

           SetDefaultColor();


            try
            {
                this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted -= ObjectsDeletedAction;
            }
            catch
            {

            }
            base.RemovedFromDocument(document);
        }

        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            string[] input1 = new string[] { GetTransLation(new string[] { "DefaultColor", "默认颜色" }), GetTransLation(new string[] { "Dc", "默认颜色" }), GetTransLation(new string[] { "DefaultColor", "默认颜色" }) };
            string[] input2 = new string[] { GetTransLation(new string[] { "SelectColor", "选中颜色" }), GetTransLation(new string[] { "Sc", "选中颜色" }), GetTransLation(new string[] { "SelectColor", "选中颜色" }) };
            string[] input3 = new string[] { GetTransLation(new string[] { "UnselectColor", "未选颜色" }), GetTransLation(new string[] { "Uc", "未选颜色" }), GetTransLation(new string[] { "UnselectColor", "未选颜色" }) };
            string[] input4 = new string[] { GetTransLation(new string[] { "EmptyColor", "空线颜色" }), GetTransLation(new string[] { "Ec", "空线颜色" }), GetTransLation(new string[] { "EmptyColor", "空线颜色" }) };


            ChangeComponentAtt(this, new string[] {GetTransLation(new string[] { "WireGlasses", "连线眼镜" }), GetTransLation(new string[] { "Wire", "连线" }),
                GetTransLation(new string[] { "To show the wire' advances information.Right click or double click to have advanced options.", "显示连线的高级信息。右键或者双击可以获得更多选项。" }) },
                new string[][] { input1, input2, input3, input4 }, new string[][] {});

            this.ExpireSolution(true);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter(GetTransLation(new string[] { "DefaultColor", "默认颜色" }), GetTransLation(new string[] { "Dc", "默认颜色" }), GetTransLation(new string[] { "DefaultColor", "默认颜色" }), GH_ParamAccess.item, Color.FromArgb(150, 0, 0, 0));
            pManager.AddColourParameter(GetTransLation(new string[] { "SelectColor", "选中颜色" }), GetTransLation(new string[] { "Sc", "选中颜色" }), GetTransLation(new string[] { "SelectColor", "选中颜色" }), GH_ParamAccess.item, Color.FromArgb(125, 210, 40));
            pManager.AddColourParameter(GetTransLation(new string[] { "UnselectColor", "未选颜色" }), GetTransLation(new string[] { "Uc", "未选颜色" }), GetTransLation(new string[] { "UnselectColor", "未选颜色" }), GH_ParamAccess.item, Color.FromArgb(50, 0, 0, 0));
            pManager.AddColourParameter(GetTransLation(new string[] { "EmptyColor", "空线颜色" }), GetTransLation(new string[] { "Ec", "空线颜色" }), GetTransLation(new string[] { "EmptyColor", "空线颜色" }), GH_ParamAccess.item, Color.FromArgb(180, 255, 60, 0));
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref defaultColor);
            DA.GetData(1, ref selectedColor);
            DA.GetData(2, ref UnselectedColor);
            DA.GetData(3, ref emptyColor);

            if (IsFirst)
            {

                this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted += ObjectsDeletedAction;
                Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires += ActiveCanvas_CanvasPrePaintWires;
                Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
                Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;

                GetProxy();
                GetObject();

                bool read = true;
                foreach (var item in allParamType)
                {
                    if (GetColor(item) != defaultColor)
                        read = false;
                }
                if (read)
                    Readtxt();

                IsFirst = false;
            }

        }


        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires -= ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;

            if (sender.Document == this.OnPingDocument())
            {
                Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires += ActiveCanvas_CanvasPrePaintWires;
                Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            }
        }

        private void ActiveCanvas_CanvasPostPaintWires(GH_Canvas sender)
        {
            SetDefaultColor();
        }

        private void ActiveCanvas_CanvasPrePaintWires(GH_Canvas sender)
        {
            SetTranslateColor();
        }

        private void ObjectsDeletedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                if (allDocObjs.Contains(docObj))
                    allDocObjs.Remove(docObj);

                //if (docObj is IGH_Param)
                //{
                //    IGH_Param param = docObj as IGH_Param;
                //    foreach (var proxy in allParamType)
                //    {
                //        if (proxy.AllInstances.Contains(param))
                //        {
                //            proxy.AllInstances.Remove(param);
                //            break;
                //        }
                //    }
                //}
                //else if (docObj is GH_Component)
                //{
                //    GH_Component com = docObj as GH_Component;
                //    foreach (var param in com.Params.Input)
                //    {
                //        foreach (var proxy in allParamType)
                //        {
                //            if (proxy.AllInstances.Contains(param))
                //            {
                //                proxy.AllInstances.Remove(param);
                //                break;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    GH_Cluster cluster = docObj as GH_Cluster;
                //    foreach (var param in cluster.Params.Input)
                //    {
                //        foreach (var proxy in allParamType)
                //        {
                //            if (proxy.AllInstances.Contains(param))
                //            {
                //                proxy.AllInstances.Remove(param);
                //                break;
                //            }
                //        }
                //    }
                //}
            }
        }

        private void ObjectsAddedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                AddOneObject(docObj);
            }
        }

        private void GetObject()
        {
            foreach (var item in this.OnPingDocument().Objects)
            {
                AddOneObject(item);
            }
        }

        private void AddOneObject(IGH_DocumentObject obj)
        {
            if (obj is WireGlassesComponent_OBSOLETE && obj != this)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, GetTransLation(new string[] { "Please place only one WireGlasses component per document!", "请在每个文档中只放置一个连线眼镜运算器！" }));
            }

            if (obj is GH_Component || obj is IGH_Param || obj is GH_Cluster)
            {
                allDocObjs.Add(obj);
            }
                
        }

        private void GetProxy()
        {
            allParamType = new List<ParamTypeInfo>();
            foreach (IGH_ObjectProxy item in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                Type type = item.Type;
                if (type != null)
                {

                    //if (IsGenericSubclassOf(type, typeof(GH_PersistentParam<>)))
                    //if (IsGenericSubclassOf(type, typeof(GH_Param<>)))
                    if (typeof(IGH_Param).IsAssignableFrom(type))
                    {
                        ParamTypeInfo info = new ParamTypeInfo(item);
                        if (info.HasInput) continue;
                        //if (info.IsPlugin) continue;
                        bool addFlag = true;

                        if (allParamType.Count > 0)
                        {
                            foreach (var infoex in allParamType)
                            {
                                if (infoex.TypeFullName == info.TypeFullName)
                                {
                                    addFlag = false;

                                    if (infoex.IsPlugin && !info.IsPlugin)
                                    {
                                        allParamType.Remove(infoex);
                                        allParamType.Add(info);
                                    }
                                    else if (!infoex.IsPlugin && !info.IsPlugin)
                                    {
                                        if (info.AssemblyName.Contains("Grasshopper"))
                                        {
                                            if (infoex.AssemblyName.Contains("Grasshopper"))
                                            {
                                                if (!infoex.ParamFullName.Contains("Parameters"))
                                                {
                                                    allParamType.Remove(infoex);
                                                    allParamType.Add(info);
                                                }
                                                else if (info.ParamFullName.Contains("Parameters") && infoex.ParamFullName.Contains("Parameters"))
                                                {
                                                    if (infoex.ParamFullName.Contains("Script"))
                                                    {
                                                        allParamType.Remove(infoex);
                                                        allParamType.Add(info);
                                                    }
                                                    if (infoex.ParamFullName.Contains("Path"))
                                                    {
                                                        allParamType.Remove(infoex);
                                                        allParamType.Add(info);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                allParamType.Remove(infoex);
                                                allParamType.Add(info);
                                            }

                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        
                        if(addFlag)
                        {
                            allParamType.Add(info);
                        }
                    }
                }
            }
        }

        //public bool IsGenericSubclassOf(Type type, Type superType)
        //{
        //    if (type.BaseType != null
        //        && !type.BaseType.Equals(typeof(object))
        //        && type.BaseType.IsGenericType)
        //    {
        //        if (type.BaseType.GetGenericTypeDefinition().Equals(superType))
        //        {
        //            return true;
        //        }
        //        return IsGenericSubclassOf(type.BaseType, superType);
        //    }

        //    return false;
        //}

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.WireGlasses;
            }
        }



        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3e579f72-106a-4544-bebc-f3a673bc8c98"); }
        }

        public Color GetColor(ParamTypeInfo info)
        {
            return GetValue(info.TypeFullName.ToString(), defaultColor);
        }

        internal void SetColor(ParamTypeInfo info, Color color)
        {
            SetValue(info.TypeFullName.ToString(), color);
        }

        internal Dictionary<string, Color> ColorDefinationToDict()
        {
            Dictionary<string, Color> dict = new Dictionary<string, Color>();
            foreach (var info in allParamType)
            {
                if (GetColor(info) != defaultColor)
                    dict[info.TypeFullName] = GetColor(info);
            }
            return dict;
        }

        internal void ReadDictionary(Dictionary<string, Color> dict)
        {
            foreach (var item in dict.Keys)
            {
                SetValue(item.ToString(), dict[item]);
            }
        }

        internal void WriteTxt(Dictionary<string, Color> dict)
        {
            string name = "WireGlasses_Default";
            string path = Grasshopper.Folders.DefaultAssemblyFolder + name + ".txt";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string saveStr = "";

            foreach (var item in dict.Keys)
            {
                saveStr += item.ToString() + ',' + dict[item].ToArgb().ToString() + "\n";
            }

            sw.Write(saveStr);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void Readtxt()
        {
            string name = "WireGlasses_Default";
            string path = Grasshopper.Folders.DefaultAssemblyFolder + name + ".txt";

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);

                try
                {
                    while (true)
                    {

                        string[] strs = sr.ReadLine().Split(',');
                        SetValue(strs[0], Color.FromArgb(int.Parse(strs[1])));


                    }

                }
                catch
                {

                }
            }
            catch
            {

            }

        }

        internal void ResetColor()
        {
            foreach (var info in allParamType)
            {
                SetColor(info, defaultColor);
            }
        }

        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            {
                ToolStripMenuItem exceptionsItem = new ToolStripMenuItem(GetTransLation(new string[] { "Wire Colors", "连线颜色" }));
                exceptionsItem.Click += exceptionClick;
                exceptionsItem.ToolTipText = GetTransLation(new string[] { "Click to set the wire color for different data type.", "点击以设定不同数据类型的连线颜色。" });
                exceptionsItem.Font = GH_FontServer.StandardBold;
                exceptionsItem.ForeColor = System.Drawing.Color.FromArgb(19, 34, 122);

                void exceptionClick(object sender, EventArgs e)
                {
                    WireColors_OBSOLETE window = new WireColors_OBSOLETE(this);
                    WindowInteropHelper ownerHelper = new WindowInteropHelper(window);
                    ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;
                    window.Show();
                    LanguageChanged += window.WindowLanguageChanged;
                }
                menu.Items.Add(exceptionsItem);


                string[] values = new string[] { LanguagableComponent.GetTransLation(new string[] { "Rough", "粗糙" }),
                    LanguagableComponent.GetTransLation(new string[] { "Medium", "中等" }),
                    LanguagableComponent.GetTransLation(new string[] { "High", "高" }),
                };
                WinFormPlus.AddLoopBoexItem(menu, this, LanguagableComponent.GetTransLation(new string[] { "Value Type Accuracy", "数值类型精度" }),
                     true, values, 1, "Accuracy");


                string[] values2 = new string[] { LanguagableComponent.GetTransLation(new string[] { "Bezier", "贝塞尔曲线" }),
                    LanguagableComponent.GetTransLation(new string[] { "Polyline", "折线" }),
                    LanguagableComponent.GetTransLation(new string[] { "Line", "直线" }),
                };
                WinFormPlus.AddLoopBoexItem(menu, this, LanguagableComponent.GetTransLation(new string[] { "Wire Type", "连线类型" }),
                     true, values2, 0, "WireType");
            }
            GH_DocumentObject.Menu_AppendSeparator(menu);

            {
                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Label Options", "标签选项" }), GetValue("ShowLabel", false) ? System.Drawing.Color.FromArgb(19, 34, 122) : System.Drawing.Color.FromArgb(110, 110, 110));


                WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Legend FontSize", "设置标签字体大小" }),
                       GetTransLation(new string[] { "Set the label's font size.", "设置标签气泡框的字体大小。" }), Properties.Resources.SizeIcon, GetValue("ShowLabel", false),
                       8, 4, 50, "LabelSize");


                WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[] {

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Text Color", "文字颜色" }),GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" }),
                    null, true,Color.FromArgb(200, Color.Black), "TextColor"),

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Background Color", "背景颜色" }), GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" }),
                    null, true, Color.FromArgb(200, 245, 245, 245), "BackgroundColor"),

                    new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Boundary Color", "边框颜色" }),
                            GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" }), null, true,
                            Color.FromArgb(200, 30, 30, 30), "BoundaryColor"),
                    };
                WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Colors", "颜色" }),
                GetTransLation(new string[] { "Adjust interface color.", "调整界面颜色。" }), Properties.Resources.ColorIcon, GetValue("ShowLabel", false), sets);


            }
            GH_DocumentObject.Menu_AppendSeparator(menu);
            {
                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Legend Options", "图例选项" }), GetValue("ShowLegend", false) ? System.Drawing.Color.FromArgb(19, 34, 122) : System.Drawing.Color.FromArgb(110, 110, 110));

                {

                    string[] values = new string[] { LanguagableComponent.GetTransLation(new string[] { "Left Top", "左上角" }),
                    LanguagableComponent.GetTransLation(new string[] { "Left Bottom", "左下角" }),
                    LanguagableComponent.GetTransLation(new string[] { "Right Bottom", "右下角" }),
                    LanguagableComponent.GetTransLation(new string[] { "Right Top", "右上角" }),
                    };
                    Bitmap[] maps = new Bitmap[] { Properties.Resources.LeftTopIcon, Properties.Resources.LeftBottomIcon, 
                        Properties.Resources.RightBottomIcon, Properties.Resources.RightTopIcon };
                    WinFormPlus.AddLoopBoexItem(menu, this, LanguagableComponent.GetTransLation(new string[] { "Legend Location", "图例位置" }),
                         true, values, 2, "LegendMode", maps);


                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Legend Size", "设置图例大小" }),
                       GetTransLation(new string[] { "Set the legend's size.", "设置图例的大小。" }), Properties.Resources.SizeIcon, GetValue("ShowLegend", false),
                       20, 10, 100, "LegendSize");


                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Legend Spacing", "设置图例边距" }),
                        GetTransLation(new string[] { "Set the legend's spacing.", "设置图例的边距。" }), Properties.Resources.DistanceIcon, GetValue("ShowLegend", false),
                        30, 0, 200, "LegendSpacing");


                    WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[] {

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Text Color", "文字颜色" }),GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" }),
                    null, true,Color.FromArgb(200, Color.Black), "LegendTextColor"),

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Background Color", "背景颜色" }), GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" }), 
                    null, true, Color.FromArgb(80, 255, 255, 255), "LegendBackGroundColor"),

                    new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Boundary Color", "边框颜色" }),
                            GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" }), null, true,
                            Color.FromArgb(80, 0, 0, 0), "LegendBoundaryColor"),
                    };
                    WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Colors", "颜色" }),
                    GetTransLation(new string[] { "Adjust interface color.", "调整界面颜色。" }), Properties.Resources.ColorIcon, GetValue("ShowLegend", false), sets);



                }

            }
            base.AppendAdditionComponentMenuItems(menu);
        }

      

        protected override void AppendHelpMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddURLItem(menu, "插件介绍视频（黄同学制作）", "点击以到B站查看黄同学制作的插件介绍视频。",
             WinFormPlus.ItemIconType.Bilibili  , "https://www.bilibili.com/video/BV11y4y1z7VS");
            GH_DocumentObject.Menu_AppendSeparator(menu.DropDown);
            WinFormPlus.AddURLItem(menu, GetTransLation(new string[] { "See Source Code", "查看源代码" }), GetTransLation(new string[] { "Click to the GitHub to see source code.", "点击以到GitHub查看源代码。" }),
              WinFormPlus.ItemIconType.GitHub, "https://github.com/ArchiDog1998/Showcase-Tools");
        }

        private void menu_KeyDown(GH_MenuTextBox sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) sender.CloseEntireMenuStructure();
        }

        public override void CreateWindow()
        {
            throw new NotImplementedException();
        }
    }

    public class ParamTypeInfo
    {
        public Bitmap Icon { get; set; }
        public BitmapSource IconSource { get; set; }

        public string TypeFullName { get; set; }
        public String Name { get; set; }

        public string  AssemblyName { get; set; }

        public string AssemblyDesc { get; set; }
        public string ParamFullName { get; set; }
        public string ProxyDesc { get; set; }
        public BitmapSource AssemblyIconSource { get; set; }

        public bool IsPlugin { get; set; }

        public bool HasInput { get; set; }

        private void CreateAttribute(IGH_Param param)
        {
            this.Icon = param.Icon_24x24;
            this.IconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(this.Icon, 20, 20));
            this.ProxyDesc = param.Description;


            this.HasInput = param.IsDataProvider;
            Type type = param.Type;

            this.TypeFullName = type.FullName;

            //void ScheduleCallback(GH_Document doc)
            //{
            this.Name = param.Type.Name;
            //}
            //GH_Document.GH_ScheduleDelegate callback = new GH_Document.GH_ScheduleDelegate(ScheduleCallback);
            //document.ScheduleSolution(10, callback);


            this.ParamFullName = param.GetType().FullName;

            //this.AllInstances = new List<IGH_Param>();
            GH_AssemblyInfo assem = ComTypeInfo.GetGHLibrary(param.GetType());
            this.AssemblyName = assem.Assembly.GetName().Name;
            this.AssemblyDesc = assem.Description;

            this.IsPlugin = !assem.IsCoreLibrary;
            if (assem.Icon != null)
                this.AssemblyIconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(assem.Icon, 16, 16));
            else
                this.AssemblyIconSource = CanvasRenderEngine.BitmapToBitmapImage(new Bitmap(16, 16));
        }

        public ParamTypeInfo(IGH_ObjectProxy proxy)
        {

            IGH_Param param = ((IGH_Param)proxy.CreateInstance());
            CreateAttribute(param);
        }

        public ParamTypeInfo(IGH_Param param)
        {
            CreateAttribute(param);
        }




    }


}
