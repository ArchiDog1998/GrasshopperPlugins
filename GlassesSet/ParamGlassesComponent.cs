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
using Grasshopper.GUI.Canvas;

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
        private const double _selectWireThicknessDefault = 2;
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
        private const bool _showLabelDefault = true;
        public bool IsShowLabel => GetValue(_showLabel, _showLabelDefault);

        private const string _labelFontSize = "labelFontSize";
        private const double _labelFontSizeDefault = 5;
        public double LabelFontSize => GetValue(_labelFontSize, _labelFontSizeDefault);

        private const string _labelTextColor = "labelTextColor";
        private readonly Color _labelTextColorDefault = Color.Black;
        public Color LabelTextColor => GetValue(_labelTextColor, _labelTextColorDefault);

        private const string _labelBackgroundColor = "labelBackGroundColor";
        private readonly Color _labelDackgroundColorDefault = Color.WhiteSmoke;
        public Color LabelBackGroundColor => GetValue(_labelBackgroundColor, _labelDackgroundColorDefault);

        private const string _labelBoundaryColor = "labelBoundaryColor";
        private readonly Color _labelBoundaryColorDefault = Color.FromArgb(30, 30, 30);
        public Color LabelBoundaryColor => GetValue(_labelBoundaryColor, _labelBoundaryColorDefault);

        #endregion

        private const string _showTree = "showTree";
        private const bool _showTreeDefault = false;
        public bool IsShowTree => GetValue(_showTree, _showTreeDefault);

        #region Legend
        private const string _showLegend = "showLegend";
        private const bool _showLegendDefault = true;
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
        private const bool _showControlDefault = false;
        public bool IsShowControl => GetValue(_showControl, _showControlDefault);


        #endregion

        private bool _run = true;
        private bool _isFirst = true;

        private List<ParamProxy> _allProxy;
        public List<ParamProxy> AllProxy
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

        public List<ParamProxy> ShowProxy { get; internal set; }

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
            ShowProxy = new List<ParamProxy>();

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

                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Lebel Font Size", "设置气泡框中字体大小" }),
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

                   return menu;
               });

            ClickButtonIcon<LangWindow> treeButton = new ClickButtonIcon<LangWindow>(_showTree, this, inFuncs(0), true, Properties.Resources.ShowTreeStructure, _showTreeDefault,
                tips: new string[] { "Click to switch whether to show the wire's data structure", "点击以选择是否要显示连线的数据结构。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

                    return menu;
                });


            ClickButtonIcon<LangWindow> LegendButton = new ClickButtonIcon<LangWindow>(_showLegend, this, outFuncs(0), true, Properties.Resources.LegendIcon, _showLegendDefault,
                tips: new string[] { "Click to choose whether to show the wire's legend.", "点击以选择是否要显示连线的图例。" },
                createMenu: () =>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };

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

            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Selected Wire Thickness Plus", "选中时连线宽度增值" }),
                GetTransLation(new string[] { "Set Selected Wire Thickness Plus", "设置选中时连线宽度增值" }),
                ArchiTed_Grasshopper.Properties.Resources.TextIcon, true, _selectWireThicknessDefault, 0, 20, _selectWireThickness);

            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Selected Wire Color Alpha Plus", "选中时连线颜色ALpha通道增量" }),
                GetTransLation(new string[] { "Set Selected Wire Color Alpha Plus", "选中时连线颜色ALpha通道增量" }),
                ArchiTed_Grasshopper.Properties.Resources.TextIcon, true, _selectWireSolidDefault, -255, 255, _selectWireSolid);

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
                    null, true, _polywireParamDefault, 0, 1, _polywireParam);
            }

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

        #region Algrithm
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref _run);
            this.RenderObjs = new List<IRenderable>();
            this.RenderObjsUnderComponent = new List<IRenderable>();

            this.OnPingDocument().ObjectsAdded -= InfeGlassesComponent_ObjectsAdded;

            if (_isFirst)
            {

                this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted += ObjectsDeletedAction;
                Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires += ActiveCanvas_CanvasPrePaintWires;
                Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
                Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;

                //GetProxy();
                //GetObject();

                //bool read = true;
                //foreach (var item in allParamType)
                //{
                //    if (GetColor(item) != defaultColor)
                //        read = false;
                //}
                //if (read)
                //    Readtxt();

                _isFirst = false;
            }

            if (_run)
            {
                foreach (var obj in this.OnPingDocument().Objects)
                {
                    this.AddOneObject(obj);
                }
                this.OnPingDocument().ObjectsAdded += InfeGlassesComponent_ObjectsAdded;
                Grasshopper.Instances.ActiveCanvas.Refresh();

                if (this.IsShowLegend)
                {
                    this.RenderObjs.Add(new ParamLegend(this));
                }
            }
        }

        private void InfeGlassesComponent_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (var obj in e.Objects)
            {
                this.AddOneObject(obj);
            }
        }

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
                    this.RenderObjs.Add(new WireConnectRenderItem(param));
                }
            }
            else if(obj is IGH_Component)
            {
                IGH_Component com = obj as IGH_Component;
                foreach (IGH_Param param in com.Params.Input)
                {
                    this.RenderObjs.Add(new WireConnectRenderItem(param));
                }
            }
        }

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

        }

        private void ActiveCanvas_CanvasPostPaintWires(GH_Canvas sender)
        {
            SetDefaultColor();
        }

        private void ActiveCanvas_CanvasPrePaintWires(GH_Canvas sender)
        {
            SetTranslateColor();
        }

        private void UpdateAllProxy()
        {
            _allProxy = new List<ParamProxy>();
            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (!proxy.Obsolete && proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    IGH_DocumentObject obj = proxy.CreateInstance();
                    if (IsPersistentParam(obj.GetType()))
                    {
                        _allProxy.Add(new ParamProxy((IGH_Param)obj, this.DefaultColor));
                    }
                }
            }
        }

        private bool IsPersistentParam(Type type)
        {
            if(type == null)
            {
                return false;
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(GH_PersistentParam<>))
                    return true;
                else if (type.GetGenericTypeDefinition() == typeof(GH_Param<>))
                    return false;
            }

            return IsPersistentParam(type.BaseType);
        }

        #endregion

        #region After Algrithm
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
            Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintWires -= ActiveCanvas_CanvasPrePaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;

            SetDefaultColor();


            try
            {
                this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted -= ObjectsDeletedAction;
            }
            catch
            {

            }
            base.RemovedFromDocument(document);
        }
        #endregion

    }
}