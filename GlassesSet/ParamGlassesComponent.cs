/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

//数值存储以TypeFullName进行存储！

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
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using System.IO;
using System.Text;
using GH_IO.Serialization;
using System.Linq;

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
        private const double _selectWireThicknessDefault = 4;
        public double SelectWireThickness => GetValue(_selectWireThickness, _selectWireThicknessDefault);

        private const string _selectWireSolid = "selectWireSolid";
        private const int _selectWireSolidDefault = 255;
        public int SelectWireSolid => GetValue(_selectWireSolid, _selectWireSolidDefault);

        private const string _wireType = "wireType";
        private const int _wireTypeDefault = 0;
        public int WireType => GetValue(_wireType, _wireTypeDefault);

        private const string _polywireParam = "polywireParam";
        private const double _polywireParamDefault = 0.5;
        public double PolywireParam => GetValue(_polywireParam, _polywireParamDefault);

        private const string _accuracy = "accuracy";
        private const int _accuracyDefault = 1;
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

        private const string _labelTextColor = "labelTextColor";
        private readonly Color _labelTextColorDefault = Color.Black;
        public Color LabelTextColor => GetValue(_labelTextColor, _labelTextColorDefault);

        private const string _labelBackgroundColor = "labelBackGroundColor";
        private readonly Color _labelDackgroundColorDefault = Color.FromArgb(150, Color.WhiteSmoke);
        public Color LabelBackGroundColor => GetValue(_labelBackgroundColor, _labelDackgroundColorDefault);

        private const string _labelBoundaryColor = "labelBoundaryColor";
        private readonly Color _labelBoundaryColorDefault = Color.FromArgb(30, 30, 30);
        public Color LabelBoundaryColor => GetValue(_labelBoundaryColor, _labelBoundaryColorDefault);

        #endregion
        #region Tree
        private const string _showTree = "showTree";
        private const bool _showTreeDefault = true;
        public bool IsShowTree => GetValue(_showTree, _showTreeDefault);

        private const string _treeCount = "treeCount";
        private const int _treeCountDefault = 10;
        public int TreeCount => GetValue(_treeCount, _treeCountDefault);
        #endregion

        #region Legend
        private const string _showLegend = "showLegend";
        private const bool _showLegendDefault = false;
        public bool IsShowLegend => GetValue(_showLegend, _showLegendDefault);

        private const string _legendLocation = "legendLocation";
        private const int _legendLocationDefault = 2;
        public int LegendLocation => GetValue(_legendLocation, _legendLocationDefault);

        private const string _legendSize = "legendSize";
        private const double _legendSizeDefault = 20;
        public double LegendSize => GetValue(_legendSize, _legendSizeDefault);

        private const string _legendSpacing = "legendSpacing";
        private const double _legendSpacingDefault = 30;
        public double LegendSpacing => GetValue(_legendSpacing, _legendSpacingDefault);

        private const string _legendTextColor = "legendTextColor";
        private readonly Color _legendTextColorDefault = Color.Black;
        public Color LegendTextColor => GetValue(_legendTextColor, _legendTextColorDefault);

        private const string _legendBackgroundColor = "legendBackGroundColor";
        private readonly Color _legendDackgroundColorDefault = Color.WhiteSmoke;
        public Color LegendBackGroundColor => GetValue(_legendBackgroundColor, _legendDackgroundColorDefault);

        private const string _legendBoundaryColor = "labelBoundaryColor";
        private readonly Color _legendBoundaryColorDefault = Color.FromArgb(30, 30, 30);
        public Color LegendBoundaryColor => GetValue(_legendBoundaryColor, _legendBoundaryColorDefault);

        #endregion

        private const string _showControl = "showControl";
        private const bool _showControlDefault = true;
        public bool IsShowControl => GetValue(_showControl, _showControlDefault);

        private const string _showBoolControl = "showBoolControl";
        private const bool _showBoolControlDefault = true;
        public bool IsShowBoolControl => GetValue(_showBoolControl, _showBoolControlDefault);

        private const string _showColorControl = "showColorControl";
        private const bool _showColorControlDefault = true;
        public bool IsShowColorControl => GetValue(_showColorControl, _showColorControlDefault);

        private const string _showDoubleControl = "showDoubleControl";
        private const bool _showDoubleControlDefault = true;
        public bool IsShowDoubleControl => GetValue(_showDoubleControl, _showDoubleControlDefault);

        private const string _showIntControl = "showIntControl";
        private const bool _showIntControlDefault = true;
        public bool IsShowIntControl => GetValue(_showIntControl, _showIntControlDefault);

        private const string _showStringControl = "showStringControl";
        private const bool _showStringControlDefault = true;
        public bool IsShowStringControl => GetValue(_showStringControl, _showStringControlDefault);

        private const string _showOtherControl = "showOtherControl";
        private const bool _showOtherControlDefault = true;
        public bool IsShowOtherControl => GetValue(_showOtherControl, _showOtherControlDefault);
        #endregion

        private bool _run = true;
        private bool _isFirst = true;

        private List<GooTypeProxy> _allParamProxy;
        public List<GooTypeProxy> AllParamProxy
        {
            get
            {
                if (_allParamProxy == null)
                {
                    UpdateAllParamProxy();
                }
                return _allParamProxy;
            }
            set { _allParamProxy = value; }
        }

        private List<ParamGlassesProxy> _allProxy;
        public List<ParamGlassesProxy> AllProxy
        {
            get
            {
                if (_allProxy == null)
                {
                    UpdateAllProxy();
                }
                return _allProxy;
            }
            set { _allProxy = value; }
        }

        /// <summary>
        /// For Legend Render
        /// </summary>
        public List<GooTypeProxy> ShowProxy { get; internal set; }


        #region ParamSettings Control
        public Dictionary<string, AddProxyParams[]> CreateProxyDictInput { get; set; }

        public Dictionary<string, AddProxyParams[]> CreateProxyDictOutput { get; set; }

        public Dictionary<Guid, string> ProxyReplaceDictInput { get; set; }


        public Dictionary<string, Color> ColorDict { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the ParamGlassesComponent class.
        /// </summary>
        public ParamGlassesComponent()
            : base(GetTransLation(new string[] { "ParamGlasses", "参数眼镜" }), GetTransLation(new string[] { "Param", "参数" }),
                 GetTransLation(new string[] { "To show the wire's and parameter's advances information.Right click or double click to have advanced options.",
                     "显示连线或参数的高级信息。右键或者双击可以获得更多选项。" }), "Params", "Showcase Tools", windowsType: typeof(ParamSettingsWindow))

        {
            LanguageChanged += ResponseToLanguageChanged;
            ResponseToLanguageChanged(this, new EventArgs());
            ShowProxy = new List<GooTypeProxy>();
            //For test
            ProxyReplaceDictInput = new Dictionary<Guid, string>()
            {
                {new Guid("{51a2ede9-8f8c-4fdf-a375-999c2062eab7}"), "Grasshopper.Kernel.Types.GH_Integer" },
                {new Guid("{bc984576-7aa6-491f-a91d-e444c33675a7}"), "Grasshopper.Kernel.Types.GH_Number" },
            };


            int width = 24;

            PointF changeInput;
            var inFuncs = WinformControlHelper.GetInnerRectLeftFunc(1, 2, new SizeF(width, width), out changeInput);
            this.InputLayoutMove = changeInput;

            PointF changeOutput;
            var outFuncs = WinformControlHelper.GetInnerRectRightFunc(1, 1, new SizeF(width, width), out changeOutput);
            this.OutputLayoutMove = changeOutput;

            ClickButtonIcon<LangWindow> LabelButton = new ClickButtonIcon<LangWindow>(_showLabel, this, inFuncs(1), true, Properties.Resources.LabelIcon, _showLabelDefault,
               tips: new string[] { "Click to choose whether to show the wire's label.", "点击以选择是否要显示连线的名称。" },
               createMenu: () =>
               {
                   ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                   WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Label Options", "标签选项" }));

                   WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Lebel Font Size", "气泡框中字体大小" }),
                    GetTransLation(new string[] { "Set Lebel Font Size", "设置气泡框中字体大小" }),
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


                   WinFormPlus.AddLoopBoexItem(menu, this, GetTransLation(new string[] { "Accuracy", "数据精度" }), true, new string[]
                   {
                        GetTransLation(new string[]{ "Rough", "粗糙"}),
                        GetTransLation(new string[]{ "Medium", "中等"}),
                        GetTransLation(new string[]{ "High", "高精"}),
                        GetTransLation(new string[]{ "Extrem High", "超高精"}),
                   }, _accuracyDefault, _accuracy);

                   GH_DocumentObject.Menu_AppendSeparator(menu);

                   WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Show Data Structure", "展示数据结构" }),
                       LanguagableComponent.GetTransLation(new string[] { "Click to switch whether to show the wire's data structure", "点击以选择是否要显示连线的数据结构。" }),
                       Properties.Resources.ShowTreeStructure, this, _showTree, _showTreeDefault);

                   WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Data Tree Count", "树型数据显示长度" }),
                        GetTransLation(new string[] { "Set Data Tree Count", "设置树型数据显示长度" }),
                        ArchiTed_Grasshopper.Properties.Resources.SizeIcon, this.IsShowTree, _treeCountDefault, 1, 50, _treeCount);

                   return menu;
               });

            ClickButtonIcon<LangWindow> LegendButton = new ClickButtonIcon<LangWindow>(_showLegend, this, inFuncs(0), true, Properties.Resources.LegendIcon, _showLegendDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                    WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Legend Options", "图例选项" }));

                    WinFormPlus.AddLoopBoexItem(menu, this, GetTransLation(new string[] { "Legend Location", "图例位置" }), true, new string[]
                    {
                        GetTransLation(new string[] { "Left Top", "左上角" }),
                        GetTransLation(new string[] { "Left Buttom", "左下角" }),
                        GetTransLation(new string[] { "Right Buttom", "右下角" }),
                        GetTransLation(new string[] { "Right Top", "右上角" }),
                    }, _legendLocationDefault, _legendLocation, new Bitmap[] 
                    { 
                        Properties.Resources.LeftTopIcon,
                        Properties.Resources.LeftBottomIcon,
                        Properties.Resources.RightBottomIcon,
                        Properties.Resources.RightTopIcon,
                    });

                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Legend Size", "图例大小" }),
                        GetTransLation(new string[] { "Set Legend Size", "设置图例大小" }),
                        ArchiTed_Grasshopper.Properties.Resources.SizeIcon, true, _legendSizeDefault, 10, 100, _legendSize);

                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Legend Spacing", "图例间距" }),
                        GetTransLation(new string[] { "Set Legend Spacing to Border", "设置图例到窗体边缘的距离" }),
                        ArchiTed_Grasshopper.Properties.Resources.DistanceIcon, true, _legendSpacingDefault, 0, 200, _legendSpacing);

                    WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[] {

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Text Color", "文字颜色" }),GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" }),
                    null, true, _legendTextColorDefault, _legendTextColor),

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Background Color", "背景颜色" }), GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" }),
                    null, true, _legendDackgroundColorDefault, _legendBackgroundColor),

                    new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Boundary Color", "边框颜色" }),
                            GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" }), null, true,
                            _legendBoundaryColorDefault, _legendBoundaryColor),
                    };
                    WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Colors", "颜色" }),
                    GetTransLation(new string[] { "Adjust color.", "调整颜色。" }), ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);

                    return menu;
                });

            ClickButtonIcon<LangWindow> ControlButton = new ClickButtonIcon<LangWindow>(_showControl, this, outFuncs(0), true, Properties.Resources.InputLogo, _showControlDefault,
                tips: new string[] { "Click to choose whether to show the param's control.", "点击以选择是否要显示参数的控制项。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                    WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Control Options", "控制选项" }));

                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Bool Control", "布尔控制项" }), null, null, this, _showBoolControl, _showBoolControlDefault);
                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Colour Control", "颜色控制项" }), null, null, this, _showColorControl, _showColorControlDefault);
                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Number Control", "数值控制项" }), null, null, this, _showDoubleControl, _showDoubleControlDefault);
                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Int Control", "整数控制项" }), null, null, this, _showIntControl, _showIntControlDefault);
                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Text Control", "文字控制项" }), null, null, this, _showStringControl, _showStringControlDefault);
                    WinFormPlus.AddCheckBoxItem(menu, LanguagableComponent.GetTransLation(new string[] { "Other Control", "其他控制项" }), null, null, this, _showOtherControl, _showOtherControlDefault);

                    return menu;
                });

            this.Controls = new IRespond[] { LabelButton, LegendButton, ControlButton};
        }

        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem exceptionsItem = new ToolStripMenuItem(GetTransLation(new string[] { "ParamSettings", "参数设定" }), Properties.Resources.ExceptionIcon, exceptionClick);
            exceptionsItem.ToolTipText = GetTransLation(new string[] { "A window to set the wire's color, input controls and output controls.", "可以设置连线颜色、输入控制项、输出控制项的窗口。" });
            exceptionsItem.Font = GH_FontServer.StandardBold;
            exceptionsItem.ForeColor = Color.FromArgb(19, 34, 122);

            void exceptionClick(object sender, EventArgs e)
            {
                CreateWindow();
            }
            menu.Items.Add(exceptionsItem);

            GH_DocumentObject.Menu_AppendSeparator(menu);



            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Selected Wire Thickness Plus", "选中时连线宽度增值" }),
                GetTransLation(new string[] { "Set Selected Wire Thickness Plus", "设置选中时连线宽度增值" }),
                ArchiTed_Grasshopper.Properties.Resources.SizeIcon, true, _selectWireThicknessDefault, 0, 20, _selectWireThickness);

            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Selected Wire Color Alpha Plus", "选中时连线颜色ALpha通道增量" }),
                GetTransLation(new string[] { "Set Selected Wire Color Alpha Plus", "选中时连线颜色ALpha通道增量" }),
                ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, _selectWireSolidDefault, -255, 255, _selectWireSolid);

            GH_DocumentObject.Menu_AppendSeparator(menu);

            WinFormPlus.AddLoopBoexItem(menu, this, GetTransLation(new string[] { "Wire Type", "连线类型" }), true, new string[]
            {
                GetTransLation(new string[]{ "Bezier Curve", "贝塞尔曲线"}),
                GetTransLation(new string[]{ "PolyLine", "多段线"}),
                GetTransLation(new string[]{ "Line", "直线"}),
            }, _wireTypeDefault, _wireType);

            if(WireType == 1)
            {
                WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "    PolyLine Param", "    多段线参数" }),
                    GetTransLation(new string[] { "Click to set the polyline wire param.", "点击以修改多段线的参数。" }),
                    null, true, _polywireParamDefault, 0, 0.5, _polywireParam);
            }


            WinFormPlus.ItemSet<Color>[] sets2 = new WinFormPlus.ItemSet<Color>[]
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
            GetTransLation(new string[] { "Adjust wire color.", "调整连线颜色。" }), ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets2);

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

        #region Algrithm
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref _run);
            RemoveAll();
            RemoveRespond();

            if (_isFirst)
            {
                if(ColorDict == null)
                {
                    ReadColorTxt();
                }
                if(CreateProxyDictInput == null)
                {
                    ReadInputTxt();
                }
                if(CreateProxyDictOutput == null)
                {
                    ReadOutputTxt();
                }

                _isFirst = false;
            }
            for (int i = 0; i < this.Controls.Length - 1; i++)
            {
                this.Controls[i].Enable = false;
            }


            if (_run)
            {
                foreach (var item in this.Controls)
                {
                    item.Enable = true;
                }
                AddRespond();
            }
            {
                foreach (var obj in this.OnPingDocument().Objects)
                {
                    this.AddOneObject(obj);
                }
                Grasshopper.Instances.ActiveCanvas.Refresh();

                if (this.IsShowLegend)
                {
                    this.RenderObjs.Add(new ParamLegend(this));
                }
            }
        }



        #region Add & Remove for documentObject
        /// <summary>
        /// Add a new object into this component.
        /// </summary>
        /// <param name="obj"></param>
        private void AddOneObject(IGH_DocumentObject obj)
        {
            if(obj is IGH_Param)
            {
                IGH_Param param = obj as IGH_Param;
                if (param.Attributes.HasInputGrip)
                {
                    AddOneParamInput(param);
                }
                if (param.Attributes.HasOutputGrip)
                {
                    AddOneParamOutput(param);
                }
            }
            else if(obj is IGH_Component)
            {
                IGH_Component com = obj as IGH_Component;
                bool addControl = obj != this;
                foreach (IGH_Param param in com.Params.Input)
                {
                    AddOneParamInput(param, addControl);
                }
                foreach (IGH_Param param in com.Params.Output)
                {
                    AddOneParamOutput(param);
                }
                if (com is IGH_VariableParameterComponent)
                {
                    com.AttributesChanged -= VariableComponentAttributesChanged;
                    com.AttributesChanged += VariableComponentAttributesChanged;
                }
            }
        }



        private void RemoveOneObject(IGH_DocumentObject obj)
        {
            if (obj is IGH_Param)
            {
                IGH_Param param = obj as IGH_Param;
                if (param.Attributes.HasInputGrip)
                {
                    RemoveOneParam(param, true);
                }
                if (param.Attributes.HasOutputGrip)
                {
                    RemoveOneParam(param, false);
                }
            }
            else if (obj is IGH_Component)
            {
                IGH_Component com = obj as IGH_Component;
                foreach (IGH_Param param in com.Params.Input)
                {
                    RemoveOneParam(param, true);
                }
                foreach (IGH_Param param in com.Params.Output)
                {
                    RemoveOneParam(param, false);
                }
                if (com is IGH_VariableParameterComponent)
                {
                    com.AttributesChanged -= VariableComponentAttributesChanged;
                }
            }
        }

        private void RemoveOneParam(IGH_Param param, bool isInputSide)
        {
            while (true)
            {
                bool findit = false;
                foreach (var item in this.RenderObjs)
                {
                    //bool isOutputParam = item.GetType().GetGenericTypeDefinition() == typeof(ButtonAddObjectOutput<>);

                    var result = item.GetType().GetProperty("Target");
                    if (result != null)
                    {
                        var prop = result.GetValue(item);
                        if (prop == param)
                        {
                            //if((isOutputParam && !isInputSide) || (!isOutputParam && isInputSide))
                            {
                                if (item is IDisposable)
                                {
                                    ((IDisposable)item).Dispose();
                                }
                                this.RenderObjs.Remove(item);
                                findit = true;
                                break;
                            }

                        }
                    }
                }
                if (!findit) break;
            }

        }
        private void AddOneParamOutput(IGH_Param param)
        {
            if (!this.IsShowControl) return;

            if (IsPersistentParam(param.GetType(), out _)&& param.Attributes.HasOutputGrip && this.IsShowOtherControl)
            {
                Type paramType = typeof(ButtonAddObjectOutput<>).MakeGenericType(param.Type);
                this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, null, true, false));
            }
        }

        private void AddOneParamInput(IGH_Param param, bool addControl = true)
        {
            if (this._run)
                this.RenderObjs.Add(new WireConnectRenderItem(param, this));
            if (!this.IsShowControl || !addControl) return;
            Type type = param.Type;

            if (IsPersistentParam(param.GetType(), out _))
            {

                if (this.IsShowBoolControl && typeof(GH_Goo<bool>).IsAssignableFrom(type))
                {
                    Type paramType = typeof(CheckBoxParam<>).MakeGenericType(type);
                    this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, null, true, false));
                    return;
                }
                else if (this.IsShowColorControl && typeof(GH_Goo<Color>).IsAssignableFrom(type))
                {
                    Type paramType = typeof(ColourSwatchParam<>).MakeGenericType(type);
                    this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 1000, false));
                    return;
                }
                else if (this.IsShowDoubleControl && typeof(GH_Goo<double>).IsAssignableFrom(type))
                {
                    Type paramType = typeof(InputBoxDoubleParam<>).MakeGenericType(type);
                    this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, false));
                    return;
                }
                else if (this.IsShowIntControl && typeof(GH_Goo<int>).IsAssignableFrom(type))
                {
                    Type paramType = typeof(InputBoxIntParam<>).MakeGenericType(type);
                    this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, false));
                    return;
                }
                else if (this.IsShowStringControl && typeof(GH_Goo<string>).IsAssignableFrom(type))
                {
                    Type paramType = typeof(InputBoxStringParam<>).MakeGenericType(type);
                    this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, false));
                    return;
                }

            }
            if (param.Attributes.HasInputGrip && this.IsShowOtherControl)
            {
                Type paramType = typeof(ButtonAddObjectInput<>).MakeGenericType(type);
                this.RenderObjs.Add((IRenderable)Activator.CreateInstance(paramType, param, this, true, null, 5000, null, true, false));
                return;
            }
        }

        private void RemoveAll()
        {
            if (this.RenderObjs != null)
            {
                foreach (var item in this.RenderObjs)
                {
                    if (item is IDisposable)
                    {
                        var dispose = item as IDisposable;
                        dispose.Dispose();
                    }
                }
            }
            this.RenderObjs = new List<IRenderable>();
            this.RenderObjsUnderComponent = new List<IRenderable>();
        }

        void VariableComponentAttributesChanged(IGH_DocumentObject sender, GH_AttributesChangedEventArgs e)
        {
            foreach (IGH_Param param in ((IGH_Component)sender).Params.Input)
            {
                RemoveOneParam(param, true);
                AddOneParamInput(param);
            }
            foreach (IGH_Param param in ((IGH_Component)sender).Params.Output)
            {
                RemoveOneParam(param, false);
                AddOneParamOutput(param);
            }
        }
        #endregion



        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires -= ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;

            if (sender.Document == this.OnPingDocument())
            {
                Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires += ActiveCanvas_CanvasPrePaintWires;
                Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            }
        }

        private void ObjectsAddedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                AddOneObject(docObj);
            }
        }

        private void ObjectsDeletedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docobj in e.Objects)
            {
                RemoveOneObject(docobj);
            }
        }

        private void ActiveCanvas_CanvasPostPaintWires(GH_Canvas sender)
        {
            SetDefaultColor();
        }

        private void ActiveCanvas_CanvasPrePaintWires(GH_Canvas sender)
        {
            SetTranslateColor();
        }

        #region For Proxy
        private void UpdateAllParamProxy()
        {
            _allParamProxy = new List<GooTypeProxy>();
            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (!proxy.Obsolete && proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    try
                    {
                        //IGH_DocumentObject obj = proxy.CreateInstance();
                        Type dataType;
                        if (IsPersistentParam(proxy.Type, out dataType))
                        {
                            #region Check if has this type before
                            bool flag = false;
                            foreach (var item in this._allParamProxy)
                            {
                                if (item.TypeFullName == dataType.FullName)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag) continue;
                            #endregion

                            _allParamProxy.Add(new GooTypeProxy(dataType, this));
                        }
                    }
                    catch
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, proxy.Desc.Name +
                            GetTransLation(new string[] { " failed to create!", "创建失败！" }));
                    }

                }
            }
        }

        public void UpdateAllProxy()
        {
            _allProxy = new List<ParamGlassesProxy>();
            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (!proxy.Obsolete && proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    _allProxy.Add(new ParamGlassesProxy(proxy));
                }
            }
        }
        #endregion
        public bool IsPersistentParam(Type type, out Type dataType)
        {
            dataType = default(Type);
            if (type == null)
            {
                return false;
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(GH_PersistentParam<>))
                {
                    dataType = type.GenericTypeArguments[0];
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(GH_Param<>))
                    return false;
            }
            return IsPersistentParam(type.BaseType, out dataType);
        }

        #endregion

        #region After Algrithm
        #region Lanuage & Window
        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            string[] input = new string[] { GetTransLation(new string[] { "Run Wire Colour", "启动曲线颜色" }), GetTransLation(new string[] { "R", "启动" }), GetTransLation(new string[] { "Run Wire Colour", "启动启动曲线颜色" }) };

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

        #endregion
        private void AddRespond()
        {
            this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;
            this.OnPingDocument().ObjectsDeleted += ObjectsDeletedAction;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires += ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
        }

        private void RemoveRespond()
        {
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires -= ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;
            try
            {
                this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted -= ObjectsDeletedAction;
            }
            catch
            {

            }
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            LanguageChanged -= ResponseToLanguageChanged;
            RemoveAll();
            SetDefaultColor();
            RemoveRespond();

            base.RemovedFromDocument(document);
        }
        #region Get & Set
        private void SetTranslateColor()
        {
            GH_Skin.wire_default = Color.Transparent;
            GH_Skin.wire_empty = Color.Transparent;
            GH_Skin.wire_selected_a = Color.Transparent;
            GH_Skin.wire_selected_b = Color.Transparent;
        }

        private void SetDefaultColor()
        {
            GH_Skin.wire_default = this.DefaultColor;
            GH_Skin.wire_empty = this.EmptyColor;
            GH_Skin.wire_selected_a = this.SelectedColor;
            GH_Skin.wire_selected_b = this.UnselectColor;
        }

        internal void SetColor(string name, Color color)
        {
            ColorDict[name] = color;
        }

        internal Color GetColor(string name)
        {
            try
            {
                return ColorDict[name];
            }
            catch
            {
                return this.DefaultColor;
            }
        }

        internal void SetCreateProxyGuid(string name, AddProxyParams[] proxies)
        {
            CreateProxyDictInput[name] = proxies;
        }

        internal AddProxyParams[] GetCreateProxyGuid(string name)
        {
            try
            {
                return CreateProxyDictInput[name];
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region IO for .txt

        #region OutputControl
        public string WriteOutputTxt(string path)
        {
            IO_Helper.WriteString(path, () =>
            {
                string saveStr = "";

                foreach (var item in CreateProxyDictOutput)
                {
                    string proxies = "";
                    foreach (var proxy in item.Value)
                    {
                        proxies += proxy.Guid + ",";
                        proxies += proxy.Index + ";";
                    }
                    saveStr += item.Key + ';' + proxies + "\n";
                }
                return saveStr;
            });

            return IO_Helper.GetWriteMessage(path);
        }

        internal string WriteOutputTxt()
        {
            string name = "OutputControl_Default";
            string path = IO_Helper.GetNamedPath(this, name, create: true);
            return WriteOutputTxt(path);
        }

        public string ReadOutputTxt(string path)
        {
            CreateProxyDictOutput = new Dictionary<string, AddProxyParams[]>();
            int successCount = 0;
            int failCount = 0;

            IO_Helper.ReadFileInLine(path, (str, index) =>
            {
                string[] strs = str.Split(';');
                if (strs.Length <= 2)
                {
                    failCount++;
                    return;
                }

                List<AddProxyParams> proxies = new List<AddProxyParams>();
                for (int i = 1; i < strs.Length; i++)
                {
                    string[] strCom = strs[i].Split(',');
                    if (strCom.Length != 2)
                    {
                        continue;
                    }
                    try
                    {
                        proxies.Add(new AddProxyParams(new Guid(strCom[0]), byte.Parse(strCom[1])));
                    }
                    catch { }
                }

                CreateProxyDictOutput[strs[0]] = proxies.ToArray();
                successCount++;
            });

            return IO_Helper.GetReadMessage(successCount, failCount);
        }
        private string ReadOutputTxt()
        {
            string name = "OutputControl_Default";
            string path = IO_Helper.GetNamedPath(this, name);
            return ReadOutputTxt(path);
        }
        #endregion

        #region InputControl
        public string WriteInputTxt(string path)
        {
            IO_Helper.WriteString(path, () =>
            {
                string saveStr = "";

                foreach (var item in CreateProxyDictInput)
                {
                    if (item.Value.Length == 0) continue;
                    string proxies = "";
                    foreach (var proxy in item.Value)
                    {
                        proxies += proxy.Guid + ",";
                        proxies += proxy.Index + ";";
                    }
                    saveStr += item.Key + ';' + proxies + "\n";
                }
                return saveStr;
            });

            return IO_Helper.GetWriteMessage(path);
        }

        internal string WriteInputTxt()
        {
            string name = "InputControl_Default";
            string path = IO_Helper.GetNamedPath(this, name, create: true);
            return WriteInputTxt(path);
        }

        public string ReadInputTxt(string path)
        {
            CreateProxyDictInput = new Dictionary<string, AddProxyParams[]>();
            int successCount = 0;
            int failCount = 0;

            IO_Helper.ReadFileInLine(path, (str, index) =>
            {
                string[] strs = str.Split(';');
                if (strs.Length <= 2)
                {
                    failCount++;
                    return;
                }

                List<AddProxyParams> proxies = new List<AddProxyParams>();
                for (int i = 1; i < strs.Length; i++)
                {
                    string[] strCom = strs[i].Split(',');
                    if(strCom.Length != 2)
                    {
                        continue;
                    }
                    try
                    {
                        proxies.Add(new AddProxyParams(new Guid(strCom[0]), byte.Parse(strCom[1])));
                    }
                    catch { }
                }

                CreateProxyDictInput[strs[0]] = proxies.ToArray();
                successCount++;
            });

            return IO_Helper.GetReadMessage(successCount, failCount);
        }
        private string ReadInputTxt()
        {
            string name = "InputControl_Default";
            string path = IO_Helper.GetNamedPath(this, name);
            return ReadInputTxt(path);
        }
        #endregion

        #region WireColor
        public string WriteColorTxt(string path)
        {
            IO_Helper.WriteString(path, () =>
            {
                string saveStr = "";

                foreach (var item in ColorDict.Keys)
                {
                    saveStr += item.ToString() + ',' + ColorDict[item].ToArgb().ToString() + "\n";
                }
                return saveStr;
            });

            return IO_Helper.GetWriteMessage(path);
        }

        internal string WriteColorTxt()
        {
            string name = "WireColors_Default";
            string path = IO_Helper.GetNamedPath(this, name, create: true);
            return  WriteColorTxt(path);
        }

        public string ReadColorTxt(string path)
        {
            ColorDict = new Dictionary<string, Color>();
            int successCount = 0;
            int failCount = 0;

            IO_Helper.ReadFileInLine(path, (str, index) =>
            {
                string[] strs = str.Split(',');
                if(strs.Length != 2)
                {
                    failCount++;
                    return;
                }
                try
                {
                    this.SetColor(strs[0], Color.FromArgb(int.Parse(strs[1])));
                    successCount++;
                }
                catch
                {
                    failCount++;
                }
            });

            return IO_Helper.GetReadMessage(successCount, failCount);
        }

        private string ReadColorTxt()
        {
            string name = "WireColors_Default";
            string path = IO_Helper.GetNamedPath(this, name);
            return ReadColorTxt(path);
        }
        #endregion
        #endregion

        #region Write & Read
        public override bool Write(GH_IWriter writer)
        {
            //Write Color
            if(ColorDict.Count != 0)
            {
                writer.SetInt32("ColorCount", ColorDict.Count);
                int n = 0;
                foreach (string key in ColorDict.Keys)
                {
                    writer.SetString("colorName" + n.ToString(), key);
                    writer.SetDrawingColor("color" + n.ToString(), ColorDict[key]);
                    n++;
                }
            }

            //WieteCreate Input
            if (CreateProxyDictInput.Count != 0)
            {
                writer.SetInt32("AutoAddCount", CreateProxyDictInput.Count);
                int n = 0;
                foreach (string key in CreateProxyDictInput.Keys)
                {
                    writer.SetString("autoName" + n.ToString(), key);

                    int valueCount = CreateProxyDictInput[key].Length;
                    writer.SetInt32("autoValueCount" + n.ToString(), valueCount);
                    for (int m = 0; m < valueCount; m++)
                    {
                        var set = CreateProxyDictInput[key][m];
                        writer.SetGuid("autoValueGuid" + n.ToString("D5") + m.ToString(), set.Guid);
                        writer.SetByte("autoValueInt" + n.ToString("D5") + m.ToString(), set.Index);
                    }
                    n++;
                }
            }

            //WieteCreate Output
            if (CreateProxyDictOutput.Count != 0)
            {
                writer.SetInt32("AutoAddCountOut", CreateProxyDictOutput.Count);
                int n = 0;
                foreach (string key in CreateProxyDictOutput.Keys)
                {
                    writer.SetString("autoNameOut" + n.ToString(), key);

                    int valueCount = CreateProxyDictOutput[key].Length;
                    writer.SetInt32("autoValueCountOut" + n.ToString(), valueCount);
                    for (int m = 0; m < valueCount; m++)
                    {
                        var set = CreateProxyDictOutput[key][m];
                        writer.SetGuid("autoValueGuidOut" + n.ToString("D5") + m.ToString(), set.Guid);
                        writer.SetByte("autoValueIntOut" + n.ToString("D5") + m.ToString(), set.Index);
                    }
                    n++;
                }
            }

            //Write Replace
            if (ProxyReplaceDictInput.Count != 0)
            {
                writer.SetInt32("ReplaceCount", ProxyReplaceDictInput.Count);
                int n = 0;
                foreach (Guid key in ProxyReplaceDictInput.Keys)
                {
                    writer.SetGuid("ReplaceGuid" + n.ToString(), key);
                    writer.SetString("ReplaceName" + n.ToString(), ProxyReplaceDictInput[key]);
                    n++;
                }
            }

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            ColorDict = new Dictionary<string, Color>();
            CreateProxyDictInput = new Dictionary<string, AddProxyParams[]>();
            CreateProxyDictOutput = new Dictionary<string, AddProxyParams[]>();
            ProxyReplaceDictInput = new Dictionary<Guid, string>();

            //Read Color
            int colorCount = 0;
            if( reader.TryGetInt32("ColorCount", ref colorCount))
            {
                for (int n = 0; n < colorCount; n++)
                {
                    ColorDict[reader.GetString("colorName" + n.ToString())] =
                        reader.GetDrawingColor("color" + n.ToString());
                }
                
            }

            //Read Create Input
            int autoCount = 0;
            if (reader.TryGetInt32("AutoAddCount", ref autoCount))
            {
                for (int n = 0; n < autoCount; n++)
                {
                    int valueCount = reader.GetInt32("autoValueCount" + n.ToString());
                    AddProxyParams[] value = new AddProxyParams[valueCount];
                    for (int m = 0; m < valueCount; m++)
                    {
                        Guid guid = reader.GetGuid("autoValueGuid" + n.ToString("D5") + m.ToString());
                        int outIndex = reader.GetByte("autoValueInt" + n.ToString("D5") + m.ToString());
                        value[m] = new AddProxyParams(guid, (byte)outIndex);
                    }
                    CreateProxyDictInput[reader.GetString("autoName" + n.ToString())] = value;
                }
            }

            //Read Create Out
            int autoCountOut = 0;
            if (reader.TryGetInt32("AutoAddCountOut", ref autoCountOut))
            {
                for (int n = 0; n < autoCountOut; n++)
                {
                    int valueCount = reader.GetInt32("autoValueCountOut" + n.ToString());
                    AddProxyParams[] value = new AddProxyParams[valueCount];
                    for (int m = 0; m < valueCount; m++)
                    {
                        Guid guid = reader.GetGuid("autoValueGuidOut" + n.ToString("D5") + m.ToString());
                        int outIndex = reader.GetByte("autoValueIntOut" + n.ToString("D5") + m.ToString());
                        value[m] = new AddProxyParams(guid, (byte)outIndex);
                    }
                    CreateProxyDictOutput[reader.GetString("autoNameOut" + n.ToString())] = value;
                }
            }

            //Read Replace
            int replaceCount = 0;
            if(reader.TryGetInt32("ReplaceCount", ref replaceCount))
            {
                for (int n = 0; n < replaceCount; n++)
                {
                    Guid guid = reader.GetGuid("ReplaceGuid" + n.ToString());
                    string name = reader.GetString("ReplaceName" + n.ToString());
                    ProxyReplaceDictInput[guid] = name;
                }
            }

            return base.Read(reader);
        }
        #endregion
        #endregion

    }
}