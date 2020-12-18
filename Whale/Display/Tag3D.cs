using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Rhino.Display;

namespace Whale.Display
{
    public class Tag3D : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Tag3D class.
        /// </summary>
        public Tag3D()
          : base("Tag3D", "Tag3D",
              "Tag3D",
              "Whale", "Display")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        private BoundingBox clip;

        List<string> txt;
        List<Color> col;
        List<Plane> pln;
        List<double> hgt;
        List<string> font;
        List<bool> bold;
        List<bool> italy;

        protected override void BeforeSolveInstance()
        {
            txt = new List<string>();
            col = new List<Color>();
            pln = new List<Plane>();
            hgt = new List<double>();
            font = new List<string>();
            bold = new List<bool>();
            italy = new List<bool>();
        }

        public override BoundingBox ClippingBox => clip;

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (Locked) return;
            base.DrawViewportWires(args);

            if (txt != null)
                for (int i = 0; i < txt.Count; i++)
                {
                    if (this.Attributes.Selected)
                    {
                        args.Display.Draw3dText(txt[i], this.OnPingDocument().PreviewColourSelected, pln[i], hgt[i], font[i], bold[i], italy[i]);
                    }
                    else
                        args.Display.Draw3dText(txt[i], col[i], pln[i], hgt[i], font[i], bold[i], italy[i]);
                }
                    
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Location", "L", "Location", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddTextParameter("Text", "T", "Text", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "Size", GH_ParamAccess.item, 20);
            pManager.AddColourParameter("Color", "C", "Color", GH_ParamAccess.item, Color.Aqua);
            pManager.AddIntegerParameter("Justification", "J", "Text justification", GH_ParamAccess.item, 7);
            pManager.AddTextParameter("Font", "F", "Font", GH_ParamAccess.item, "Arial");
            pManager.AddBooleanParameter("Bold", "B", "Bold", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Italy", "I", "Italy", GH_ParamAccess.item, false);

            pManager.HideParameter(0);
            pManager[1].Optional = true;
            pManager[3].Optional = true;
            Param_Integer obj = (Param_Integer)pManager[4];
            obj.AddNamedValue("Top Left", 1);
            obj.AddNamedValue("Top Center", 2);
            obj.AddNamedValue("Top Right", 3);
            obj.AddNamedValue("Middle Left", 4);
            obj.AddNamedValue("Middle Center", 5);
            obj.AddNamedValue("Middle Right", 6);
            obj.AddNamedValue("Bottom Left", 7);
            obj.AddNamedValue("Bottom Center", 8);
            obj.AddNamedValue("Bottom Right", 9);
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
            Plane plane = Plane.WorldXY;
            string text = "";
            double height = 0.0;
            Color color = Color.Black;
            bool boldIt = false;
            bool italyIt = false;
            string Font = "";
            int justi = 0;

            DA.GetData("Location", ref plane);
            DA.GetData("Text", ref text);
            DA.GetData("Size", ref height);
            DA.GetData("Color", ref color);
            DA.GetData("Bold", ref boldIt);
            DA.GetData("Italy", ref italyIt);
            DA.GetData("Font", ref Font);
            DA.GetData("Justification", ref justi);


            Text3d text3d = new Text3d(text, Plane.WorldXY, height);
            BoundingBox boundingBox = text3d.BoundingBox;
            double u = 0.0;
            double v = 0.0;
            switch (justi)
            {
                case 1:
                case 2:
                case 3:
                    v = -1.0 * boundingBox.Diagonal.Y;
                    break;
                case 4:
                case 5:
                case 6:
                    v = -0.5 * boundingBox.Diagonal.Y;
                    break;
            }
            switch (justi)
            {
                case 2:
                case 5:
                case 8:
                    u = -0.5 * boundingBox.Diagonal.X;
                    break;
                case 3:
                case 6:
                case 9:
                    u = -1.0 * boundingBox.Diagonal.X;
                    break;
            }
            plane.Origin = plane.PointAt(u, v);
            text3d.TextPlane = plane;
            clip.Union(text3d.BoundingBox);

            txt.Add(text);
            col.Add(color);
            pln.Add(plane);
            hgt.Add(height);
            font.Add(Font);
            bold.Add(boldIt);
            italy.Add(italyIt);
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
                return Properties.Resources.Tag3D;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("54a98a8a-1b88-46af-b4ed-3444af83575c"); }
        }
    }
}