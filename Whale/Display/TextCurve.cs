using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace Whale.Display
{
    public class TextCurve : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the TextCurve class.
        /// </summary>
        public TextCurve()
          : base("TextCurve", "TextCrv",
              "TextCurve",
              "Whale", "Display")
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Location", "L", "Location", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddTextParameter("Text", "T", "Text", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Size", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Justification", "J", "0 = BottomLeft, 1 = BottomCenter, 2 = BottomRight, 3 = MiddleLeft, 4 = MiddleCenter, 5 = MiddleRight, 6 = TopLeft, 7 = TopCenter, 8 = TopRight", GH_ParamAccess.item, 4);
            pManager.AddTextParameter("Font", "F", "Font", GH_ParamAccess.item, "Arial");
            pManager.AddBooleanParameter("Bold", "B", "Bold", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("OffsetDistance", "O", "", GH_ParamAccess.item, 0);

            pManager.HideParameter(0);

            Param_Integer var = pManager[3] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("BottomLeft", 0);
                var.AddNamedValue("BottomCenter", 1);
                var.AddNamedValue("BottomRight", 2);
                var.AddNamedValue("MiddleLeft", 3);
                var.AddNamedValue("MiddleCenter", 4);
                var.AddNamedValue("MiddleRight", 5);
                var.AddNamedValue("TopLeft", 6);
                var.AddNamedValue("TopCenter", 7);
                var.AddNamedValue("TopRight", 8);
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve", GH_ParamAccess.list);
            pManager.AddCurveParameter("Bound", "B", "Bound", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = new Plane();
            string text = "";
            double size = 5;
            int justify = 0;
            string font = "";
            bool bold = false;
            double offsetDis = 0;

            DA.GetData("Location", ref plane);
            DA.GetData("Text", ref text);
            DA.GetData("Size", ref size);
            DA.GetData("Justification", ref justify);
            DA.GetData("Font", ref font);
            DA.GetData("Bold", ref bold);
            DA.GetData("OffsetDistance", ref offsetDis);

            if (justify < 0 || justify > 8)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid Justification.Only 0-8 are allowed.");
                justify = 4;
            }

            TextJustification justification = TextJustification.MiddleCenter;
            if (justify == 0)
            {
                Message = "BottomLeft";
                justification = TextJustification.BottomLeft;
            }
            else if (justify == 1)
            {
                Message = "BottomCenter";
                justification = TextJustification.BottomCenter;
            }
            else if (justify == 2)
            {
                Message = "BottomRight";
                justification = TextJustification.BottomRight;
            }
            else if (justify == 3)
            {
                Message = "MiddleLeft";
                justification = TextJustification.MiddleLeft;
            }
            else if (justify == 4)
            {
                Message = "MiddleCenter";
                justification = TextJustification.MiddleCenter;
            }
            else if (justify == 5)
            {
                Message = "MiddleRight";
                justification = TextJustification.MiddleRight;
            }
            else if (justify == 6)
            {
                Message = "TopLeft";
                justification = TextJustification.TopLeft;
            }
            else if (justify == 7)
            {
                Message = "TopCenter";
                justification = TextJustification.TopCenter;
            }
            else if (justify == 8)
            {
                Message = "TopRight";
                justification = TextJustification.TopRight;
            }


            TextEntity t = new TextEntity();
            t.Plane = plane;
            t.PlainText = text;
            t.DimensionScale = size;
            t.Font = Rhino.DocObjects.Font.InstalledFonts(font)[0];
            t.Justification = justification;
            t.SetBold(bold);
            List<Curve> curves = new List<Curve>(t.Explode());

            List<Point3d> boxCorner = new List<Point3d>();
            foreach (Curve crv in curves)
                foreach (Point3d pt in crv.GetBoundingBox(plane).GetCorners())
                    boxCorner.Add(pt);

            Box box = new Box(plane, new BoundingBox(boxCorner));
            Point3d[] corners = box.GetCorners();
            Point3d[] pts = new Point3d[5] { corners[0], corners[1], corners[2], corners[3], corners[0] };

            PolylineCurve polyCrv = new PolylineCurve(pts);
            Curve bound;
            if (offsetDis == 0)
                bound = polyCrv;
            else
                bound = polyCrv.Offset(plane, offsetDis, GH_Component.DocumentTolerance(), CurveOffsetCornerStyle.Round)[0];



            DA.SetDataList("Curve", curves);
            DA.SetData("Bound", bound);
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
                return Properties.Resources.TextCurve;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9e25ba32-5fd7-416c-ac01-841fe2bb5d15"); }
        }
    }
}