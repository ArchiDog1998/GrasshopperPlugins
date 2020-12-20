using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;

using Rhino.Geometry;
using Rhino.Display;
using GH_IO.Serialization;
using System.Linq;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System.Diagnostics;
using static Rhino.RhinoApp;

namespace Whale.Animation
{
    public class Frame : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the FrameComponent class.
        /// </summary>
        public Frame()
          : base("Frame", "Frame",
              "Frame",
              "Whale", "Animation")
        {
            Component = this;

            Save = false;
            DelayLog = 2;

            FPSindex = 4;

            Sizeindex = 16;
            PictWidth = 2560;
            PictHeight = 1440;

            Typeindex = 0;

            Duration = false;
            Remap = true;
            TimeInFrame = 0.5;

            SliderMultiple = 50;
            ViewportReduce = 3;
            ShowLabel = true;
            LabelSize = 40;
            ShowFrame = true;
            ShowTime = true;
            ShowPercent = true;
            ShowRemain = true;
            ShowGraph = true;
            ShowGraphOnEvent = true;

            IsInputASlider = false;
        }

        public static Frame Component = null;

        //playSet
        public bool Play = false;
        private bool shoot = false;
        public bool Save ;
        public string FilePath = "";
        public double DelayLog;

        //FPS
        public List<int> FPSitems = new List<int> { 16, 18, 24, 25, 30, 48, 50, 60,100,120, 144, 240, 300 };
        public int FPSindex;

        //Size
        public List<string> SizeItems = new List<string> {
            "192 x 144",
            "320 x 240",
            "480 x 360",
            "720 x 480",
            "720 x 486",
            "720 x 576",
            "1280 x 720",
            "1280 x 1080",
            "1828 x 1332",
            "1828 x 1556",
            "1920 x 1080",
            "1998 x 1080",
            "2048 x 858",
            "2048 x 1080",
            "2048 x 1152",
            "2048 x 1556",
            "2560 x 1440",
            "3840 x 2160",
            "7680 x 4320"
        };
        public int Sizeindex;
        public int PictWidth;
        public int PictHeight;

        //Type
        public List<System.Drawing.Imaging.ImageFormat> PictTypeItems = new List<System.Drawing.Imaging.ImageFormat>
        {
            System.Drawing.Imaging.ImageFormat.Jpeg,
            System.Drawing.Imaging.ImageFormat.Png,
            System.Drawing.Imaging.ImageFormat.Bmp,
            System.Drawing.Imaging.ImageFormat.Gif,
            System.Drawing.Imaging.ImageFormat.Tiff,
        };
        public int Typeindex;

        //TimeLine
        public bool Duration;
        public bool Remap;
        public double TimeInFrame;

        //Show
        public double SliderMultiple;
        public double ViewportReduce;
        public float SliderWidth;
        public bool ShowLabel;
        public double LabelSize;
        public bool ShowFrame;
        public bool ShowTime;
        public bool ShowPercent;
        public bool ShowRemain;
        public bool ShowGraph;
        public bool ShowGraphOnEvent;

        //ComputeThing
        public RhinoView RhinoView;
        public int MaxFrame;
        public double MaxTime;
        public double FrameTimeLast;

        public List<double> KeyTimes { get; set; }
        public List<Interval> TimeDomain;
        public bool IsInputASlider;

        //CompareData
        List<double> LastInputNumbers = new List<double>();
        int StartFrame;
        int AutoFrame;
        Stopwatch watchtime = null;
        

        //wireless IOdata
        public List<GroupLocker> ToLockList = new List<GroupLocker>();
        public List<EventOperation> ClearComponent = new List<EventOperation>();
        public IGH_DocumentObject FrameObject = null;

