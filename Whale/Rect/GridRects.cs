using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Rect
{
    public class GridRects : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GridRect class.
        /// </summary>
        public GridRects()
          : base("GridRects", "GridRect",
              "GridRects",
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
            pManager.AddIntegerParameter("Columns", "C", "Number of columns for boundary subdivision", GH_ParamAccess.item, 5);
            pManager.AddIntegerParameter("Rows", "R", "Number of rows for boundary subdivision", GH_ParamAccess.item, 5);
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

            int C = 5;
            DA.GetData(1, ref C);

            int R = 5;
            DA.GetData(2, ref R);

            double IP = 0.02;
            DA.GetData(3, ref IP);

            double EP = 0.02;
            DA.GetData(4, ref EP);

            int PA = 0;
            DA.GetData(5, ref PA);

            //Output
            DA.SetDataList(0, RectSolution.GridTiles(B, IP, EP, C, R, PA));
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
                return Properties.Resources.GridRect;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0d5da376-a5d1-4898-96df-a32d321de99f"); }
        }
    }
}