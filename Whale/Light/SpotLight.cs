using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace Whale.Light
{
    public class SpotLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SpotLight class.
        /// </summary>
        public SpotLight()
          : base("SpotLight", "SpotLight",
              "SpotLight",
              "Whale", "Light")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Param_Guid guid = new Param_Guid();
            guid.Name = "Guid";
            guid.NickName = "G";
            guid.Description = "Guid of Light";
            guid.Access = GH_ParamAccess.item;
            pManager.AddParameter(guid);
            pManager.AddPointParameter("Location", "Loc", "Location", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddPointParameter("Target", "T", "Target", GH_ParamAccess.item, new Point3d(0, 0, -1));
            pManager.AddNumberParameter("Intensity", "In", "Intensity", GH_ParamAccess.item, 100);
            pManager.AddAngleParameter("Radius", "R", "Radius, must be in 0-Pi/2.", GH_ParamAccess.item, Math.PI / 4);
            pManager.AddNumberParameter("HotSpot", "H", "HotSpot, must be in 0-1.", GH_ParamAccess.item, 0.5);
            pManager.AddColourParameter("DiffuseColor", "Di", "DiffuseColor", GH_ParamAccess.item, Color.Aqua);
            pManager.AddColourParameter("SpecularColor", "Sp", "SpecularColor", GH_ParamAccess.item, Color.Aqua);
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
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Guid guid = new Guid();
            Point3d loc = new Point3d();
            Point3d tar = new Point3d();
            double ins = 100;
            double angle = 90;
            double hot = 0.5;
            Color diffuse = Color.Aqua;
            Color spec = Color.Aqua;

            DA.GetData("Guid", ref guid);
            DA.GetData("Location", ref loc);
            DA.GetData("Target", ref tar);
            DA.GetData("Intensity", ref ins);
            DA.GetData("Radius", ref angle);
            DA.GetData("HotSpot", ref hot);
            DA.GetData("DiffuseColor", ref diffuse);

            Rhino.DocObjects.LightObject spotObj = null;
            foreach (Rhino.DocObjects.LightObject obj in Rhino.RhinoDoc.ActiveDoc.Lights)
                if (obj.Id == guid)
                {
                    spotObj = obj;
                    break;
                }

            if (((Param_Number)Params.Input[4]).UseDegrees)
                angle = RhinoMath.ToRadians(angle);

            if (angle <= 0 || angle >= Math.PI / 2)
            {
                angle = Math.PI / 4;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Radius must be in 0-Pi/2.");
            }

            if (hot <= 0 || hot >= 1)
            {
                hot = 0.5;
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "HotSpot must be in 0-1.");
            }

            spotObj.LightGeometry.LightStyle = LightStyle.WorldSpot;
            spotObj.LightGeometry.Location = loc;
            spotObj.LightGeometry.Direction = tar - loc;
            spotObj.LightGeometry.Intensity = ins;
            spotObj.LightGeometry.SpotAngleRadians = angle;
            spotObj.LightGeometry.HotSpot = hot;
            spotObj.LightGeometry.Diffuse = diffuse;
            spotObj.LightGeometry.Specular = spec;


            Rhino.RhinoDoc.ActiveDoc.Lights.Modify(guid, spotObj.LightGeometry);
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
                return Properties.Resources.SpotLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("65ed98c9-bba4-4056-8239-d3d3b09d2f24"); }
        }
    }
}