        //Time
        public int RightFrame;
        public double RightTime;
        public string PercentStr;
        public string RemainStr;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public class FrameComponentAttributes : GH_ComponentAttributes
        {
            public new Frame Owner;
            List<List<RectangleF>> ToEventButton = new List<List<RectangleF>>();
            List<RectangleF> GoButtons;
            RectangleF maxRect = new RectangleF();

            RectangleF baseRect;
            RectangleF thisRect;

            RectangleF PlayButtonRect;
            RectangleF ShootButtonRect;

            public FrameComponentAttributes(Frame owner) : base(owner)
            {
                Owner = owner;
            }

            protected override void Layout()
            {
                base.Layout();
                float width = 6;
                
                PlayButtonRect = new RectangleF(Owner.Attributes.Bounds.X, Owner.Attributes.Bounds.Y + Owner.Attributes.Bounds.Height + 5f,
                    Owner.Attributes.Bounds.Width / 2 - width / 2, 20);
                ShootButtonRect = new RectangleF(Owner.Attributes.Bounds.X + Owner.Attributes.Bounds.Width / 2 + width / 2, Owner.Attributes.Bounds.Y + Owner.Attributes.Bounds.Height + 5f, 
                    Owner.Attributes.Bounds.Width / 2 - width / 2, 20);
                baseRect = this.Bounds;
                thisRect = new RectangleF(this.baseRect.X, this.baseRect.Y, this.baseRect.Width, this.baseRect.Height + 30);
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                Bounds = baseRect;
                base.Render(canvas, graphics, channel);
                

                //Show Top Text
                if (channel == GH_CanvasChannel.Overlay && Owner.ShowLabel)
                {
                    float size = (float)Owner.LabelSize / Grasshopper.Instances.ActiveCanvas.Viewport.Zoom;
                    string showMessage = (Owner.ShowFrame ? "Frame:" + Owner.RightFrame.ToString() + "  " : "") + (Owner.ShowTime ? "Time:" + Owner.RightTime.ToString("f2") + "s  " : "")
                        + (Owner.ShowPercent ? Owner.PercentStr : "") + (Owner.ShowRemain ? Owner.RemainStr : "");
                    graphics.DrawString(showMessage, GH_FontServer.NewFont(GH_FontServer.Script, size, FontStyle.Regular),
                        new SolidBrush(ShareData.ThemeColor), Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion.X, Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion.Y);
                }

                //Render Button
                if (channel == GH_CanvasChannel.Objects)
                {
                    GH_Palette pale = Owner.Locked ? GH_Palette.Locked : GH_Palette.Normal;

                    GH_Capsule PlayCapsule = GH_Capsule.CreateTextCapsule(PlayButtonRect, PlayButtonRect, pale, "Play");
                    if (Owner.Play)
                        PlayCapsule.Render(graphics, ShareData.ThemeColor);
                    else
                        PlayCapsule.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                    PlayCapsule.Dispose();

                    GH_Capsule ShootCapsule = GH_Capsule.CreateTextCapsule(ShootButtonRect, ShootButtonRect, pale, "Shoot");
                    if (Owner.shoot)
                        ShootCapsule.Render(graphics, ShareData.ThemeColor);
                    else
                        ShootCapsule.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                    ShootCapsule.Dispose();

                    if (Owner.ShowGraph && Owner.FrameObject != null)
                    {
                        if(Owner.FrameObject.OnPingDocument() == Owner.OnPingDocument())
                        {
                            float PtXStart;
                            float PtY;
                            GoButtons = new List<RectangleF>();
                            ToEventButton = new List<List<RectangleF>>();

                            Frame.Component = Frame.Component ?? Owner;

                            PtXStart = Owner.Attributes.Bounds.X + Owner.Attributes.Bounds.Width / 2 - Frame.Component.SliderWidth / 2 + 10 + 40;
                            PtY = Owner.Attributes.Bounds.Y - 20;

                            foreach (EventOperation eveOperation in Owner.ClearComponent)
                            {
                                if (!eveOperation.ShowGraphOnSlider)
                                    continue;

                                List<RectangleF> relayDict = new List<RectangleF>();
                                RenderCanvas.DrawGraph(graphics, eveOperation.keyPoints, PtXStart, PtY, eveOperation.GraphHeight, eveOperation.eventThing, out relayDict, out maxRect);
                                ToEventButton.Add(relayDict);


                                RectangleF buttonRect = new RectangleF(PtXStart - 80, PtY - eveOperation.GraphHeight - ShareData.GraphCurveThickness,
                                    60, eveOperation.GraphHeight + ShareData.GraphCurveThickness);

                                GoButtons.Add(buttonRect);

                                if (this.maxRect != new RectangleF())
                                    maxRect = RectangleF.Union(maxRect, buttonRect);
                                PtY -= ShareData.GraphDistance + eveOperation.GraphHeight;

                                //Split String
                                int maxlen = (int)((float)buttonRect.Width / 3.75f);
                                string[] listArray = eveOperation.NickName.Split(' ');
                                string showTxt = listArray[0];
                                string joinStr = "";
                                for (int n = 1; n < listArray.Length; n++)
                                {
                                    if (listArray[n].Length > maxlen)
                                    {
                                        if (joinStr.Length > 0)
                                            showTxt += "\n" + joinStr;
                                        showTxt += "\n" + listArray[n];

                                        joinStr = "";
                                    }
                                    else if (listArray[n].Length + joinStr.Length > maxlen)
                                    {
                                        showTxt += "\n" + joinStr;
                                        joinStr = listArray[n];
                                    }
                                    else
                                    {
                                        joinStr += " " + listArray[n];
                                    }
                                }
                                if (joinStr.Length > 0)
                                    showTxt += "\n" + joinStr;

                                int deep = (int)((float)buttonRect.Height / 3);
                                GH_Capsule gH_Capsule = GH_Capsule.CreateTextCapsule(buttonRect, buttonRect, GH_Palette.Blue, showTxt,
                                    GH_FontServer.NewFont(GH_FontServer.Script, 6, FontStyle.Regular), GH_Orientation.horizontal_center, 5, deep);
                                gH_Capsule.Render(graphics, ShareData.ThemeColor);
                                gH_Capsule.Dispose();
                            }

                        }

                        if (this.maxRect != new RectangleF())
                            thisRect = RectangleF.Union(new RectangleF(this.baseRect.X, this.baseRect.Y, this.baseRect.Width, this.baseRect.Height + 30), this.maxRect);
                        else
                            thisRect = new RectangleF(this.baseRect.X, this.baseRect.Y, this.baseRect.Width, this.baseRect.Height + 30);
                    }

                }

                if (channel == GH_CanvasChannel.First && Owner.FrameObject != null)
                {

                    Rectangle rect = GH_Convert.ToRectangle(Owner.FrameObject.Attributes.Bounds);
                    rect.Inflate(ShareData.FrameWidth, ShareData.FrameWidth);
                    GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(rect, GH_Palette.Pink, ShareData.FrameWidth, 5);
                    gH_Capsule.Render(graphics, ShareData.ThemeColor);
                    gH_Capsule.Dispose();

                }

                this.Bounds = this.thisRect;
            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                this.Bounds = baseRect;
                if (Owner.Attributes.Bounds.Contains(e.CanvasLocation)&&e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    AdvancedOptions windowForm = new AdvancedOptions(Owner);

                    WindowInteropHelper ownerHelper = new WindowInteropHelper(windowForm);

                    ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;

                    windowForm.Show();
                    return GH_ObjectResponse.Release;

                }

                GH_ObjectResponse result = base.RespondToMouseDoubleClick(sender, e);
                this.Bounds = thisRect;
                return result;
            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                //this.Bounds = thisRect;

                if (ShootButtonRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.shoot = true;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseDown(sender, e);
                this.Bounds = thisRect;
                return result;
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                this.Bounds = thisRect;

                if (ShootButtonRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.shoot = false;
                    Owner.Caputre("s");
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Release;
                }
                else if (PlayButtonRect.Contains(e.CanvasLocation))
                {
                    Owner.Play = !Owner.Play;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Release;
                }
                

                for(int n = 0;n < GoButtons.Count; n++)
                {
                    if (GoButtons[n].Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                        Owner.ClearComponent[n].GoComponent(Owner.ClearComponent[n]);
                    else
                    {
                        for(int m = 0; m < ToEventButton[n].Count; m++)
                        {
                            if (ToEventButton[n][m].Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                                Owner.ClearComponent[n].GoComponent(Owner.ClearComponent[n].eventThing[m]);
                        }
                    }
                }

                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseUp(sender, e);
                this.Bounds = thisRect;
                return result;
            }

            public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseMove(sender, e);
                this.Bounds = thisRect;
                return result;
            }
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new FrameComponentAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Current Frame", "Frame", "Slider or VrayTimeLine's Current Frame!", GH_ParamAccess.item);
            Param_FilePath param_FilePath = new Param_FilePath();
            pManager.AddParameter(param_FilePath, "Work File Path", "FilePath", "FileLocation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Key Time1", "Time1", "KeyTime1", GH_ParamAccess.item, 1);

            pManager[1].Optional = true;
            this.Hidden = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("OutPutResolution", "Res", "OutputResolution To Vray Component", GH_ParamAccess.list);
            pManager.AddNumberParameter("CurrentTime", "Time", "Current Time", GH_ParamAccess.item);
            pManager.AddIntervalParameter("TimeDomain1", "TimeD1", "TimeDomain1", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {


            #region TimeLine
            List<double> inputNumbers = new List<double> { 0 };
            for (int i = 2; i < this.Params.Input.Count; i++)
            {
                double ramNumber = 0;
                DA.GetData(i, ref ramNumber);
                inputNumbers.Add(ramNumber);
            }
            if (!LastInputNumbers.SequenceEqual(inputNumbers))
                ComputeInterval(inputNumbers);
            for (int n = 0; n < TimeDomain.Count; n++)
                DA.SetData(n + 2,TimeDomain[n]);
            #endregion

            int inputFrame = FindFrameObject(DA);
            MaintainViewPort(PictWidth, PictHeight, this.ViewportReduce);

            if(IsInputASlider && !DA.GetData("Work File Path", ref FilePath))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please Input a File Path!");
                return;
            }

            AutoFrameCompute(inputFrame);

            //SetInformation
            DA.SetDataList("OutPutResolution", new List<int> { this.PictWidth, this.PictHeight });
            DA.SetData("CurrentTime", this.RightTime);
            //FFMpegSharp.FFMPEG.FFMpeg fF = new FFMpegSharp.FFMPEG.FFMpeg();
            //fF.JoinImageSequence(new System.IO.FileInfo(), FPSitems[FPSindex], new FFMpegSharp.ImageInfo);
        }
        protected void ComputeInterval(List<double> inputNumbers)
        {
            List<double> keyTime = new List<double>();
            if (Duration)
                for (int n = 1; n <= inputNumbers.Count; n++)
                {
                    double sum = 0;
                    for (int index = 0; index < n; index++)
                        sum += inputNumbers[index];
                    keyTime.Add(sum);
                }
            else
            {
                for (int j = 0; j < inputNumbers.Count - 1; j++)
                {
                    if (inputNumbers[j] > inputNumbers[j + 1] + 0.0000000000001)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The input time does not grow from small to large Input sequentially!");
                        inputNumbers.Sort();
                    }
                }
                keyTime = inputNumbers;
            }

            this.MaxTime = keyTime[keyTime.Count - 1];
            this.MaxFrame = (int)(keyTime[keyTime.Count - 1] * (double)this.FPSitems[FPSindex]) - 1;
            this.FrameTimeLast = 1.0 / (double)this.FPSitems[FPSindex];
            this.KeyTimes = new List<double>();

            for (int frame = 0; frame <= this.MaxFrame; frame++)
            {
                double start = (double)frame * FrameTimeLast;
                this.KeyTimes.Add(start + this.TimeInFrame * FrameTimeLast);
            }

            this.TimeDomain = new List<Interval>();
            for (int n = 0; n < keyTime.Count - 1; n++)
            {
                double startNum;
                double endNum;

                if (this.Remap)
                {
                    List<double> copyTimes = new List<double>();
                    foreach (double num in KeyTimes)
                        copyTimes.Add(num);


                    copyTimes.Sort((x, y) => Math.Abs(x - keyTime[n]).CompareTo(Math.Abs(y - keyTime[n])));
                    if (Math.Abs(this.TimeInFrame) < 1E-12 || Math.Abs(this.TimeInFrame - 1) < 1E-12 && keyTime[n] < 1E-12)
                        startNum = copyTimes[0];

                    else if (Math.Abs(this.TimeInFrame - 1) < 1E-12)
                        startNum = copyTimes[1] > keyTime[n] ? copyTimes[1] : copyTimes[2];

                    else
                        startNum = copyTimes[0] > keyTime[n] ? copyTimes[0] : copyTimes[1];

                    copyTimes.Sort((x, y) => Math.Abs(x - keyTime[n + 1]).CompareTo(Math.Abs(y - keyTime[n + 1])));
                    if (Math.Abs(this.TimeInFrame) < 1E-12 && Math.Abs(keyTime[n + 1] - MaxTime) < 1E-12 || Math.Abs(this.TimeInFrame - 1) < 1E-12)
                        endNum = copyTimes[0];

                    else if (Math.Abs(this.TimeInFrame) < 1E-12)
                        endNum = copyTimes[1] > keyTime[n + 1] ? copyTimes[2] : copyTimes[1];

                    else
                        endNum = copyTimes[0] > keyTime[n + 1] ? copyTimes[1] : copyTimes[0];

                    TimeDomain.Add(new Interval(startNum - 1E-12, endNum + 1E-12));
                }
                else
                {
                    TimeDomain.Add(new Interval(keyTime[n], keyTime[n + 1]));
                }
            }

        }

        protected void MaintainViewPort(int Width, int Height, double Reduce)
        {
            int width = (int)((float)Width / Reduce);
            int height = (int)((float)Height / Reduce);
            int x = Convert.ToInt32(Math.Round((double)Screen.PrimaryScreen.Bounds.Width - width));
            int y = 200;

            
            if (Rhino.RhinoDoc.ActiveDoc.Views.Find("Whale", true) == null)
            {
                RhinoView = Rhino.RhinoDoc.ActiveDoc.Views.Add("Whale", DefinedViewportProjection.Perspective, new Rectangle(x, y, width, height), true);
                RhinoView.Size = new Size(width, height);
                RhinoView.ActiveViewport.DisplayMode = DisplayModeDescription.GetDisplayMode(DisplayModeDescription.RenderedId);
                IntPtr parent = GetControl.GetParent(RhinoView.Handle);
                GetControl.SetWindowPos(parent, -1, 0, 0, 0, 0, 3);
            }
            else
            {
                RhinoView = Rhino.RhinoDoc.ActiveDoc.Views.Find("Whale", true);
                int viewWidth = RhinoView.Size.Width;
                Reduce = (float)Width / (float)viewWidth;
                RhinoView.Size = new Size(viewWidth, viewWidth * height/width);
                RhinoView.Redraw();
            }
        }

        protected int FindFrameObject(IGH_DataAccess DA)
        {
            if (this.Params.Input[0].SourceCount == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Please input a NumberSlider or a VrayTimeLine to Input Frame.");
                return int.MinValue;
            }

            FrameObject = this.Params.Input[0].Sources[0].Attributes.GetTopLevel.DocObject;
            if (FrameObject is GH_NumberSlider)
            {
                this.IsInputASlider = true;
                GH_NumberSlider frameSlider = FrameObject as GH_NumberSlider;

                if (frameSlider.Slider.Maximum != this.MaxFrame)
                    frameSlider.Slider.Maximum = this.MaxFrame;
                if (frameSlider.Slider.Minimum != 0)
                    frameSlider.Slider.Minimum = 0;
                frameSlider.Slider.Type = Grasshopper.GUI.Base.GH_SliderAccuracy.Integer;

                if (frameSlider.Slider.Value > frameSlider.Slider.Maximum)
                    frameSlider.Slider.Value = frameSlider.Slider.Maximum;
                else if (frameSlider.Slider.Value < frameSlider.Slider.Minimum)
                    frameSlider.Slider.Value = frameSlider.Slider.Minimum;

                frameSlider.NickName = "Frames";

                frameSlider.Expression = "X * " + this.FrameTimeLast.ToString() + " + " + (this.TimeInFrame * FrameTimeLast).ToString();

                this.SliderWidth = (float)(this.MaxTime * SliderMultiple) + 20;
                this.SliderWidth = SliderWidth > 120 ? SliderWidth : 120;

                float beforeWidth = frameSlider.Attributes.Bounds.Width;
                

                int SliderBigger = (int)SliderWidth - frameSlider.Slider.Bounds.Width;
                frameSlider.Attributes.Bounds = new RectangleF(frameSlider.Attributes.Bounds.X, frameSlider.Attributes.Bounds.Y,
                    frameSlider.Attributes.Bounds.Width + SliderBigger, frameSlider.Attributes.Bounds.Height);

                float afterWidth = frameSlider.Attributes.Bounds.Width;
                float X = frameSlider.Attributes.Pivot.X - (afterWidth - beforeWidth);
                frameSlider.Attributes.Pivot = new PointF(X, frameSlider.Attributes.Pivot.Y);

                frameSlider.OnPingDocument().DestroyAttributeCache();
                frameSlider.Attributes.ExpireLayout();
                frameSlider.Attributes.PerformLayout();

                this.Params.Input[1].Optional = false;

                return (int)frameSlider.Slider.Value;
            }
                
            else if (FrameObject.GetType().ToString() == "VRayForGrasshopper.VRayTimelineComponent")
            {
                this.IsInputASlider = false;
                GH_Component VrayComponent = FrameObject as GH_Component;
                SliderWidth = VrayComponent.Attributes.Bounds.Width - VrayComponent.Params.InputWidth - VrayComponent.Params.OutputWidth - 4;
                if ((VrayComponent.Params.Input[0].VolatileData.AllData(true).ElementAt(0) as GH_Integer).Value != this.MaxFrame)
                {
                    VrayComponent.Params.Input[0].VolatileData.Clear();
                    VrayComponent.Params.Input[0].AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, this.MaxFrame);
                    VrayComponent.ExpireSolution(true);
                }
                this.Params.Input[1].Optional = true;

                this.Save = false;

                int inputFrame = 0;
                DA.GetData("Current Frame", ref inputFrame);
                return inputFrame;
            }
                
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Only NumberSlider and VrayTimeLine are allowed for Input Frame!");
                FrameObject = null;
                return int.MinValue;
            }
                
            
        }

