using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using System.Linq;
using Grasshopper.Kernel.Parameters.Hints;
using Grasshopper.GUI;
using System.Windows.Forms;

namespace Whale.Animation
{
    public class Event : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the Event class.
        /// </summary>
        public Event()
          : base("Event", "Event",
              "SingleEvent",
              "Whale", "Animation")
        {
        }

        public int StartFrame = 0;
        public int EndFrame = 0;
        public Interval timedomain = new Interval();
        public Interval valuedomain = new Interval();
        public EventOperation Mother = null;

        public List<int> frames;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public class EventAttributes : GH_ComponentAttributes
        {

            public new Event Owner;
            public EventAttributes(Event owner) : base(owner)
            {
                Owner = owner;
            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f && Owner.Attributes.Bounds.Contains(e.CanvasLocation))
                {
                    if (Owner.Mother != null)
                    {
                        Owner.Mother.GoComponent(Owner.Mother);
                        return GH_ObjectResponse.Release;
                    }
                        

                }
                return base.RespondToMouseDoubleClick(sender, e);
            }
        }

        public override void CreateAttributes()
        {
            base.m_attributes = (IGH_Attributes)(object)new EventAttributes(this);
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("TimeDomain", "T", "TimeDomain", GH_ParamAccess.item);
            pManager.AddIntervalParameter("ValueDomain", "V", "ValueDomain", GH_ParamAccess.item, new Interval(0,1));
            pManager.AddIntegerParameter("BlendType", "T", "BlendType, 0=Line, 1=Sigmoid, 2=Start, 3=End", GH_ParamAccess.item, 0);

            this.Hidden = true;

            Param_Integer var = pManager[2] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("Line", 0);
                var.AddNamedValue("Sigmoid", 1);
                var.AddNamedValue("Start", 2);
                var.AddNamedValue("End", 3);
            }

            pManager[0].WireDisplay = GH_ParamWireDisplay.faint;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "Value", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Animation.Frame frameComponent;
            if (!BasicFunction.ConnectToFrame(this, out frameComponent))
                return;

            if (this.Params.Input.Count == 3)
                DA.GetData("TimeDomain", ref timedomain);
            else if (this.Params.Input.Count == 4)
            {
                Interval startDomain = new Interval();
                Interval endDomain = new Interval();
                DA.GetData("StartTime", ref startDomain);
                DA.GetData("EndTime", ref endDomain);
                timedomain = new Interval(startDomain.T0, endDomain.T1);
            }
            DA.GetData("ValueDomain", ref valuedomain);


            this.frames = new List<int>();
            List<double> outValue = new List<double>();
            for (int i = 0; i <= frameComponent.MaxFrame; i++)
            {
                if (timedomain.IncludesParameter(frameComponent.KeyTimes[i]))
                {
                    double value = (frameComponent.KeyTimes[i] - timedomain.T0) / timedomain.Length;
                    outValue.Add(value);
                    this.frames.Add(i);
                }
            }


            int type = 0;
            DA.GetData("BlendType", ref type);

            double[] newoutValue = new double[outValue.Count];
           
            for (int n = 0; n < outValue.Count; n++)
            {
                newoutValue[n] = BasicFunction.RemapByType(outValue[n], type, this);
            }

            for (int i = 0; i < Frame.Component.KeyTimes.Count; i++)
            {
                if (Math.Abs( Frame.Component.KeyTimes[i] - timedomain.T0 )< 0.00001)
                {
                    this.StartFrame = i;
                }
                else if (Math.Abs(Frame.Component.KeyTimes[i] - timedomain.T1) < 0.00001)
                {
                    this.EndFrame = i;
                    break;
                }
            }

            DA.SetDataList("Value", newoutValue);

           //if (timedomain.IncludesParameter(frameComponent.RightTime))
           // {
           //     double value = (frameComponent.RightTime - timedomain.T0) / timedomain.Length;
           //     DA.SetData("Value", value);
           //     RightFrame = frameComponent.RightFrame;
           // }
           // else
           // {
           //     if(frameComponent.RightTime < timedomain.T0)
           //     {
           //         for(int n = 0; n < frameComponent.KeyTimes.Count; n++)
           //             if (frameComponent.KeyTimes[n] > timedomain.T0)
           //             {
           //                 RightFrame = n;
           //                 break;
           //             }
           //         DA.SetData("Value", 0);
           //     }
           //     else
           //     {
           //         for (int n = 0; n < frameComponent.KeyTimes.Count; n++)
           //             if (frameComponent.KeyTimes[n] > timedomain.T1)
           //             {
           //                 RightFrame = n-1;
           //                 break;
           //             }
           //         DA.SetData("Value", 1);
           //     }
           // }

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
                return Properties.Resources.EventOperation;
            }
        }

        protected override void BeforeSolveInstance()
        {
            base.BeforeSolveInstance();
            Param_Integer var = this.Params.Input[this.Params.Input.Count()-1] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("Line", 0);
                var.AddNamedValue("Sigmoid", 1);
                var.AddNamedValue("Start", 2);
                var.AddNamedValue("End", 3);
            }

        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (this.Params.Input.Count == 3 && side == GH_ParameterSide.Input && index == 1)
                return true;
            else return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (this.Params.Input.Count == 4 && side == GH_ParameterSide.Input && index == 1)
                return true;
            else return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_Interval var = new Param_Interval();
            var.Name = "EndTime";
            var.NickName = "E";
            var.Description = "End TimeDomain";
            var.Access = GH_ParamAccess.item;
            return var;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            var param = this.Params.Input[0];

            if (this.Params.Input.Count == 4)
            {
                param.Name = "StartTime";
                param.NickName = "S";
                param.Description = "Start TimeDomain";
                param.Access = GH_ParamAccess.item;
            }
            else if (this.Params.Input.Count == 3)
            {
                param.Name = "TimeDomain";
                param.NickName = "T";
                param.Description = "TimeDomain";
                param.Access = GH_ParamAccess.item;
            }
        }



        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("691d2e35-2620-455a-8dcf-d54df27e3a1f"); }
        }
    }
}