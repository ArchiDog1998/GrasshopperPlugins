using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using GH_IO.Serialization;

namespace Whale.Animation
{
    public class EventOperation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EventOperation class.
        /// </summary>
        public EventOperation()
          : base("EventOperation", "Event",
              "EventOperation",
              "Whale", "Animation")
        {
            SetValue("ShowGraphOnThis", true);
        }

        public Dictionary<int, double> keyPoints = new Dictionary<int, double>();
        public bool ShowGraphOnSlider = true;
        public int GraphHeight;
      
        public List<Event> eventThing = new List<Event>();
        double showValue;
        GH_Component showGraphics = null;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public class EventOperationAttributes : GH_ComponentAttributes
        {
            public new EventOperation Owner;
            List<RectangleF> finalDict = new List<RectangleF>();
            RectangleF maxRect = new RectangleF();

            RectangleF baseRect;
            RectangleF thisRect;

            //GH_Canvas canvas;

            public EventOperationAttributes(EventOperation owner) : base(owner)
            {
                Owner = owner;
            }

            protected override void Layout()
            {
                base.Layout();
                baseRect = this.Bounds;
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                this.Bounds = baseRect;

                base.Render(canvas, graphics, channel);

                if (channel == GH_CanvasChannel.First && Owner.showGraphics != null)
                    if (Owner.showGraphics.OnPingDocument() == Owner.OnPingDocument())
                        RenderCanvas.HighLightCom(graphics, Owner.showGraphics, 2);

                
                if (channel == GH_CanvasChannel.Objects && Frame.Component != null && Owner.Params.Input[0].SourceCount > 0)
                {
                    finalDict = new List<RectangleF>();

                    if (Owner.GetValue("ShowGraphOnThis", @default: true) && Frame.Component.ShowGraphOnEvent)
                    {
                        float X = Owner.Attributes.Bounds.X + Owner.Attributes.Bounds.Width / 2 - Frame.Component.SliderWidth / 2+10;
                        float Y = Owner.Attributes.Bounds.Y - 20;

                        RenderCanvas.DrawGraph(graphics, Owner.keyPoints, X, Y, Owner.GraphHeight, Owner.eventThing, out finalDict, out maxRect);


                        PointF txtLoc = new PointF(X, Y - Owner.GraphHeight - 25);
                        graphics.DrawString(Owner.NickName, GH_FontServer.NewFont(GH_FontServer.Script, 9, FontStyle.Bold), new SolidBrush(ShareData.ThemeColor), txtLoc);
                    }
                    else
                    {
                        PointF txtLoc = new PointF(Owner.Attributes.Bounds.X, Owner.Attributes.Bounds.Y - 15);
                        graphics.DrawString(Owner.NickName, GH_FontServer.NewFont(GH_FontServer.Script, 9, FontStyle.Bold), new SolidBrush(ShareData.ThemeColor), txtLoc);

                    }
                }
                if (maxRect != new RectangleF())
                    thisRect = RectangleF.Union(baseRect, maxRect);
                else thisRect = baseRect;
                this.Bounds = thisRect;
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {

                for (int n = 0;n < Owner.eventThing.Count; n++)
                    if (e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f && (finalDict[n]).Contains(e.CanvasLocation))
                        Owner.GoComponent(Owner.eventThing[n]);
     
                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseUp(sender, e);
                this.Bounds = thisRect;
                return result;

            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f && Owner.Attributes.Bounds.Contains(e.CanvasLocation))
                {
                    if (Frame.Component.FrameObject != null)
                    {
                        Owner.GoComponent(Frame.Component);
                        return GH_ObjectResponse.Release;
                    }
                        

                }

                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseDoubleClick(sender, e);
                this.Bounds = thisRect;
                return result;

            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                this.Bounds = baseRect;
                GH_ObjectResponse result = base.RespondToMouseDown(sender, e);
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
            base.m_attributes = (IGH_Attributes)(object)new EventOperationAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Events", "EV", "Events", GH_ParamAccess.list);
            pManager.AddIntegerParameter("GraphHeight", "H", "GraphHeight,which must be larger than 0.", GH_ParamAccess.item, 40);
            pManager.AddIntegerParameter("BlendType", "T", "BlendType, 0=Line, 1=Sigmoid, 2=Start, 3=End", GH_ParamAccess.item, 1);

            this.Hidden = true;
            pManager[0].DataMapping = GH_DataMapping.Flatten;

            Param_Integer var = pManager[2] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("Line", 0);
                var.AddNamedValue("Sigmoid", 1);
                var.AddNamedValue("Start", 2);
                var.AddNamedValue("End", 3);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "outValue", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> valueList = new List<double>();
            int height = 0;
            int type = 0;

            DA.GetDataList("Events", valueList);
            DA.GetData("GraphHeight", ref height);
            DA.GetData("BlendType", ref type);

            Frame frameComponent;
            if (!BasicFunction.ConnectToFrame(this, out frameComponent))
                return;
            if (!Frame.Component.ClearComponent.Contains(this))
                Frame.Component.ClearComponent.Add(this);

            if (height < 1)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input GraphHeight must be larger than 0!");
                height = 1;
            }
            GraphHeight = height;

            keyPoints = new Dictionary<int, double>();
            CheckParam();
            AddDict(frameComponent.RightFrame);
            FixDict(type, frameComponent.MaxFrame);

            showValue = keyPoints[frameComponent.RightFrame];
            DA.SetData("Value", showValue);

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
                return Properties.Resources.TimeLine;
            }
        }



        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            GH_DocumentObject.Menu_AppendItem(menu, "ShowGraphOnThis", Menu_OutPutClicked, enabled: true, GetValue("ShowGraphOnThis", @default: true)).ToolTipText = "When checked, it will show the graph on this component.";
            GH_DocumentObject.Menu_AppendItem(menu, "ShowGraphOnSlider", Menu_OutPutClicked2, enabled: true, GetValue("ShowGraphOnSlider", @default: true)).ToolTipText = "When checked, it will show the graph on slider component.";
            GH_DocumentObject.Menu_AppendItem(menu, "ClearRamData", Menu_OutPutClicked3, enabled: true, GetValue("ClearRamData", @default: false)).ToolTipText = "When checked, it will show the graph on slider component.";
        }

        private void Menu_OutPutClicked(object sender, EventArgs e)
        {
            if (GetValue("ShowGraphOnThis", @default: true))
            {
                RecordUndoEvent("ShowGraphOnThis");
            }
            else
            {
                RecordUndoEvent("ShowGraphOnThis");
            }
            SetValue("ShowGraphOnThis", !GetValue("ShowGraphOnThis", @default: true));
            ExpireSolution(recompute: true);
        }

        private void Menu_OutPutClicked2(object sender, EventArgs e)
        {
            if (GetValue("ShowGraphOnSlider", @default: true))
            {
                RecordUndoEvent("ShowGraphOnSlider");
                this.ShowGraphOnSlider = false;
            }
            else
            {
                RecordUndoEvent("ShowGraphOnSlider");
                this.ShowGraphOnSlider = true;
            }
            SetValue("ShowGraphOnSlider", !GetValue("ShowGraphOnSlider", @default: true));
            ExpireSolution(recompute: true);
        }

        private void Menu_OutPutClicked3(object sender, EventArgs e)
        {
            if (GetValue("ClearRamData", @default: false))
            {
                RecordUndoEvent("ClearRamData");

            }
            else
            {
                RecordUndoEvent("ClearRamData");
            }
            SetValue("ClearRamData", !GetValue("ClearRamData", @default: false));
            ExpireSolution(recompute: true);
        }

        protected override void ValuesChanged()
        {
            Message = showValue.ToString("f2");

            if (GetValue("ShowGraphOnThis", @default: true))
            {
                Message += "\nThis";
                if (GetValue("ShowGraphOnSlider", @default: true))
                    Message += ", ";
            }
            else
                Message += "\n";

            if (GetValue("ShowGraphOnSlider", @default: true))
            {
                Message += "Slider";
            }

            if (GetValue("ClearRamData", @default: false))
            {
                this.keyPoints = new Dictionary<int, double>();
                SetValue("ClearRamData", false);
            }

        }

        public void GoComponent(IGH_DocumentObject com)
        {
            PointF pivot = new PointF(com.Attributes.Pivot.X, com.Attributes.Pivot.Y);
            GH_NamedView view = new GH_NamedView("", pivot, 1.5f, GH_NamedViewType.center);

            foreach (IGH_DocumentObject ghCom in this.OnPingDocument().SelectedObjects())
            {
                ghCom.Attributes.Selected = false;
            }
            com.Attributes.Selected = true;

            view.SetToViewport(Grasshopper.Instances.ActiveCanvas, 500);
        }

        protected void CheckParam()
        {

            List<IGH_DocumentObject> objs = new List<IGH_DocumentObject>();
            foreach (var a in this.Params.Input[0].Sources)
                objs.Add(a.Attributes.GetTopLevel.DocObject);

            eventThing = new List<Event>();
            foreach (IGH_DocumentObject obj in objs)
            {
                Event eve = GetFirstParamNumber(new List<IGH_DocumentObject> { obj });
                if (eve == null)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something Wrong with Find Event!");
                }
                else
                {
                    eve.Mother = this;
                    eventThing.Add(eve);
                }
            }



        }

        protected Event GetFirstParamNumber(List<IGH_DocumentObject> objs)
        {
            while (true)
            {
                foreach (IGH_DocumentObject obj in objs)
                    if (obj is Event)
                        return (Event)obj;

                objs = GetUpStream(objs);
                if (objs.Count == 0)
                    return null;
            }
        }

        protected List<IGH_DocumentObject> GetUpStream(List<IGH_DocumentObject> objs)
        {
            List<IGH_DocumentObject> returnObjs = new List<IGH_DocumentObject>();
            foreach (IGH_DocumentObject obj in objs)
            {
                if (obj is IGH_Param)
                    foreach (var param in ((IGH_Param)obj).Sources)
                        returnObjs.Add(param.Attributes.GetTopLevel.DocObject);

                else if (obj is GH_Component || obj is GH_Cluster)
                    foreach (var param in ((GH_Component)obj).Params.Input)
                        foreach (var pa in param.Sources)
                            returnObjs.Add(pa.Attributes.GetTopLevel.DocObject);
            }
            return returnObjs;
        }

        protected void AddDict(int rightFrame)
        {
            for (int n = 0; n < eventThing.Count; n++)
            {
                for (int i = 0; i < this.Params.Input[0].Sources[n].VolatileData.DataCount; i++)
                {
                    double relayValue = (this.Params.Input[0].Sources[n].VolatileData.AllData(true).ElementAt(i) as GH_Number).Value;
                    double valueToIn = eventThing[n].valuedomain.T0 + relayValue * eventThing[n].valuedomain.Length;
                    keyPoints[eventThing[n].frames[i]] = valueToIn;
                }

                eventThing[n].Message = this.NickName + "  " + eventThing[n].NickName + "\n" +
                    eventThing[n].timedomain.T0.ToString("f1") + "s -- " + eventThing[n].timedomain.T1.ToString("f1") + "s";

                if (eventThing[n].frames.Contains(rightFrame))
                    showGraphics = eventThing[n];
            }
        }

        protected void FixDict(int type, int maxFrame)
        {
            //Dictionary<int, double> stopFrame = new Dictionary<int, double>();
            //bool flag = keyPoints.Keys.Contains(0);
            //for (int frame = 1; frame <= maxFrame + 1; frame++)
            //{
            //    if (flag && keyPoints.Keys.Contains(frame))
            //    {
            //        if (flag) stopFrame[frame - 1] = keyPoints[frame - 1];
            //        else stopFrame[frame] = keyPoints[frame];
            //    }
            //    flag = keyPoints.Keys.Contains(frame);
            //}


            for (int frame = 0; frame <= maxFrame; frame++)
            {
                if (keyPoints.Keys.Contains(frame))
                    continue;

                int leftframe = eventThing[0].StartFrame;
                int rightframe = eventThing[0].StartFrame;
                for (int i = 0; i < eventThing.Count; i++)
                {
                    try
                    {
                        if (eventThing[i].EndFrame <= frame && eventThing[i + 1].StartFrame >= frame)
                        {
                            leftframe = eventThing[i].EndFrame;
                            rightframe = eventThing[i + 1].StartFrame;
                            break;
                        }
                    }
                    catch
                    {
                        leftframe = eventThing[i].EndFrame;
                        rightframe = eventThing[i].EndFrame;
                    }
                }

                //double rightTime = Frame.Component.KeyTimes[frame]
                //int leftframe = 0;
                //for (int minframe = frame; minframe >= 0; minframe--)
                //{
                //    if (stopFrame.Keys.Contains(minframe))
                //    {
                //        leftframe = minframe;
                //        break;
                //    }
                //}

                //int rightframe = maxFrame;
                //for (int maxframe = frame-1; maxframe <= maxFrame; maxframe++)
                //{
                //    if (stopFrame.Keys.Contains(maxframe))
                //    {
                //        rightframe = maxframe;
                //        break;
                //    }
                //}

                double t = ((double)(frame - leftframe)) / ((double)(rightframe - leftframe));
                double f = BasicFunction.RemapByType(t, type, this);
                double right = keyPoints[rightframe];
                double left = keyPoints[leftframe];
                keyPoints[frame] = keyPoints[leftframe] + f * (right - left);
            }

        }

        public override void RemovedFromDocument(GH_Document document)
        {
            if (Frame.Component.ClearComponent.Contains(this))
                Frame.Component.ClearComponent.Remove(this);
            foreach(Event eve in this.eventThing)
            {
                eve.Mother = null;
            }

            base.RemovedFromDocument(document);
        }

        public override bool Read(GH_IReader reader)
        {
            if (reader.ItemExists("Count"))
            {
                int n = reader.GetInt32("Count");
                for (int m = 0; m < n; m++)
                {
                    try
                    {
                        keyPoints[reader.GetInt32(m.ToString() + "K")] = reader.GetDouble(m.ToString() + "V");
                    }
                    catch
                    {

                    }
                }
            }
            reader.TryGetBoolean("ShowGraphSlider", ref ShowGraphOnSlider);

            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            int n = 0;
            foreach (var kps in keyPoints)
            {
                writer.SetInt32(n.ToString() + "K", kps.Key);
                writer.SetDouble(n.ToString() + "V", kps.Value);
                n++;
            }
            writer.SetInt32("Count", n);
            writer.SetBoolean("ShowGraphSlider", ShowGraphOnSlider);

            return base.Write(writer);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1b33879c-baa8-4d9b-acd3-475e6b8aed3c"); }
        }
    }
}