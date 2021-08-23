using System;
using System.Collections.Generic;
using System.Drawing;
using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace InfoGlasses
{
    public class MyComponent1 : ControllableComponent
    {
        public bool Checked => this.GetValue(nameof(Checked), true);
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("MyComponent1", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
            PointF inputLayoutMove;
            var funcs = WinformControlHelper.GetInnerRectLeftFunc(1, 2, new SizeF(20, 20), out inputLayoutMove);
            this.InputLayoutMove = inputLayoutMove;

            this.Controls = new IRespond[] {
                new ClickButtonName<LangWindow>(nameof(Checked), this, funcs(0), true, new string[] { "N", "名字" }, true),
                new ClickButtonName<LangWindow>(nameof(Checked), this, funcs(1), true, new string[] { "S", "第二个" }, true),
            };
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddAngleParameter("T", "T", "T", GH_ParamAccess.item);
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
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects += ActiveCanvas_CanvasPostPaintObjects;
        }

        private void ActiveCanvas_CanvasPostPaintObjects(GH_Canvas sender)
        {
            sender.Graphics.DrawString("Can I Do This?", GH_FontServer.Standard, new SolidBrush(Color.DarkRed), new PointF(0, 0));
        }

        public override void CreateWindow()
        {
            GH_Canvas canvas = Grasshopper.Instances.ActiveCanvas;
            canvas.MouseClick += Canvas_MouseClick;
        }

        private void Canvas_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GH_Canvas canvas = (GH_Canvas)sender;

            PointF pivot = canvas.Viewport.UnprojectPoint(e.Location);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("FDC62C6D-7C03-412D-8FF8-B76439197730"); }
        }
    }
}