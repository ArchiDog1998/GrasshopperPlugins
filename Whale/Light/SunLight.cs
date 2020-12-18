using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Light
{
    public class SunLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SunLight class.
        /// </summary>
        public SunLight()
          : base("SunLight", "Sun",
              "SunLight",
              "Whale", "Light")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item, true);
            pManager.AddTimeParameter("Time", "T", "Time", GH_ParamAccess.item);
            pManager.AddNumberParameter("LatitudeDegrees", "La", "LatitudeDegrees", GH_ParamAccess.item, 30.28);
            pManager.AddNumberParameter("LongitudeDegrees", "Lo", "LongitudeDegrees", GH_ParamAccess.item, 120.15);
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
            bool run = true;
            double lati = 60;
            double longi = 60;
            DateTime time = new DateTime();

            DA.GetData("Run", ref run);
            DA.GetData("Time", ref time);
            DA.GetData("LatitudeDegrees", ref lati);
            DA.GetData("LongitudeDegrees", ref longi);

            Rhino.Render.Sun sun = Rhino.RhinoDoc.ActiveDoc.Lights.Sun;
            if (run)
            {
                sun.Enabled = true;
                sun.ManualControl = true;
                sun.SetPosition(time, lati, longi);
            }
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
                return Properties.Resources.SunManu;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("941a0895-d338-4c6c-a378-fe0c579bd359"); }
        }
    }
}