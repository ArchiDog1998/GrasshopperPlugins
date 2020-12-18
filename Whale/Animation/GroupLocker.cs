using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;

namespace Whale.Animation
{
    public class GroupLocker : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the GroupLocker class.
        /// </summary>
        public GroupLocker()
          : base("GroupLocker", "GLocker",
              "GroupLocker",
              "Whale", "Animation")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        List<GH_Group> aimGroup = new List<GH_Group>();
        Interval timeDomain = new Interval();

        protected void FindGroup()
        {
            GH_Document doc = this.OnPingDocument();
            if (doc == null)
                return;

            List<GH_Group> grp = new List<GH_Group>();
            foreach (IGH_DocumentObject obj in doc.Objects)
                if (obj is GH_Group)
                    if (((GH_Group)obj).ObjectIDs.Contains(this.InstanceGuid))
                        grp.Add((GH_Group)obj);
            aimGroup = grp;
        }

        protected List<IGH_ActiveObject> FindObject()
        {
            List<IGH_ActiveObject> waitToLock = new List<IGH_ActiveObject>();
            foreach (GH_Group group in aimGroup)
            {
                foreach (IGH_DocumentObject obj in group.Objects())
                    if (obj is IGH_ActiveObject && obj.InstanceGuid != this.InstanceGuid)
                        waitToLock.Add((IGH_ActiveObject)obj);
            }
            return waitToLock;
        }

        public void ToLockIt(double rightTime)
        {
            if (aimGroup.Count == 0)
                FindGroup();
            if (aimGroup.Count == 0)
                return;

            List<IGH_ActiveObject> ao = FindObject();

            ao.ForEach(delegate (IGH_ActiveObject o)
            {
                o.Locked = !timeDomain.IncludesParameter(rightTime);
            });
            ao.ForEach(delegate (IGH_ActiveObject o)
            {
                ((IGH_DocumentObject)o).ExpireSolution(false);
            });
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("TimeDomain", "T", "TimeDomain", GH_ParamAccess.item);

            this.Hidden = true;
            pManager[0].WireDisplay = GH_ParamWireDisplay.faint;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Lock?", "L", "Lock them?", GH_ParamAccess.item);
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

            if (!Animation.Frame.Component.ToLockList.Contains(this))
                Animation.Frame.Component.ToLockList.Add(this);

            if (this.Params.Input.Count == 1)
                DA.GetData("TimeDomain", ref timeDomain);
            else if (this.Params.Input.Count == 2)
            {
                Interval startDomain = new Interval();
                Interval endDomain = new Interval();
                DA.GetData("StartTime", ref startDomain);
                DA.GetData("EndTime", ref endDomain);
                timeDomain = new Interval(startDomain.T0, endDomain.T1);
            }
            DA.SetData("Lock?", !timeDomain.IncludesParameter(Animation.Frame.Component.RightTime));
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
                return Properties.Resources.GroupLocker;
            }
        }
        public override void RemovedFromDocument(GH_Document document)
        {
            if (Animation.Frame.Component.ToLockList.Contains(this))
                Animation.Frame.Component.ToLockList.Remove(this);
            base.RemovedFromDocument(document);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (this.Params.Input.Count == 1 && side == GH_ParameterSide.Input && index == 1)
                return true;
            else return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (this.Params.Input.Count == 2 && side == GH_ParameterSide.Input && index == 1)
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

            if (this.Params.Input.Count == 2)
            {
                param.Name = "StartTime";
                param.NickName = "S";
                param.Description = "Start TimeDomain";
                param.Access = GH_ParamAccess.item;
            }
            else if (this.Params.Input.Count == 1)
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
            get { return new Guid("95a69c3b-a84b-4550-a6e6-04605aaa1dac"); }
        }
    }
}