        protected void AutoFrameCompute(int inputFrame)
        {
            this.OnPingDocument().SolutionEnd -= ends;
            int Delay = (int)Math.Pow(10, DelayLog);
            this.PercentStr = "";
            this.RemainStr = "";

            GH_Document.GH_ScheduleDelegate callback = new GH_Document.GH_ScheduleDelegate(this.ScheduleCallback);
            if (this.Play)
            {
                if (GH_Document.IsEscapeKeyDown())
                {
                    this.Play = false;
                    this.ExpireSolution(true);
                    this.AutoFrame = -1;
                    this.RightFrame = inputFrame;
                }

                if (AutoFrame == -1)
                {
                    watchtime = Stopwatch.StartNew();
                    StartFrame = inputFrame;
                    this.AutoFrame = StartFrame;
                    this.RightFrame = AutoFrame;
                    this.OnPingDocument().ScheduleSolution(Delay, callback);
                    this.OnPingDocument().SolutionEnd += ends;

                }
                else
                {
                    this.RightFrame = AutoFrame;

                    if (this.AutoFrame <= this.MaxFrame)
                    {
                        double remain = ((watchtime.Elapsed.TotalSeconds) / (double)(AutoFrame - StartFrame + 1)) * (double)(this.MaxFrame - AutoFrame);
                        double percent = (double)(AutoFrame - StartFrame) / (double)(this.MaxFrame - StartFrame) * 100;
                        PercentStr = percent.ToString("f0") + "%  ";
                        RemainStr = remain.ToString("f2") + "s Left";
                        AutoFrame++;
                        this.OnPingDocument().ScheduleSolution(Delay, callback);
                        this.OnPingDocument().SolutionEnd += ends;
                    }
                    else
                    {
                        PercentStr = "Finished!  ";
                        RemainStr = "TotalTime: " + watchtime.Elapsed.TotalSeconds.ToString("f2") + "s";
                        watchtime = null;
                        this.Play = false;
                        this.AutoFrame = -1;
                        this.RightFrame = inputFrame;
                        this.OnPingDocument().SolutionEnd += ends;
                        
                    }
                }
            }
            else
            {

                this.RightFrame = inputFrame;
                AutoFrame = -1;
                watchtime = null;
                this.OnPingDocument().ScheduleSolution(0, callback);
            }

            this.RightTime = this.KeyTimes[this.RightFrame];
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            try
            {
                this.OnPingDocument().SolutionEnd -= ends;
            }
            catch
            {

            }


            base.RemovedFromDocument(document);
        }

