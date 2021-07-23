using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using System.Runtime.CompilerServices;
using Whale.Animation;
using System.Runtime.InteropServices;

namespace Whale.Animation
{
    class ShareData
    {
        public static Color ThemeColor = Color.FromArgb(19, 34, 122);
        public static Color HalfThemeColor = Color.FromArgb(128, ThemeColor);
        public static Color QuarterThemeColor = Color.FromArgb(64, ThemeColor);
        public static Color SecondaryColor = Color.DimGray;
        public static Color HighLightColor = Color.DarkRed;

        public static float GraphCurveThickness = 3f;
        public static float lightThick = 0.8f;
        public static float middleThick = 1.5f;

        public static float KeyTimeRadius = 1.5f;
        public static float PointRadius = 1.2f;
        public static float KeyPtRadius = 3f;

        public static int FrameWidth = 6;
        public static int FrameWidthGraph = 10;

        public static float GraphDistance = 40;
    }

    class RenderCanvas
    {
        public static void DrawOutCircle(Graphics graphics, IGH_Param param)
        {
            float height = param.Attributes.Bounds.Height;
            PointF LeftTop = param.Attributes.Bounds.Location;
            float PtX = LeftTop.X - ShareData.PointRadius;
            float PtY = LeftTop.Y + height / 2 - ShareData.PointRadius;
            graphics.DrawEllipse(new Pen(ShareData.ThemeColor, ShareData.PointRadius), PtX, PtY, ShareData.PointRadius, ShareData.PointRadius);

        }

        public static void DrawInCircle(Graphics graphics, IGH_Param param)
        {
            float width = param.Attributes.Bounds.Width;
            float height = param.Attributes.Bounds.Height;
            PointF LeftTop = param.Attributes.Bounds.Location;
            float PtX = LeftTop.X - ShareData.PointRadius + width;
            float PtY = LeftTop.Y + height / 2 - ShareData.PointRadius;
            graphics.DrawEllipse(new Pen(ShareData.ThemeColor, ShareData.PointRadius), PtX, PtY, ShareData.PointRadius, ShareData.PointRadius);

        }

        public static void HighLightCom(Graphics graphics, GH_Component com, int rowsCount)
        {
            Rectangle relayrect = GH_Convert.ToRectangle(com.Attributes.Bounds);
            relayrect.Inflate(ShareData.FrameWidthGraph, ShareData.FrameWidthGraph);
            Rectangle rect = new Rectangle(relayrect.X, relayrect.Y, relayrect.Width, relayrect.Height + rowsCount * 15);
            GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(rect, GH_Palette.Pink, ShareData.FrameWidth, 5);
            gH_Capsule.Render(graphics, ShareData.ThemeColor);
            gH_Capsule.Dispose();
        }


        protected static PointF DrawTimeLine(Graphics graphics, float PtXStart, float PtY)
        {

            float Width = Frame.Component.SliderWidth - 20;
            float PtXend = PtXStart + Width;

            //draw basic line
            graphics.DrawLine(new Pen(ShareData.SecondaryColor, ShareData.middleThick), new PointF(PtXStart, PtY), new PointF(PtXend, PtY));


            int count = (int)(Width / 25f);
            float step = (float)Width / (float)count;

            if(Frame.Component.KeyTimes == null)
            {
                return new PointF();
            }
            else if(Frame.Component.KeyTimes.Count == 0)
            {
                return new PointF();
            }
            float startTime = (float)Frame.Component.KeyTimes[0];
            float timeStep = ((float)Frame.Component.MaxTime - (float)Frame.Component.FrameTimeLast) / (float)count;

            //draw keytime
            for (int n = 0; n <= count; n++)
            {
                PointF loc = new PointF(PtXStart + n * step, PtY);
                PointF textLoc = new PointF(loc.X - 8, loc.Y + 2);
                string tmStr = (startTime + n * timeStep).ToString("f1");
                graphics.DrawString(tmStr, GH_FontServer.NewFont(GH_FontServer.Script, 5, FontStyle.Regular), new SolidBrush(ShareData.SecondaryColor), textLoc);

                PointF keyPt = new PointF(loc.X - ShareData.KeyTimeRadius / 2, loc.Y - ShareData.KeyTimeRadius / 2);
                try
                {
                    graphics.DrawEllipse(new Pen(ShareData.SecondaryColor, ShareData.KeyTimeRadius), keyPt.X, keyPt.Y, ShareData.KeyTimeRadius, ShareData.KeyTimeRadius);
                }
                catch
                {

                }

            }

            graphics.DrawString("s", GH_FontServer.NewFont(GH_FontServer.Script, 5, FontStyle.Regular), new SolidBrush(ShareData.SecondaryColor),
                new PointF(PtXend + 5, PtY + 2));

            //draw right time
            string txtT = Frame.Component.RightTime.ToString("f2") + "s";
            PointF Cloc = new PointF((float)((Frame.Component.RightTime - Frame.Component.KeyTimes[0]) / (Frame.Component.MaxTime - Frame.Component.FrameTimeLast)) * Width + PtXStart, PtY);

            PointF Ptloc = new PointF(Cloc.X - ShareData.KeyPtRadius / 2, Cloc.Y - ShareData.KeyPtRadius / 2);
            graphics.DrawEllipse(new Pen(ShareData.HighLightColor, ShareData.KeyPtRadius), Ptloc.X, Ptloc.Y, ShareData.KeyPtRadius, ShareData.KeyPtRadius);

            PointF txtLoc = new PointF(Cloc.X - 12, Cloc.Y + 5);
            graphics.DrawString(txtT, GH_FontServer.NewFont(GH_FontServer.Script, 7, FontStyle.Bold), new SolidBrush(ShareData.HighLightColor), txtLoc);

            return Cloc;
        }

