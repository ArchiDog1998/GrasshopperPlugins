using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Orthoptera.Language;
using Grasshopper.Kernel.Attributes;
using System.Drawing;
using Grasshopper.GUI;
using System.Reflection;
using Grasshopper.GUI.Ribbon;
using Orthoptera;
using System.Linq;

namespace InfoGlasses
{
    public class MyComponent1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("MyComponent1", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new TestAttr(this);
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
            //GH_DescriptionTable.WriteXml(new System.Globalization.CultureInfo("zh_CN"));

            //GH_DescriptionTable.WriteXml(new System.Globalization.CultureInfo(1033));
            //GH_LanguageRibbon.ChangePopulateRibbon();
            MethodInfo oldMethod = typeof(GH_Ribbon).GetRuntimeMethods().Where((method) => method.Name.Contains("PopulateRibbon")).First();
            MethodInfo newMethod = typeof(GH_LanguageRibbon).GetRuntimeMethods().Where((method) => method.Name.Contains("NewPopulateRibbon")).First();
            UnsafeHelper.ExchangeMethod(oldMethod, newMethod);
            GH_DescriptionTable.Culture = new System.Globalization.CultureInfo("zh_CN");
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
            get { return new Guid("12702A51-2750-431F-8688-5CA122AC0BB2"); }
        }
    }

    public class TestAttr : GH_ComponentAttributes
    {
        public override void SetupTooltip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Title = "Hello";
            e.Description = "To Hard";
            e.Diagram = Owner.Icon_24x24;
        }

        public TestAttr(MyComponent1 owner):base(owner)
        {

        }
    }
}