        private void ends(object sender, GH_SolutionEventArgs e)
        {
            if (this.Save)
                Caputre("");

        }

        private void ScheduleCallback(GH_Document doc)
        {

            foreach (var a in ToLockList)
                a.ToLockIt(this.RightTime);

            if (Play)
                this.ExpireSolution(false);
        }

        protected void Caputre(string suffix)
        {
            Bitmap bitmap = RhinoView.CaptureToBitmap(new Size(this.PictWidth, this.PictHeight), false, false, false);
            string numberofMaxFrame = "D" + ((int)this.MaxFrame).ToString().Length.ToString();
            if (bitmap != null)
            {
                string end = PictTypeItems[Typeindex].ToString().ToLower();
                string filePath = FilePath + "Whale  " + (this.RightFrame).ToString(numberofMaxFrame) + suffix + "." + end;
                bitmap.Save(filePath, PictTypeItems[Typeindex]);
                
            }
            bitmap.Dispose();
        }

        //protected string ComponentInformation()
        //{
        //    string combiStr = "\n-----------------------\n";

        //    string fpsStr = "FPS: " + FPSitems[FPSindex].ToString();
        //    string sizeStr = "Size: " + PictWidth.ToString() + " x " + PictHeight.ToString();
        //    string typeStr = "Type: " + PictTypeItems[Typeindex].ToString();
        //    string formatStr = fpsStr + "\n" + sizeStr + "\n" + typeStr ;

