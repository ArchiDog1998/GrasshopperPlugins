using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Reflection;
namespace InfoGlasses
{
    public class MyComponent1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("OverrideTest", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
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
            MethodInfo info1 = typeof(Test).GetMethods().Where((method) => method.Name.Contains("Show")).First();
            MethodInfo info2 = typeof(Another).GetMethods().Where((method) => method.Name.Contains("ShowNEw")).First();
            ArchiTed_Grasshopper.Unsafe.UnsafeHelper.ExchangeMethod(info1, info2);
            System.Windows.Forms.MessageBox.Show(new Test().Show());
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
            get { return new Guid("2EBCC201-2EF7-4855-A9D9-EED5EF36DFB6"); }
        }
    }

    public class Test
    {
        public string MyProperty { get; set; } = "What?";
        public string Show()
        {
            return MyProperty + "Hello";
        }
    }

    public class Another : Test
    {
        public string ShowNEw()
        {
            return MyProperty + "成功了";
        }
    }
}