        protected static PointF DrawGraphCurve(Graphics graphics, Dictionary<int, double> dict, float PtXStart, float PtY, int height, List<Event> mapperNum, out List<RectangleF> OutList, out RectangleF MaxRext)
        {

            float Width = Frame.Component.SliderWidth - 20;
            float PtXend = PtXStart + Width;
            PtY -= ShareData.GraphCurveThickness;

            MaxRext = new RectangleF(PtXStart, PtY - height, Width, height);

            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (double value in dict.Values)
            {
                min = min < value ? min : value;
                max = max > value ? max : value;
            }

            if (max - min < 1E-12)
                max += 1E-12;

            List<RectangleF> relayDict = new List<RectangleF>();
            float rectY = PtY - height;
            foreach (Event map in mapperNum)
            {
                float rectX = (float)((map.timedomain.T0 - Frame.Component.KeyTimes[0]) / (Frame.Component.MaxTime - Frame.Component.KeyTimes[0]) * (double)Width) + PtXStart;
                float rectWidth = (float)((map.timedomain.T1 - map.timedomain.T0) / (Frame.Component.MaxTime - Frame.Component.KeyTimes[0]) * (double)Width);

                RectangleF sectRect = new RectangleF(rectX, rectY, rectWidth, height);
                relayDict.Add(sectRect);

                
                graphics.DrawRectangle(new Pen(new SolidBrush(ShareData.QuarterThemeColor), 3), rectX, rectY, rectWidth, height);
                graphics.FillRectangle(new SolidBrush(ShareData.QuarterThemeColor), sectRect);

                graphics.DrawString(map.NickName, GH_FontServer.NewFont(GH_FontServer.Script, 6, FontStyle.Bold), new SolidBrush(ShareData.HalfThemeColor), new PointF(rectX + 2, rectY + 3));

            }
            OutList = relayDict;

            List<PointF> pts = new List<PointF>();

            foreach (KeyValuePair<int, double> row in dict)
            {
                if (row.Key > Frame.Component.MaxFrame)
                    continue;
                float xPt = (float)row.Key / (float)Frame.Component.MaxFrame * Width + PtXStart;
                float yPt = (float)(PtY - (row.Value - min) / (max - min) * height);
                pts.Add(new PointF(xPt, yPt));
            }
            if (pts.Count == 1)
                return new PointF();

            //DrawCurve
            pts.Sort((m, n) => m.X.CompareTo(n.X));
            graphics.DrawLines(new Pen(ShareData.ThemeColor, ShareData.GraphCurveThickness), pts.ToArray());



            float HptX = (float)Frame.Component.RightFrame / (float)Frame.Component.MaxFrame * Width + PtXStart;
            float HptY = (float)(PtY - (dict[Frame.Component.RightFrame] - min) / (max - min) * height);
            graphics.DrawEllipse(new Pen(ShareData.HighLightColor, ShareData.KeyPtRadius), HptX - ShareData.KeyPtRadius / 2, HptY - ShareData.KeyPtRadius / 2, ShareData.KeyPtRadius, ShareData.KeyPtRadius);

            string text = dict[Frame.Component.RightFrame].ToString("f2");
            PointF txtLoc = new PointF(HptX - 15, HptY - 18);
            graphics.DrawString(text, GH_FontServer.NewFont(GH_FontServer.Script, 9, FontStyle.Bold), new SolidBrush(ShareData.HighLightColor), txtLoc);

            return new PointF(HptX, HptY);
        }

        public static void DrawGraph(Graphics graphics, Dictionary<int, double> dict, float PtXStart, float PtY, int height, List<Event> mapperNum, out List<RectangleF> outDict, out RectangleF maxRect)
        {
            PointF pt1 = DrawTimeLine(graphics, PtXStart, PtY);
            outDict = new List<RectangleF>();
            maxRect = RectangleF.Empty;

            if (dict.Count > 1)
            {
                PointF pt2 = DrawGraphCurve(graphics, dict, PtXStart, PtY, height, mapperNum, out outDict, out maxRect);
                graphics.DrawLine(new Pen(ShareData.HighLightColor, ShareData.middleThick), pt1, pt2);
            }
            else
            {
                PointF TxtLoc = new PointF(PtXStart, PtY - height);
                graphics.DrawString("Please drag the Frame Slider!", GH_FontServer.NewFont(GH_FontServer.Script, 9, FontStyle.Bold), new SolidBrush(ShareData.ThemeColor), TxtLoc);
            }
        }
    }

    class BasicFunction
    {
        public static bool ConnectToFrame(GH_Component com, out Animation.Frame frameComponent)
        {
            if (Animation.Frame.Component == null || Animation.Frame.Component.OnPingDocument() != com.OnPingDocument())
            {
                com.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please Set Frame Component First!");
                frameComponent = null;
                return false;
            }
            else if (Animation.Frame.Component.Params.Input[0].SourceCount == 0)
            {
                com.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please Finish Frame Component!");
                frameComponent = null;
                return false;
            }
            else
            {
                frameComponent = Animation.Frame.Component;
                return true;
            }
        }

        public static double RemapByType(double t, int type, GH_Component com)
        {
            switch (type)
            {
                case 0:
                    return t;
                case 1:
                    double domainForSigmoid = 6;
                    double realSig = 1 / (1 + Math.Pow(Math.E, domainForSigmoid - 2 * domainForSigmoid * t));
                    return  (realSig - 0.5) / (1 - 2 / (1 + Math.Pow(Math.E, domainForSigmoid))) + 0.5;
                case 2:
                    return 0;
                case 3:
                    return 1;
                default:
                    com.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input Type must be in 0-3!");
                    return t;
            }
        }
    }

    internal class GetControl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetDC(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
    }
}
