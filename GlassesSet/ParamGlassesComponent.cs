/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.Kernel;
using InfoGlasses.WPF;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace InfoGlasses
{
    public class ParamGlassesComponent : LanguagableComponent
    {
        #region Basic Component Info
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.WireGlasses;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid =>new Guid("0100df68-78e0-42bf-8055-6e352465c8b1");
        #endregion

        #region Values
        private const string _showLabel = "showLabel";
        private const bool _showLabelDefault = false;
        public bool IsShowLabel => GetValue(_showLabel, _showLabelDefault);

        private const string _showTree = "showTree";
        private const bool _showTreeDefault = false;
        public bool IsShowTree => GetValue(_showTree, _showTreeDefault);

        private const string _showLegend = "showLegend";
        private const bool _showLegendDefault = false;
        public bool IsShowLegend => GetValue(_showLegend, _showLegendDefault);

        private const string _showControl = "showControl";
        private const bool _showControlDefault = false;
        public bool IsShowControl => GetValue(_showControl, _showControlDefault);

        #endregion

        /// <summary>
        /// Initializes a new instance of the ParamGlassesComponent class.
        /// </summary>
        public ParamGlassesComponent()
            : base(GetTransLation(new string[] { "ParamGlasses", "参数眼镜" }), GetTransLation(new string[] { "Param", "参数" }),
                 GetTransLation(new string[] { "To show the wire's and parameter's advances information.Right click or double click to have advanced options.",
                     "显示连线或参数的高级信息。右键或者双击可以获得更多选项。" }), "Params", "Showcase Tools", windowsType: typeof(WireColorsWindow))

        {
            LanguageChanged += ResponseToLanguageChanged;
            ResponseToLanguageChanged(this, new EventArgs());

            int width = 24;

            Func<RectangleF, RectangleF> changeInput;
            var inFuncs = WinformControlHelper.GetInnerRectLeftFunc(1, 2, new SizeF(width, width), out changeInput);
            this.ChangeInputLayout = changeInput;

            Func<RectangleF, RectangleF> changeOutput;
            var outFuncs = WinformControlHelper.GetInnerRectRightFunc(1, 2, new SizeF(width, width), out changeOutput);
            this.ChangeOutputLayout = changeOutput;

            ClickButtonIcon<LangWindow> LabelButton = new ClickButtonIcon<LangWindow>(_showLabel, this, inFuncs(1), true, Properties.Resources.LabelIcon, _showLabelDefault,
               tips: new string[] { "Click to choose whether to show the wire's label.", "点击以选择是否要显示连线的名称。" },
               createMenu: () =>
               {
                   ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                   return menu;
               });

            ClickButtonIcon<LangWindow> treeButton = new ClickButtonIcon<LangWindow>(_showTree, this, inFuncs(0), true, Properties.Resources.ShowTreeStructure, _showTreeDefault,
                tips: new string[] { "Click to switch whether to show the wire's data structure", "点击以选择是否要显示连线的数据结构。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                    return menu;
                });


            ClickButtonIcon<LangWindow> LegendButton = new ClickButtonIcon<LangWindow>(_showLegend, this, outFuncs(0), true, Properties.Resources.LegendIcon, _showLabelDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                    return menu;
                });

            ClickButtonIcon<LangWindow> ControlButton = new ClickButtonIcon<LangWindow>(_showControl, this, outFuncs(1), true, Properties.Resources.InputLogo, _showControlDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                    return menu;
                });

            this.Controls = new IRespond[] { LabelButton, treeButton, LegendButton, ControlButton};
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("", "", "", GH_ParamAccess.item, true);

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
        }

        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            string[] input = new string[] { GetTransLation(new string[] { "Run", "启动" }), GetTransLation(new string[] { "R", "启动" }), GetTransLation(new string[] { "Run", "启动" }) };

            ChangeComponentAtt(this, new string[] {GetTransLation(new string[] { "ParamGlasses", "参数眼镜" }), GetTransLation(new string[] { "Param", "参数" }),
                GetTransLation(new string[] { "To show the wire's and parameter's advances information.Right click or double click to have advanced options.",
                     "显示连线或参数的高级信息。右键或者双击可以获得更多选项。" }) },
                new string[][] { input }, new string[][] { });

            this.ExpireSolution(true);
        }

        public override void CreateWindow()
        {
            WinformControlHelper.CreateWindow(Activator.CreateInstance(this.WindowsType, this) as LangWindow, this);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            LanguageChanged -= ResponseToLanguageChanged;
            base.RemovedFromDocument(document);
        }


    }
}