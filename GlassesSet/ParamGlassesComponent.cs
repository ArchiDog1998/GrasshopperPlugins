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
        private readonly string _showLabel = "showLabel";
        private readonly bool _showLabelDefault = false;
        public bool IsShowLabel => GetValue(_showLabel, _showLabelDefault);

        private readonly string _showLegend = "showLegend";
        private readonly bool _showLegendDefault = false;
        public bool IsShowLegend => GetValue(_showLegend, _showLegendDefault);

        private readonly string _showControl = "showControl";
        private readonly bool _showControlDefault = false;
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

            var funcs = WinformControlHelper.GetInnerRectRightFunc(1, 3, new SizeF(width, width), out _);

            ClickButtonIcon<LangWindow> LabelButton = new ClickButtonIcon<LangWindow>(_showLabel, this, funcs(0), true, Properties.Resources.LabelIcon, _showLabelDefault,
               tips: new string[] { "Click to choose whether to show the wire's label.", "点击以选择是否要显示连线的名称。" },
               createMenu: () =>
               {
                   return null;
                   //ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true }

                   //return menu;
               });

            ClickButtonIcon<LangWindow> LegendButton = new ClickButtonIcon<LangWindow>(_showLegend, this, funcs(1), true, Properties.Resources.LegendIcon, _showLabelDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    return null;
                    //ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true }

                    //return menu;
                });

            ClickButtonIcon<LangWindow> ControlButton = new ClickButtonIcon<LangWindow>(_showControl, this, funcs(2), true, Properties.Resources.InputLogo, _showControlDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    return null;
                    //ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true }

                    //return menu;
                });

            this.Controls = new IRespond[] { LabelButton, LegendButton, ControlButton};

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


    }
}