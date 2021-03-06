﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Rect
{
    public class HorizontalRects : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HorizontalRect class.
        /// </summary>
        public HorizontalRects()
          : base("HorizontalRects", "HorzRect",
              "HorizontalRects",
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
            pManager.AddNumberParameter("Relative Sizes", "rS", "List of numbers that define tiles relative sizes", GH_ParamAccess.list, new List<double>() { 1.0, 1.0, 1.0 });
            pManager.AddNumberParameter("Height", "H", "Height of tiles as pct of boundary height", GH_ParamAccess.item, 0.25);
            pManager.AddNumberParameter("Width Padding", "Wp", "Width padding as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddNumberParameter("Height Padding", "Hp", "Height padding as pct of padding axis", GH_ParamAccess.item, 0.02);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("RestRectangle", "rR", "RestRectangle", GH_ParamAccess.item);
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

            List<double> R = new List<double>();
            DA.GetDataList(1, R);

            double H = 0.25;
            DA.GetData(2, ref H);

            double WP = 0.02;
            DA.GetData(3, ref WP);

            double HP = 0.02;
            DA.GetData(4, ref HP);



            //Output
            Rectangle3d rest;
            DA.SetDataList(1, RectSolution.HorizontalTiles(B, R, H, WP, HP, out rest));
            DA.SetData(0, rest);

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
                return Properties.Resources.HorizontalRect;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("875e9ae7-338a-45c4-8f60-da4d896a21ea"); }
        }
    }
}