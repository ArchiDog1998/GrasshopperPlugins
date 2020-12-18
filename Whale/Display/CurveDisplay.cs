using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Display
{
    public class CurveDisplay : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CurveDisplay class.
        /// </summary>
        public CurveDisplay()
          : base("CurveDisplay", "CrvDis",
              "CurveDisplay",
              "Whale", "Display")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        List<Curve> crv;
        List<Color> col;
        List<int> thick;

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
            if (!this.Locked && !this.Hidden && crv != null)
                for (int i = 0; i < crv.Count; i++)
                {
                    if (this.Attributes.Selected)
                    {
                        args.Display.DrawCurve(crv[i],this.OnPingDocument().PreviewColourSelected, thick[i]);
                    }
                    else
                        args.Display.DrawCurve(crv[i], col[i], thick[i]);
                }
                    
        }

        protected override void BeforeSolveInstance()
        {
            crv = new List<Curve>();
            col = new List<Color>();
            thick = new List<int>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "c", "Color", GH_ParamAccess.item, Color.Aqua);
            pManager.AddIntegerParameter("Thickness", "T", "Thickness", GH_ParamAccess.item, 2);
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
            Curve curve = null;
            Color color = Color.Aqua;
            int thickness = 0;


            DA.GetData("Curve", ref curve);
            DA.GetData("Color", ref color);
            DA.GetData("Thickness", ref thickness);

            crv.Add(curve);
            col.Add(color);
            thick.Add(thickness);
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
                return Properties.Resources.DrawCurve;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("83dd0654-e411-4b47-8e80-171234c2e60c"); }
        }
    }
}