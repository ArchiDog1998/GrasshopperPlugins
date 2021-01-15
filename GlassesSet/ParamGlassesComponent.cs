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
        private const bool _showTreeDefault = false;
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

            //CreateProxyDictInput = new Dictionary<string, AddProxyParams[]>() 
            //{
            //    {"Grasshopper.Kernel.Types.GH_Point", new AddProxyParams[]{new AddProxyParams(new Guid("{3581F42A-9592-4549-BD6B-1C0FC39D067B}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Vector", new AddProxyParams[]{new AddProxyParams(new Guid("{56b92eab-d121-43f7-94d3-6cd8f0ddead8}"), 0) } },

            //    {"Grasshopper.Kernel.Types.GH_Circle",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{807b86e3-be8d-4970-92b5-f8cdcb45b06b}"), 0),
            //        new AddProxyParams(new Guid("{d114323a-e6ee-4164-946b-e4ca0ce15efa}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Arc", new AddProxyParams[]{new AddProxyParams(new Guid("{bb59bffc-f54c-4682-9778-f6c3fe74fce3}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Curve", new AddProxyParams[]{new AddProxyParams(new Guid("{2b2a4145-3dff-41d4-a8de-1ea9d29eef33}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Line", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{4c4e56eb-2f04-43f9-95a3-cc46a14f495a}"), 0),
            //        new AddProxyParams(new Guid("{4c619bc9-39fd-4717-82a6-1e07ea237bbe}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Plane", new AddProxyParams[]{new AddProxyParams(new Guid("{bc3e379e-7206-4e7b-b63a-ff61f4b38a3e}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Rectangle", new AddProxyParams[]{new AddProxyParams(new Guid("{d93100b6-d50b-40b2-831a-814659dc38e3}"), 0) } },

            //    {"Grasshopper.Kernel.Types.GH_Box", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{28061aae-04fb-4cb5-ac45-16f3b66bc0a4}"), 0),
            //        new AddProxyParams(new Guid("{79aa7f47-397c-4d3f-9761-aaf421bb7f5f}"), 0),
            //        new AddProxyParams(new Guid("{0bb3d234-9097-45db-9998-621639c87d3b}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Mesh",new AddProxyParams[]{ 
            //        new AddProxyParams(new Guid("{e2c0f9db-a862-4bd9-810c-ef2610e7a56f}"), 0),
            //        new AddProxyParams(new Guid("{60e7defa-8b21-4ee1-99aa-a9223d6134ff}"), 0),
            //        new AddProxyParams(new Guid("{58cf422f-19f7-42f7-9619-fc198c51c657}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_MeshFace", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{1cb59c86-7f6b-4e52-9a0c-6441850e9520}"), 0),
            //        new AddProxyParams(new Guid("{5a4ddedd-5af9-49e5-bace-12910a8b9366}"), 0), 
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_SubD", new AddProxyParams[]{new AddProxyParams(new Guid("{855a2c73-31c0-41d2-b061-57d54229d11b}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Surface", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{6e5de495-ba76-42d0-9985-a5c265e9aeca}"), 0),
            //        new AddProxyParams(new Guid("{c77a8b3b-c569-4d81-9b59-1c27299a1c45}"), 0),
            //        new AddProxyParams(new Guid("{36132830-e2ef-4476-8ea1-6a43922344f0}"), 0),
            //        new AddProxyParams(new Guid("{71506fa8-9bf0-432d-b897-b2e0c5ac316c}"), 0),
            //    } },
            //    {"SquishyXMorphs.GH_TwistedBox",new AddProxyParams[]{ new AddProxyParams(new Guid("{124de0f5-65f8-4ae0-8f61-8fb066e2ba02}"), 0) } },

            //    {"Grasshopper.Kernel.Types.GH_Field",new AddProxyParams[]{ 
            //        new AddProxyParams(new Guid("{d9a6fbd2-2e9f-472e-8147-33bf0233a115}"), 0),
            //        new AddProxyParams(new Guid("{8cc9eb88-26a7-4baa-a896-13e5fc12416a}"), 0),
            //        new AddProxyParams(new Guid("{cffdbaf3-8d33-4b38-9cad-c264af9fc3f4}"), 0),
            //        new AddProxyParams(new Guid("{4b59e893-d4ee-4e31-ae24-a489611d1088}"), 0),
            //        new AddProxyParams(new Guid("{d27cc1ea-9ef7-47bf-8ee2-c6662da0e3d9}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_GeometryGroup",new AddProxyParams[]{ new AddProxyParams(new Guid("{874eebe7-835b-4f4f-9811-97e031c41597}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Transform",new AddProxyParams[]{ 
            //        new AddProxyParams(new Guid("{407e35c6-7c40-4652-bd80-fde1eb7ec034}"), 1),
            //        new AddProxyParams(new Guid("{4d2a06bd-4b0f-4c65-9ee0-4220e4c01703}"), 1),
            //        new AddProxyParams(new Guid("{290f418a-65ee-406a-a9d0-35699815b512}"), 1),
            //        new AddProxyParams(new Guid("{5a27203a-e05f-4eea-b80f-a5f29a00fdf2}"), 1),
            //        new AddProxyParams(new Guid("{f19ee36c-f21f-4e25-be4c-4ca4b30eda0d}"), 1),
            //        new AddProxyParams(new Guid("{23285717-156c-468f-a691-b242488c06a6}"), 1),
            //        new AddProxyParams(new Guid("{06d7bc4a-ba3e-4445-8ab5-079613b52f28}"), 1),
            //        new AddProxyParams(new Guid("{f12daa2f-4fd5-48c1-8ac3-5dea476912ca}"), 1),
            //        new AddProxyParams(new Guid("{e9eb1dcf-92f6-4d4d-84ae-96222d60f56b}"), 1),
            //        new AddProxyParams(new Guid("{378d0690-9da0-4dd1-ab16-1d15246e7c22}"), 1),
            //        new AddProxyParams(new Guid("{b7798b74-037e-4f0c-8ac7-dc1043d093e0}"), 1),
            //        new AddProxyParams(new Guid("{5edaea74-32cb-4586-bd72-66694eb73160}"), 1),
            //        new AddProxyParams(new Guid("{ca80054a-cde0-4f69-a132-10502b24866d}"), 0),
            //        new AddProxyParams(new Guid("{51f61166-7202-45aa-9126-3d83055b269e}"), 0),
            //    } },


            //    {"Grasshopper.Kernel.Types.GH_Boolean",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{2e78987b-9dfb-42a2-8b76-3923ac8bd91a}"), 0),
            //        new AddProxyParams(new Guid("{a8b97322-2d53-47cd-905e-b932c3ccd74e}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Integer",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{57da07bd-ecab-415d-9d86-af36d7073abc}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Number",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{57da07bd-ecab-415d-9d86-af36d7073abc}"), 0),
            //    } },

            //    {"Grasshopper.Kernel.Types.GH_Colour",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{9c53bac0-ba66-40bd-8154-ce9829b9db1a}"), 0),
            //        new AddProxyParams(new Guid("{339c0ee1-cf11-444f-8e10-65c9150ea755}"), 0),
            //        new AddProxyParams(new Guid("{6da9f120-3ad0-4b6e-9fe0-f8cde3a649b7}"), 0),
            //        new AddProxyParams(new Guid("{51a2ede9-8f8c-4fdf-a375-999c2062eab7}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_ComplexNumber",new AddProxyParams[]{ new AddProxyParams(new Guid("{63d12974-2915-4ccf-ac26-5d566c3bac92}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Interval", new AddProxyParams[]{new AddProxyParams(new Guid("{d1a28e95-cf96-4936-bf34-8bf142d731bf}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Interval2D", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{8555a743-36c1-42b8-abcc-06d9cb94519f}"), 0),
            //        new AddProxyParams(new Guid("{9083b87f-a98c-4e41-9591-077ae4220b19}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Matrix", new AddProxyParams[]{new AddProxyParams(new Guid("{54ac80cf-74f3-43f7-834c-0e3fe94632c6}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Time",new AddProxyParams[]{ new AddProxyParams(new Guid("{31534405-6573-4be6-8bf8-262e55847a3a}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_StructurePath",new AddProxyParams[]{ new AddProxyParams(new Guid("{946cb61e-18d2-45e3-8840-67b0efa26528}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Material",new AddProxyParams[]{ new AddProxyParams(new Guid("{76975309-75a6-446a-afed-f8653720a9f2}"), 0) } },

            //    {"Grasshopper.Kernel.Types.GH_MeshingParameters",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{4a0180e5-d8f9-46e7-bd34-ced804601462}"), 0),
            //        new AddProxyParams(new Guid("{255ca3e9-2c1e-443a-a404-e76b5c63f4cb}"), 0),
            //        new AddProxyParams(new Guid("{1b0ee096-cc76-4847-8941-04a9e256de76}"), 0),
            //    } },
            //    {"SurfaceComponents.SurfaceComponents.LoftOptions",new AddProxyParams[]{ new AddProxyParams(new Guid("{45f19d16-1c9f-4b0f-a9a6-45a77f3d206c}"), 0) } },

            //};

            //CreateProxyDictOutput = new Dictionary<string, AddProxyParams[]>() 
            //{
            //    {"Grasshopper.Kernel.Types.GH_Point", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{93b8e93d-f932-402c-b435-84be04d87666}"), 0),
            //        new AddProxyParams(new Guid("{4a9e9a8e-0943-4438-b360-129c30f2bb0f}"), 0),
            //        new AddProxyParams(new Guid("{4beead95-8aa2-4613-8bb9-24758a0f5c4c}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Vector", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{e9eb1dcf-92f6-4d4d-84ae-96222d60f56b}"), 1),
            //        new AddProxyParams(new Guid("{962034e9-cc27-4394-afc4-5c16e3447cf9}"), 1),
            //    } },

            //    {"Grasshopper.Kernel.Types.GH_Circle",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{23862862-049a-40be-b558-2418aacbd916}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Arc", new AddProxyParams[]{new AddProxyParams(new Guid("{23862862-049a-40be-b558-2418aacbd916}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Curve", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{fc6979e4-7e91-4508-8e05-37c680779751}"), 0),
            //        new AddProxyParams(new Guid("{c75b62fa-0a33-4da7-a5bd-03fd0068fd93}"), 0),
            //        new AddProxyParams(new Guid("{5816ec9c-f170-4c59-ac44-364401ff84cd}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Line", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{fc6979e4-7e91-4508-8e05-37c680779751}"), 0),
            //        new AddProxyParams(new Guid("{c75b62fa-0a33-4da7-a5bd-03fd0068fd93}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Plane", new AddProxyParams[]{new AddProxyParams(new Guid("{807b86e3-be8d-4970-92b5-f8cdcb45b06b}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Rectangle", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{e5c33a79-53d5-4f2b-9a97-d3d45c780edc}"), 0),
            //        new AddProxyParams(new Guid("{d0a56c9e-2483-45e7-ab98-a450b97f1bc0}"), 0),
            //    } },

            //    {"Grasshopper.Kernel.Types.GH_Box", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{db7d83b1-2898-4ef9-9be5-4e94b4e2048d}"), 0),
            //        new AddProxyParams(new Guid("{af9cdb9d-9617-4827-bb3c-9efd88c76a70}"), 0),
            //        new AddProxyParams(new Guid("{a10e8cdf-7c7a-4aac-aa70-ddb7010ab231}"), 0),
            //        new AddProxyParams(new Guid("{13b40e9c-3aed-4669-b2e8-60bd02091421}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Brep", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{8d372bdc-9800-45e9-8a26-6e33c5253e21}"), 0),
            //        new AddProxyParams(new Guid("{0148a65d-6f42-414a-9db7-9a9b2eb78437}"), 0),
            //        new AddProxyParams(new Guid("{ac750e41-2450-4f98-9658-98fef97b01b2}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Mesh",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{0b4ac802-fc4a-4201-9c66-0078b837c1eb}"), 0),
            //        new AddProxyParams(new Guid("{ba2d8f57-0738-42b4-b5a5-fe4d853517eb}"), 0),
            //        new AddProxyParams(new Guid("{2b9bf01d-5fe5-464c-b0b3-b469eb5f2efb}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_MeshFace", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{e2c0f9db-a862-4bd9-810c-ef2610e7a56f}"), 1),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_SubD", new AddProxyParams[]{new AddProxyParams(new Guid("{c0b3c6e9-d05d-4c51-a0df-1ce2678c7a33}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Surface", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{353b206e-bde5-4f02-a913-b3b8a977d4b9}"), 0),
            //        new AddProxyParams(new Guid("{404f75ac-5594-4c48-ad8a-7d0f472bbf8a}"), 0),
            //    } },
            //    {"SquishyXMorphs.GH_TwistedBox",new AddProxyParams[]{ new AddProxyParams(new Guid("{d8940ff0-dd4a-4e74-9361-54df537b50db}"), 2) } },

            //    {"Grasshopper.Kernel.Types.GH_Field",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{a7c9f738-f8bd-4f64-8e7f-33341183e493}"), 0),
            //        new AddProxyParams(new Guid("{add6be3e-c57f-4740-96e4-5680abaa9169}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_GeometryGroup",new AddProxyParams[]{ 
            //        new AddProxyParams(new Guid("{a45f59c8-11c1-4ea7-9e10-847061b80d75}"), 0),
            //        new AddProxyParams(new Guid("{610e689b-5adc-47b3-af8f-e3a32b7ea341}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Transform",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{610e689b-5adc-47b3-af8f-e3a32b7ea341}"), 1),
            //        new AddProxyParams(new Guid("{ca80054a-cde0-4f69-a132-10502b24866d}"), 0),
            //        new AddProxyParams(new Guid("{51f61166-7202-45aa-9126-3d83055b269e}"), 0),
            //    } },

            //    {"Grasshopper.Kernel.Types.GH_Boolean",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{040f195d-0b4e-4fe0-901f-fedb2fd3db15}"), 0),
            //        new AddProxyParams(new Guid("{5cad70f9-5a53-4c5c-a782-54a479b4abe3}"), 0),
            //        new AddProxyParams(new Guid("{de4a0d86-2709-4564-935a-88bf4d40af89}"), 0),
            //        new AddProxyParams(new Guid("{78669f9c-4fea-44fd-ab12-2a69eeec58de}"), 0),
            //        new AddProxyParams(new Guid("{5ca5de6b-bc71-46c4-a8f7-7f30d7040acb}"), 0),
            //        new AddProxyParams(new Guid("{548177c2-d1db-4172-b667-bec979e2d38b}"), 0),
            //        new AddProxyParams(new Guid("{b6aedcac-bf43-42d4-899e-d763612f834d}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Integer",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{a0d62394-a118-422d-abb3-6af115c75b25}"), 0),
            //        new AddProxyParams(new Guid("{9c007a04-d0d9-48e4-9da3-9ba142bc4d46}"), 0),
            //        new AddProxyParams(new Guid("{ce46b74e-00c9-43c4-805a-193b69ea4a11}"), 0),
            //        new AddProxyParams(new Guid("{9c85271f-89fa-4e9f-9f4a-d75802120ccc}"), 0),
            //        new AddProxyParams(new Guid("{a3371040-e552-4bc8-b0ff-10a840258e88}"), 0),
            //        new AddProxyParams(new Guid("{431bc610-8ae1-4090-b217-1a9d9c519fe2}"), 0),
            //        new AddProxyParams(new Guid("{28124995-cf99-4298-b6f4-c75a8e379f18}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Number",new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{a0d62394-a118-422d-abb3-6af115c75b25}"), 0),
            //        new AddProxyParams(new Guid("{9c007a04-d0d9-48e4-9da3-9ba142bc4d46}"), 0),
            //        new AddProxyParams(new Guid("{ce46b74e-00c9-43c4-805a-193b69ea4a11}"), 0),
            //        new AddProxyParams(new Guid("{9c85271f-89fa-4e9f-9f4a-d75802120ccc}"), 0),
            //        new AddProxyParams(new Guid("{a3371040-e552-4bc8-b0ff-10a840258e88}"), 0),
            //        new AddProxyParams(new Guid("{431bc610-8ae1-4090-b217-1a9d9c519fe2}"), 0),
            //        new AddProxyParams(new Guid("{28124995-cf99-4298-b6f4-c75a8e379f18}"), 0),
            //    } },

            //    {"Grasshopper.Kernel.Types.GH_Colour",new AddProxyParams[]{
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_ComplexNumber",new AddProxyParams[]{ new AddProxyParams(new Guid("{63d12974-2915-4ccf-ac26-5d566c3bac92}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Interval", new AddProxyParams[]{new AddProxyParams(new Guid("{d1a28e95-cf96-4936-bf34-8bf142d731bf}"), 0) } },
            //    {"Grasshopper.Kernel.Types.GH_Interval2D", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{8555a743-36c1-42b8-abcc-06d9cb94519f}"), 0),
            //        new AddProxyParams(new Guid("{9083b87f-a98c-4e41-9591-077ae4220b19}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Matrix", new AddProxyParams[]{
            //        new AddProxyParams(new Guid("{a0d62394-a118-422d-abb3-6af115c75b25}"), 0),
            //        new AddProxyParams(new Guid("{9c007a04-d0d9-48e4-9da3-9ba142bc4d46}"), 0),
            //        new AddProxyParams(new Guid("{ce46b74e-00c9-43c4-805a-193b69ea4a11}"), 0),
            //        new AddProxyParams(new Guid("{9c85271f-89fa-4e9f-9f4a-d75802120ccc}"), 0),
            //        new AddProxyParams(new Guid("{a3371040-e552-4bc8-b0ff-10a840258e88}"), 0),
            //        new AddProxyParams(new Guid("{431bc610-8ae1-4090-b217-1a9d9c519fe2}"), 0),
            //        new AddProxyParams(new Guid("{28124995-cf99-4298-b6f4-c75a8e379f18}"), 0),

            //        new AddProxyParams(new Guid("{be715e4c-d6d8-447b-a9c3-6fea700d0b83}"), 0),
            //        new AddProxyParams(new Guid("{1f384257-b26b-4160-a6d3-1dcd89b64acd}"), 0),
            //        new AddProxyParams(new Guid("{7d2a6064-51f0-45b2-adc4-f417b30dcd15}"), 0),
            //        new AddProxyParams(new Guid("{88fb33f9-f467-452b-a0e3-44bdb78a9b06}"), 0),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Time",new AddProxyParams[]{ } },
            //    {"Grasshopper.Kernel.Types.GH_StructurePath",new AddProxyParams[]{ 
            //        new AddProxyParams(new Guid("{1d8b0e2c-e772-4fa9-b7f7-b158251b34b8}"), 0),
            //        new AddProxyParams(new Guid("{df6d9197-9a6e-41a2-9c9d-d2221accb49e}"), 0),
            //        new AddProxyParams(new Guid("{3a710c1e-1809-4e19-8c15-82adce31cd62}"), 1),
            //        new AddProxyParams(new Guid("{c1ec65a3-bda4-4fad-87d0-edf86ed9d81c}"), 1),
            //    } },
            //    {"Grasshopper.Kernel.Types.GH_Material",new AddProxyParams[]{} },

            //    {"Grasshopper.Kernel.Types.GH_MeshingParameters",new AddProxyParams[]{ new AddProxyParams(new Guid("{60e7defa-8b21-4ee1-99aa-a9223d6134ff}"), 1) } },
            //    {"SurfaceComponents.SurfaceComponents.LoftOptions",new AddProxyParams[]{ new AddProxyParams(new Guid("{a7a41d0a-2188-4f7a-82cc-1a2c4e4ec850}"), 1) } },
            //};

            int width = 24;

            Func<RectangleF, RectangleF> changeInput;
            var inFuncs = WinformControlHelper.GetInnerRectLeftFunc(1, 2, new SizeF(width, width), out changeInput);
            this.ChangeInputLayout = changeInput;

            Func<RectangleF, RectangleF> changeOutput;
            var outFuncs = WinformControlHelper.GetInnerRectRightFunc(1, 1, new SizeF(width, width), out changeOutput);
            this.ChangeOutputLayout = changeOutput;

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
                        proxies.Add(new AddProxyParams(new Guid(strCom[0]), int.Parse(strCom[1])));
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
                        proxies.Add(new AddProxyParams(new Guid(strCom[0]), int.Parse(strCom[1])));
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
                        writer.SetInt32("autoValueInt" + n.ToString("D5") + m.ToString(), set.Index);
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
                        writer.SetInt32("autoValueIntOut" + n.ToString("D5") + m.ToString(), set.Index);
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
                        int outIndex = reader.GetInt32("autoValueInt" + n.ToString("D5") + m.ToString());
                        value[m] = new AddProxyParams(guid, outIndex);
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
                        int outIndex = reader.GetInt32("autoValueIntOut" + n.ToString("D5") + m.ToString());
                        value[m] = new AddProxyParams(guid, outIndex);
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