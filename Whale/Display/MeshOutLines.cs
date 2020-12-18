using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Display;

using Whale.Animation;

namespace Whale.Display
{
    public class MeshOutLines : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshOutLine class.
        /// </summary>
        public MeshOutLines()
          : base("MeshOutLines", "outLn",
              "MeshOutLines",
              "Whale", "Display")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "Plane", GH_ParamAccess.item);
            pManager.AddTextParameter("ViewName", "N", "ViewName", GH_ParamAccess.item);

            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("OutLine", "L", "OutLine", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = new Plane();
            Mesh mesh = null;
            string viewName = "";

            DA.GetData("Mesh", ref mesh);
            DA.GetData("Plane", ref plane);

            RhinoViewport rhinoView;
            Animation.Frame frameComponent;
            if (DA.GetData("ViewName", ref viewName))
            {
                foreach (RhinoView view in Rhino.RhinoDoc.ActiveDoc.Views)
                {
                    if (view.ActiveViewport.Name == viewName)
                    {
                        rhinoView = view.ActiveViewport;
                        DA.SetDataList("OutLine", mesh.GetOutlines(new ViewportInfo(rhinoView), plane));
                        break;
                    }
                }
            }
            else if (BasicFunction.ConnectToFrame(this, out frameComponent))
            {
                rhinoView = frameComponent.RhinoView.ActiveViewport;
                DA.SetDataList("OutLine", mesh.GetOutlines(new ViewportInfo(rhinoView), plane));
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input Frame and ViewName require an input!");
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
                return Properties.Resources.MeshOutLine;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3e7beda0-7d07-489a-9a2a-dd70436c89dd"); }
        }
    }
}