        //    string maxTimeStr = "MaxTime: " + this.MaxTime.ToString("f2") + "s";
        //    string maxFrameStr = "MaxFrame: " + this.MaxFrame.ToString();
        //    string timeLineStr = maxFrameStr + "\n" + maxTimeStr;

        //    string rightFrameStr = "RightFrame: " + this.RightFrame.ToString();
        //    string rightTimeStr = "RightTime: " + this.RightTime.ToString("f2") + "s";
        //    string delayStr = "Delay: " + (Math.Pow(10, DelayLog) / 1000).ToString("f2") + "s";
        //    string saveStr = "Save: " + this.Save.ToString();
        //    string timeStr = rightFrameStr + "\n" + rightTimeStr + "\n" + saveStr + "\n" + delayStr;

        //    return timeStr + combiStr + timeLineStr + combiStr + formatStr;
        //}

        public void ResetSetting()
        {
            Component = this;

            Save = false;
            DelayLog = 2;

            FPSindex = 4;

            Sizeindex = 16;
            PictWidth = 2560;
            PictHeight = 1440;

            Typeindex = 0;

            Duration = false;
            Remap = true;
            TimeInFrame = 0.5;

            SliderMultiple = 50;
            ViewportReduce = 3;
            ShowLabel = true;
            LabelSize = 40;
            ShowFrame = true;
            ShowTime = true;
            ShowPercent = true;
            ShowRemain = true;
            ShowGraph = true;
            ShowGraphOnEvent = true;

            IsInputASlider = false;
        }

