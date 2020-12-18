using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Rect
{
    public class IrregularRects : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IrregularTiles class.
        /// </summary>
        public IrregularRects()
          : base("IrregularRects", "IrrgRect",
              "IrregularRects",
              "Whale", "Rectangle")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "B", "The boundary to tile", GH_ParamAccess.item);
            pManager.AddNumberParameter("Relative Column Sizes", "rC", "List of numbers that define tiles relative column sizes", GH_ParamAccess.list, new List<double>() { 1.0, 2.0, 1.0 });
            pManager.AddNumberParameter("Relative Row Sizes", "rR", "List of numbers that define tiles relative row sizes", GH_ParamAccess.list, new List<double>() { 1.0, 2.0, 1.0 });
            pManager.AddNumberParameter("Interior Padding", "Ip", "Interior padding between cells as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddNumberParameter("Exterior Padding", "Ep", "Padding around exterior as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddIntegerParameter("Padding Axis", "Pa", "Axis to calculate padding from (0=X axis, 1=Y axis)", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_RectangleParam("Rectangles", "R", "Rectangles subdivisions", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rectangle3d B = Rectangle3d.Unset;
            DA.GetData(0, ref B);

            List<double> XR = new List<double>();
            DA.GetDataList(1, XR);

            List<double> YR = new List<double>();
            DA.GetDataList(2, YR);

            double IP = 0.02;
            DA.GetData(3, ref IP);

            double EP = 0.02;
            DA.GetData(4, ref EP);

            int PA = 0;
            DA.GetData(5, ref PA);

            //Output
            DA.SetDataList(0, RectSolution.IrregularTiles(B, XR, YR, EP, IP, PA));
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
                return Properties.Resources.IrregularRect;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9df768eb-745e-45b4-a8b1-5b1676d21ad9"); }
        }
    }
}