using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Whale.Display
{
    public class PointDisplay : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointDisplay class.
        /// </summary>
        public PointDisplay()
          : base("PointDisplay", "PointDis",
              "PointDisplay",
              "Whale", "Display")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        List<Point3d> pt;
        List<Color> col;
        List<int> rad;
        List<Rhino.Display.PointStyle> style;

        protected override void BeforeSolveInstance()
        {
            pt = new List<Point3d>();
            col = new List<Color>();
            rad = new List<int>();
            style = new List<Rhino.Display.PointStyle>();
        }


        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
            if (!this.Locked && pt != null)
                for (int i = 0; i < pt.Count; i++)
                {
                    if (this.Attributes.Selected)
                    {
                        args.Display.DrawPoint(pt[i], style[i], rad[i], this.OnPingDocument().PreviewColourSelected);
                    }
                    else
                        args.Display.DrawPoint(pt[i], style[i], rad[i], col[i]);
                }
                    
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "Color", GH_ParamAccess.item, Color.Aqua);
            pManager.AddIntegerParameter("Radius", "R", "Radius", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Type", "T", "Type", GH_ParamAccess.item, 7);

            Param_Integer var = pManager[3] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("Square", 0);
                var.AddNamedValue("ControlPoint", 1);
                var.AddNamedValue("ActivePoint", 2);
                var.AddNamedValue("X", 3);
                var.AddNamedValue("Circle", 4);
                var.AddNamedValue("RoundControlPoint", 5);
                var.AddNamedValue("RoundActivePoint", 6);
                var.AddNamedValue("Triangle", 7);
                var.AddNamedValue("Heart", 8);
                var.AddNamedValue("Chevron", 9);
                var.AddNamedValue("Clover", 10);
                var.AddNamedValue("Tag", 11);
                var.AddNamedValue("Asterisk", 12);
                var.AddNamedValue("Pin", 13);
                var.AddNamedValue("ArrowTail", 14);
                var.AddNamedValue("ArrowTip", 15);
            }
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
            Point3d point = new Point3d();
            Color color = Color.Aqua;
            int radius = 0;

            int type = 7;

            DA.GetData("Point", ref point);
            DA.GetData("Color", ref color);
            DA.GetData("Radius", ref radius);
            DA.GetData("Type", ref type);

            if (type < 0 || type > 15)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid Type.Only 0-15 are allowed.");
                type = 7;
            }

            if (type == 0) Message = "Square";
            else if (type == 1) Message = "ControlPoint";
            else if (type == 2) Message = "ActivePoint";
            else if (type == 3) Message = "X";
            else if (type == 4) Message = "Circle";
            else if (type == 5) Message = "RoundControlPoint";
            else if (type == 6) Message = "RoundActivePoint";
            else if (type == 7) Message = "Triangle";
            else if (type == 8) Message = "Heart";
            else if (type == 9) Message = "Chevron";
            else if (type == 10) Message = "Clover";
            else if (type == 11) Message = "Tag";
            else if (type == 12) Message = "Asterisk";
            else if (type == 13) Message = "Pin";
            else if (type == 14) Message = "ArrowTail";
            else if (type == 15) Message = "ArrowTip";


            pt.Add(point);
            col.Add(color);
            rad.Add(radius);
            style.Add((Rhino.Display.PointStyle)type);
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
                return Properties.Resources.PointDisplay;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0685a5a8-8db0-4775-a8fc-ce9c601821ea"); }
        }
    }
}