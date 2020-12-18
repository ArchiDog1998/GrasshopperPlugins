using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Whale.Display
{
    public class CustomMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CustomMaterial class.
        /// </summary>
        public CustomMaterial()
          : base("CustomMaterial", "Material",
              "Create CustomMaterial",
              "Whale", "Display")
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Diffuse", "D", "Diffuse", GH_ParamAccess.item, Color.Aqua);
            pManager.AddColourParameter("Emission", "E", "Emission", GH_ParamAccess.item, Color.Aqua);
            pManager.AddColourParameter("Specular", "Sp", "Specular", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("Shine", "S", "Shine", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Transparency", "T", "Transparency", GH_ParamAccess.item, 0);

            Param_FilePath Dt = new Param_FilePath();
            Dt.Optional = true;
            pManager.AddParameter(Dt, "DiffuseTexture", "Dt", "DiffuseTexture", GH_ParamAccess.item);

            Param_FilePath Tt = new Param_FilePath();
            Tt.Optional = true;
            pManager.AddParameter(Tt, "TransparencyTexture", "Tt", "TransparencyTexture", GH_ParamAccess.item);

            Param_FilePath Bt = new Param_FilePath();
            Bt.Optional = true;
            pManager.AddParameter(Bt, "BumpTexture", "Bt", "BumpTexutre", GH_ParamAccess.item);

            Param_FilePath Et = new Param_FilePath();
            Et.Optional = true;
            pManager.AddParameter(Et, "EnvironmentTexture", "Et", "EnvironmentTexture", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            Param_OGLShader param_OGLShader = new Param_OGLShader();
            pManager.AddParameter(param_OGLShader, "Material", "M", "The material override", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Color diff = Color.Aqua;
            string diffText = "";
            Color emiss = Color.Black;
            double shine = 0;
            Color spec = Color.White;
            double trans = 0;
            string transText = "";
            string bumpText = "";
            string enviText = "";

            DA.GetData("Diffuse", ref diff);
            DA.GetData("DiffuseTexture", ref diffText);
            DA.GetData("Emission", ref emiss);
            DA.GetData("Shine", ref shine);
            DA.GetData("Specular", ref spec);
            DA.GetData("Transparency", ref trans);
            DA.GetData("TransparencyTexture", ref transText);
            DA.GetData("BumpTexture", ref bumpText);
            DA.GetData("EnvironmentTexture", ref enviText);

            Rhino.Display.DisplayMaterial matt = new Rhino.Display.DisplayMaterial();

            matt.BackDiffuse = diff;
            matt.Diffuse = diff;
            if (diffText != "")
                matt.SetBitmapTexture(diffText, true);

            matt.BackTransparency = trans;
            matt.Transparency = trans;
            if (transText != "")
                matt.SetTransparencyTexture(transText, true);

            if (bumpText != "")
                matt.SetBumpTexture(bumpText, true);

            if (enviText != "")
                matt.SetEnvironmentTexture(enviText, true);

            matt.BackEmission = emiss;
            matt.Emission = emiss;

            matt.BackSpecular = spec;
            matt.Specular = spec;

            matt.BackShine = shine;
            matt.Shine = shine;


            GH_Material material = new GH_Material(matt);
            DA.SetData("Material", material);

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
                return Properties.Resources.CustomMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("065353e3-0814-49e8-ba80-77b0ffd3d1af"); }
        }
    }
}