/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.Kernel;
using InfoGlasses.WPF;
using InfoGlasses.WinformControls;
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

        #region Wire Setting
        private const string _selectWireThickness = "selectWireThickness";
        private const double _selectWireThicknessDefault = 5;
        public double SelectWireThickness => GetValue(_selectWireThickness, _selectWireThicknessDefault);

        private const string _wireType = "wireType";
        private const int _wireTypeDefault = 0;
        public int WireType => GetValue(_wireType, _wireTypeDefault);

        private const string _accuracy = "accuracy";
        private const int _accuracyDefault = 0;
        public int Accuracy => GetValue(_accuracy, _accuracyDefault);
        #endregion

        #region Default Color
        private const string _defaultColor = "defaultColor";
        private readonly Color _defaultColorDefault = Color.FromArgb(150, 0, 0, 0);
        public Color DefaultColor => GetValue(_defaultColor, _defaultColorDefault);

        private const string _selectedColor = "selectedColor";
        private readonly Color _selectedColorDefault = Color.FromArgb(125, 210, 40);
        public Color SelectedColor => GetValue(_selectedColor, _selectedColorDefault);

        private const string _unselectColor = "unselectedColor";
        private readonly Color _unselectColorDefalut = Color.FromArgb(50, 0, 0, 0);
        public Color UnselectColor => GetValue(_unselectColor, _unselectColorDefalut);

        private const string _emptyColor = "emptyColor";
        private readonly Color _emptyColorDefault = Color.FromArgb(180, 255, 60, 0);
        public Color EmptyColor => GetValue(_emptyColor, _emptyColorDefault);
        #endregion

        #region Label
        private const string _showLabel = "showLabel";
        private const bool _showLabelDefault = false;
        public bool IsShowLabel => GetValue(_showLabel, _showLabelDefault);

        private const string _labelFontSize = "labelFontSize";
        private const double _labelFontSizeDefault = 5;
        public double LabelFontSize => GetValue(_labelFontSize, _labelFontSizeDefault);

        private const string _labelTextColor = "TextColor";
        private readonly Color _labelTextColorDefault = Color.Black;
        public Color LabelTextColor => GetValue(_labelTextColor, _labelTextColorDefault);

        private const string _labelBackgroundColor = "BackGroundColor";
        private readonly Color _labelDackgroundColorDefault = Color.WhiteSmoke;
        public Color LabelBackGroundColor => GetValue(_labelBackgroundColor, _labelDackgroundColorDefault);

        private const string _labelBoundaryColor = "BoundaryColor";
        private readonly Color _labelBoundaryColorDefault = Color.FromArgb(30, 30, 30);
        public Color LabelBoundaryColor => GetValue(_labelBoundaryColor, _labelBoundaryColorDefault);

        #endregion

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
            WireConnectRenderItem.Owner = this;

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

                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Lebel Font Size", "设置选中时连线宽度" }),
                        GetTransLation(new string[] { "Set Lebel Font Size", "设置选中时连线宽度" }),
                        ArchiTed_Grasshopper.Properties.Resources.SizeIcon, true, _labelFontSizeDefault, 3, 20, _labelFontSize);

                   WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[] {

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Text Color", "文字颜色" }),GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" }),
                    null, true, _labelTextColorDefault, _labelTextColor),

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Background Color", "背景颜色" }), GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" }),
                    null, true, _labelDackgroundColorDefault, _labelBackgroundColor),

                    new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Boundary Color", "边框颜色" }),
                            GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" }), null, true,
                            _labelBoundaryColorDefault, _labelBoundaryColor),
                    };
                   WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Colors", "颜色" }),
                   GetTransLation(new string[] { "Adjust color.", "调整颜色。" }), ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);

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

        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem exceptionsItem = new ToolStripMenuItem(GetTransLation(new string[] { "WireColors", "连线颜色" }), Properties.Resources.ExceptionIcon, exceptionClick);
            exceptionsItem.ToolTipText = GetTransLation(new string[] { "A window to set the wire's color.", "可以设置连线颜色的窗口。" });
            exceptionsItem.Font = GH_FontServer.StandardBold;
            exceptionsItem.ForeColor = Color.FromArgb(19, 34, 122);

            void exceptionClick(object sender, EventArgs e)
            {
                CreateWindow();
            }
            menu.Items.Add(exceptionsItem);

            GH_DocumentObject.Menu_AppendSeparator(menu);

            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Selected Wire Thickness", "设置选中时连线宽度" }),
                GetTransLation(new string[] { "Set Selected Wire Thickness", "设置选中时连线宽度" }),
                ArchiTed_Grasshopper.Properties.Resources.TextIcon, true, _selectWireThicknessDefault, 3, 20, _selectWireThickness);

            WinFormPlus.AddLoopBoexItem(menu, this, GetTransLation(new string[] { "Wire Type", "连线类型" }), true, new string[]
            {
                GetTransLation(new string[]{ "Bezier Curve", "贝塞尔曲线"}),
                GetTransLation(new string[]{ "PolyLine", "多段线"}),
                GetTransLation(new string[]{ "Line", "直线"}),
            }, _wireTypeDefault, _wireType);

            WinFormPlus.AddLoopBoexItem(menu, this, GetTransLation(new string[] { "Accuracy", "数据精度" }), true, new string[]
            {
                GetTransLation(new string[]{ "Rough", "粗糙"}),
                GetTransLation(new string[]{ "Medium", "中等"}),
                GetTransLation(new string[]{ "High", "高精"}),
            }, _accuracyDefault, _accuracy);

            WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[]
            {

                new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Default Wire Color", "默认连线颜色" }),
                    GetTransLation(new string[] { "Adjust default wire color.", "调整默认连线颜色。" }),
                    null, true, _defaultColorDefault, _defaultColor),

                new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Selected Wire Color", "选中连线颜色" }),
                    GetTransLation(new string[] { "Adjust selected wire color.", "调整选中连线颜色。" }),
                    null, true, _selectedColorDefault, _selectedColor),

                new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Unselected Wire Color", "未选中连线颜色" }),
                    GetTransLation(new string[] { "Adjust unselected wire color.", "调整未选中连线颜色。" }),
                    null, true, _unselectColorDefalut, _unselectColor),

                new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Empty Wire Color", "空连线颜色" }),
                    GetTransLation(new string[] { "Adjust empty wire color.", "调整空连线颜色。" }),
                    null, true, _emptyColorDefault, _emptyColor),
            };
            WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Wire Colors", "连线颜色" }),
            GetTransLation(new string[] { "Adjust wire color.", "调整连线颜色。" }), ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);

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