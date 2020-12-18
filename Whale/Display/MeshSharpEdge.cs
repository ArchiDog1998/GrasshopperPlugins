using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Geometry;


namespace Whale.Display
{
    public class MeshSharpEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshSharpEdge class.
        /// </summary>
        public MeshSharpEdge()
          : base("MeshSharpEdge", "MeshSEdge",
              "MeshSharpEdge",
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
            pManager.AddAngleParameter("AngleLimit", "A", "AngleLimit", GH_ParamAccess.item, Math.PI / 6);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Edge", "E", "Edge", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = new Mesh();
            double angle = Math.PI / 6;

            DA.GetData("Mesh", ref mesh);
            DA.GetData("AngleLimit", ref angle);

            if (((Param_Number)Params.Input[1]).UseDegrees)
                angle = RhinoMath.ToRadians(angle);

            mesh.FaceNormals.ComputeFaceNormals();

            List<Line> meshLine = new List<Line>();
            for (int i = 0; i <= mesh.TopologyEdges.Count; i++)
            {
                int[] faces = mesh.TopologyEdges.GetConnectedFaces(i);
                if (faces.Length == 2)
                {
                    Vector3d vec0 = mesh.FaceNormals[faces[0]];
                    Vector3d vec1 = mesh.FaceNormals[faces[1]];
                    if (Vector3d.VectorAngle(vec0, vec1) > angle)
                        meshLine.Add(mesh.TopologyEdges.EdgeLine(i));

                }
                else
                    meshLine.Add(mesh.TopologyEdges.EdgeLine(i));
            }

            DA.SetDataList("Edge", meshLine);
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
                return Properties.Resources.MeshSharpEdge;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fdbe9de5-bf21-40c8-8e0c-dd3d4e176f25"); }
        }
    }
}