        public void ClearGraphData()
        {
            foreach(EventOperation com in ClearComponent)
            {
                com.keyPoints = new Dictionary<int, double>();
            }
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && index > 1) return true;
            return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Input && index > 2) return true;
            return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_Number val = new Param_Number();
            val.SetPersistentData(new object[1] { index - 1 });
            return val;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (int i = 2; i < this.Params.Input.Count; i++)
            {
                IGH_Param val = this.Params.Input[i];
                if (Duration)
                {
                    val.Name = "Time Duration" + (i - 1).ToString();
                    val.NickName = "TimeD" + (i - 1).ToString();
                    val.Description = "Time Duration" + (i - 1).ToString();
                }
                else
                {
                    val.Name = "Key Time" + (i - 1).ToString();
                    val.NickName = "TimeK" + (i - 1).ToString();
                    val.Description = "KeyTime" + (i - 1).ToString();
                }
                val.Access = GH_ParamAccess.item;
                val.MutableNickName = false;
            }

            while (this.Params.Input.Count > this.Params.Output.Count)
            {
                IGH_Param val = (IGH_Param)new Param_Interval();
                val.Name = "TimeDomain" + (Params.Output.Count-1).ToString();
                val.NickName = "TimeD" + (Params.Output.Count - 1).ToString();
                val.Description = "TimeDomain" + (Params.Output.Count - 1).ToString();
                val.Access = GH_ParamAccess.item;
                this.Params.RegisterOutputParam(val);
                val.Optional = true;
            }

            while (this.Params.Input.Count < this.Params.Output.Count)
            {
                this.Params.UnregisterOutputParameter(this.Params.Output[this.Params.Output.Count - 1]);
            }
            this.Params.OnParametersChanged();
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.FrameToTime;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ca3e7f77-5335-4bff-8168-d857210f967a"); }
        }

        public override void RegisterRemoteIDs(GH_GuidTable table)
        {
            Frame.Component = null;
            base.RegisterRemoteIDs(table);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("FPSIndex", FPSindex);

            writer.SetInt32("SizeWidth", PictWidth);
            writer.SetInt32("SizeHeight", PictHeight);
            writer.SetInt32("SizeIndex", Sizeindex);

            writer.SetInt32("TypeIndex", Typeindex);

            writer.SetBoolean("Duration", Duration);
            writer.SetBoolean("Remap", Remap);
            writer.SetDouble("TimeInFrame", TimeInFrame);

            writer.SetDouble("DelayLog", DelayLog);
            writer.SetBoolean("Save", Save);

            writer.SetDouble("SliderMul", SliderMultiple);
            writer.SetDouble("ViewportReduce", ViewportReduce);
            writer.SetBoolean("ShowLabel", ShowLabel);
            writer.SetDouble("LabelSize", LabelSize);
            writer.SetBoolean("ShowFrame", ShowFrame);
            writer.SetBoolean("ShowTime", ShowTime);
            writer.SetBoolean("ShowPercent", ShowPercent);
            writer.SetBoolean("ShowRemain", ShowRemain);
            writer.SetBoolean("ShowGraph", ShowGraph);
            writer.SetBoolean("ShowGraphEvent", ShowGraphOnEvent);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            reader.TryGetInt32("FPSIndex", ref FPSindex);

            reader.TryGetInt32("SizeWidth", ref PictWidth);
            reader.TryGetInt32("SizeHeight", ref PictHeight);
            reader.TryGetInt32("SizeIndex", ref Sizeindex);

            reader.TryGetInt32("TypeIndex", ref Typeindex);

            reader.TryGetBoolean("Duration", ref Duration);
            reader.TryGetBoolean("Remap",ref Remap);
            reader.TryGetDouble("TimeInFrame", ref TimeInFrame);

            reader.TryGetDouble("DelayLog", ref DelayLog);
            reader.TryGetBoolean("Save", ref Save);

            reader.TryGetDouble("SliderMul", ref SliderMultiple);
            reader.TryGetDouble("ViewportReduce", ref ViewportReduce);
            reader.TryGetBoolean("ShowLabel", ref ShowLabel);
            reader.TryGetDouble("LabelSize", ref LabelSize);
            reader.TryGetBoolean("ShowFrame", ref ShowFrame);
            reader.TryGetBoolean("ShowTime", ref ShowTime);
            reader.TryGetBoolean("ShowPercent", ref ShowPercent);
            reader.TryGetBoolean("ShowRemain", ref ShowRemain);
            reader.TryGetBoolean("ShowGraph", ref ShowGraph);
            reader.TryGetBoolean("ShowGraphEvent", ref ShowGraphOnEvent);

            this.Play = false;
            return base.Read(reader);
        }
    }
}