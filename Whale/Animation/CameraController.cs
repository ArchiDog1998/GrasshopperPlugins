using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Rhino;
using Rhino.Geometry.Intersect;
using Rhino.Display;

namespace Whale.Animation
{
    public class CameraController : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CameraController class.
        /// </summary>
        public CameraController()
          : base("CameraController", "Camera",
              "Camera Controller",
              "Whale", "Animation")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Run", GH_ParamAccess.item, true);
            pManager.AddPointParameter("CameraLocation", "CL", "CameraLocation", GH_ParamAccess.item);
            pManager.AddPointParameter("CameraTarget", "CT", "CameraTarget", GH_ParamAccess.item);
            pManager.AddIntegerParameter("CameraType", "T", "CameraType", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("FocalLength", "L", "FocalLength", GH_ParamAccess.item, 50);
            pManager.AddAngleParameter("Bias", "B", "Bias in Radius", GH_ParamAccess.item, 0);
            pManager.AddPointParameter("UV", "UV", "nearly UV to refine the target on view.", GH_ParamAccess.item, new Point3d(0.5, 0.5, 0));
            pManager.AddTextParameter("ViewName", "N", "ViewName", GH_ParamAccess.item);
            pManager[7].Optional = true;

            ((IGH_PreviewObject)this).Hidden = true;

            Param_Integer var = pManager[3] as Param_Integer;
            if (var != null)
            {
                var.AddNamedValue("Parallel Projection", 0);
                var.AddNamedValue("Two Point Perspective Projection", 1);
                var.AddNamedValue("Perspective Projection", 2);
            }

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddRectangleParameter("Rectangle", "R", "Rectangle", GH_ParamAccess.item);
            pManager.AddNumberParameter("UnitPerPx", "U", "UnitPerPt", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            Point3d location = new Point3d();
            Point3d target = new Point3d();
            double length = 50;
            double bias = 0;
            int type = 0;
            Point3d uv = new Point3d();
            Rhino.Display.RhinoViewport viewPort = null;

            DA.GetData("Run", ref run);
            DA.GetData("CameraLocation", ref location);
            DA.GetData("CameraTarget", ref target);
            DA.GetData("FocalLength", ref length);
            DA.GetData("Bias", ref bias);
            DA.GetData("CameraType", ref type);
            DA.GetData("UV", ref uv);

            string name = "";
            bool flag = true;
            bool useFrame = false;
            if(DA.GetData("ViewName", ref name))
            {
                foreach(RhinoView view in Rhino.RhinoDoc.ActiveDoc.Views)
                {
                    if(view.ActiveViewport.Name == name)
                    {
                        viewPort = view.ActiveViewport;
                        flag = false;
                        break;
                    }
                }
            }

            if(flag)
            {
                try
                {
                    viewPort = Frame.Component.RhinoView.ActiveViewport;
                    useFrame = true;
                }
                catch
                {
                    viewPort = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
                }
                
            }
            Message = viewPort.Name;
            //viewPort.DisplayMode.DisplayAttributes.FillMode = DisplayPipelineAttributes.FrameBufferFillMode.SolidColor;
            //viewPort.DisplayMode.DisplayAttributes.SetFill()

            if (run)
            {
                //basic settings
                viewPort.SetCameraLocations(target, location);
                viewPort.CameraUp = Vector3d.ZAxis;

                switch (type)
                {
                    case 0:
                        viewPort.ChangeToParallelProjection(true);
                        break;
                    case 1:
                        viewPort.ChangeToTwoPointPerspectiveProjection(length);
                        break;
                    case 2:
                        viewPort.ChangeToPerspectiveProjection(true, length);
                        break;
                    default:
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input Type must be in 0-2!");
                        viewPort.ChangeToPerspectiveProjection(true, length);
                        break;

                }

                if (((Param_Number)Params.Input[5]).UseDegrees)
                    bias = RhinoMath.ToRadians(bias);
                Vector3d z = Vector3d.ZAxis;
                z.Rotate(bias, viewPort.CameraZ);
                viewPort.CameraUp = z;

                Point3d[] nearCorners = viewPort.GetNearRect();
                Point3d[] farCorners = viewPort.GetFarRect();


                Plane targetPl = new Plane(target, viewPort.CameraX, viewPort.CameraY);
                double param;
                Line ln1 = new Line(nearCorners[0], farCorners[0]);
                Line ln2 = new Line(nearCorners[3], farCorners[3]);
                Intersection.LinePlane(ln1, targetPl, out param);
                Rectangle3d rectTarget = new Rectangle3d(targetPl, ln1.PointAt(param), ln2.PointAt(param));
                Point3d X = rectTarget.PointAt(uv.X, uv.Y);

                viewPort.SetCameraLocations(NewTarget(location, target, X), location);

                nearCorners = viewPort.GetNearRect();
                farCorners = viewPort.GetFarRect();
                Point3d[] corners = new Point3d[4]
                    {
                    (nearCorners[0] * 0.99 + farCorners[0] * 0.01),
                    (nearCorners[1] * 0.99 + farCorners[1] * 0.01),
                    (nearCorners[2] * 0.99 + farCorners[2] * 0.01),
                    (nearCorners[3] * 0.99 + farCorners[3] * 0.01)
                    };

                Plane recPlane = new Plane(corners[0], corners[1], corners[2]);
                Rectangle3d rect = new Rectangle3d(recPlane, corners[0], corners[3]);


                double viewRectWidth = corners[0].DistanceTo(corners[1]);
                double unitPerPx;
                if(useFrame)
                {
                    unitPerPx = viewRectWidth / (Frame.Component.Play ? Frame.Component.PictWidth : viewPort.Size.Width);
                    Message += "\n" + (Frame.Component.Play ? "Product" : "Preview");
                }
                else
                {
                    unitPerPx = viewRectWidth / viewPort.Size.Width;
                }
                

                
                

                DA.SetData("Rectangle", rect);
                DA.SetData("UnitPerPx", unitPerPx);
            }
        }

        private Point3d NewTarget(Point3d location, Point3d target, Point3d X)
        {
            X.Transform(Transform.Mirror(new Plane(target, X - target)));
            Line ln = new Line(location, X);
            return ln.PointAt(ln.ClosestParameter(target));
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
                return Properties.Resources.CameraController;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3b0b31bd-4431-438d-8010-8b8be1c33213"); }
        }
    }
}