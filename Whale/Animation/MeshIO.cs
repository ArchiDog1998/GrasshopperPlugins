using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Rhino.FileIO;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;

namespace Whale.Animation
{
    public class MeshIO : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshIO class.
        /// </summary>
        public MeshIO()
          : base("MeshIO", "MeshIO",
              "MeshInput and Output",
              "Whale", "Animation")
        {
            ValuesChanged();
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.list);

            this.Hidden = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Frame frameComponent;
            if (!BasicFunction.ConnectToFrame(this, out frameComponent))
                return;

            if (!frameComponent.Play)
            {
                if (GetValue("InPut", @default: false))
                    Message = "InPut";
                else
                    Message = "OutPut";
                return;
            }

            string numberofMaxFrame = "D" + ((int)frameComponent.MaxFrame).ToString().Length.ToString();
            string location = frameComponent.FilePath + this.NickName + "  " + frameComponent.RightFrame.ToString(numberofMaxFrame) + ".3dm";

            if (GetValue("InPut", @default: false))
            {
                Message = "InPut";

                List<IGH_Goo> list = new List<IGH_Goo>();
                try
                {
                    File3dm val = File3dm.Read(location);
                    if (val == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "File could not be read");
                        return;
                    }
                    foreach (File3dmObject item in (CommonComponentTable<File3dmObject>)(object)val.Objects)
                    {
                        IGH_Goo iGH_Goo = GH_Convert.ToGoo(item.Geometry);
                        if (iGH_Goo != null)
                        {
                            list.Add(iGH_Goo);
                        }

                    }
                    Message += "  " + frameComponent.RightFrame.ToString();
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error during file reading: " + ex.Message);
                    return;
                }

                DA.SetDataList("Geometry", list);
            }
            else
            {
                Message = "OutPut";

                List<Mesh> meshes = new List<Mesh>();
                DA.GetDataList("Mesh", meshes);

                File3dm file3d = new File3dm();
                foreach (Mesh mesh in meshes)
                    file3d.Objects.AddMesh(mesh);
                file3d.Write(location, 6);

                Message += "  " + frameComponent.RightFrame.ToString();
            }
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            GH_DocumentObject.Menu_AppendItem(menu, "InPut", Menu_OutPutClicked, enabled: true, GetValue("InPut", @default: false)).ToolTipText = "When checked, it will change to InPut.";
        }

        private void Menu_OutPutClicked(object sender, EventArgs e)
        {
            if (GetValue("InPut", @default: false))
            {
                RecordUndoEvent("InPut");
            }
            else
            {
                RecordUndoEvent("InPut");
            }
            SetValue("InPut", !GetValue("InPut", @default: false));
            ExpireSolution(recompute: true);
        }

        protected override void ValuesChanged()
        {
            if (GetValue("InPut", @default: false))
            {
                Message = "InPut";
            }
            else
            {
                Message = "OutPut";
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
                return Properties.Resources.MeshIO;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("17e5bd55-d737-41a6-b149-018f6d151ce6"); }
        }
    }
}