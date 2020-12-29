/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TextBox = ArchiTed_Grasshopper.WinformControls.TextBox;
using InfoGlasses.WPF;
using System.IO;
using System.Text;

namespace InfoGlasses
{
    public class InfoGlassesComponent : LanguagableComponent
    {
        #region Values
        #region Basic Component Info
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.InfoGlasses;
        public override Guid ComponentGuid => new Guid("de131812-96cf-4cef-b9ee-7c7031802751");
        #endregion

        #region NameSets
        private readonly string _showNickName = "showNickName";
        private readonly bool _showNickNameDefault = false;
        public bool IsShowNickName => GetValue(_showNickName, _showNickNameDefault);

        private readonly string _nameBoxDistance = "nameBoxDistance";
        private readonly int _nameBoxDistanceDefault = 3;
        public int NameBoxDistance => GetValue(_nameBoxDistance, _nameBoxDistanceDefault);

        private readonly string _nameBoxFontSize = "nameBoxFontSize";
        private readonly float _nameBoxFontSizeDefault = 8;
        public float NameBoxFontSize => (float)GetValue(_nameBoxFontSize, _nameBoxFontSizeDefault);

        private readonly string _textColor = "TextColor";
        private readonly Color _textColorDefault = Color.Black;
        public Color TextColor => GetValue(_textColor, _textColorDefault);

        private readonly string _backgroundColor = "BackGroundColor";
        private readonly Color _backgroundColorDefault = Color.WhiteSmoke;
        public Color BackGroundColor => GetValue(_backgroundColor, _backgroundColorDefault);

        private readonly string _boundaryColor = "BoundaryColor";
        private readonly Color _boundaryColorDefault = Color.FromArgb(30, 30, 30);
        public Color BoundaryColor => GetValue(_boundaryColor, _boundaryColorDefault);
        #endregion

        #region Category Sets
        private readonly string _cateSetName = "cateSet";
        private readonly bool _cateDefaultValue = false;
        public bool IsShowCategory => GetValue(_cateSetName, _cateDefaultValue);

        private readonly string _fullCategory = "FullName Category";
        private readonly bool _fullCateDefault = true;
        public bool IsShowFullCate => GetValue(_fullCategory, _fullCateDefault);

        private readonly string _mergeCateBox = "Merge Box";
        private readonly bool _mergeCateDefault = true;
        public bool IsMergeCateBox => GetValue(_mergeCateBox, _mergeCateDefault);
        #endregion

        #region Assembly Sets
        private readonly string _assemSetName = "assemSet";
        private readonly bool _assemDefaultValue = false;
        public bool IsShowAssem => GetValue(_assemSetName, _assemDefaultValue);

        private readonly string _autoAssemHeight = "autoHeight";
        private readonly bool _autoAssemHeightDefault = true;
        public bool IsAutoAssem => GetValue(_autoAssemHeight, _autoAssemHeightDefault);

        private readonly string _avoidProfiler = "avoidPro";
        private readonly bool _avoidProDefault = false;
        public bool IsAvoidProfiler => GetValue(_avoidProfiler, _avoidProDefault);

        private readonly string _assemFontSize = "assemFontSize";
        private readonly float _assemFontSizeDefault = 5;
        public float AssemFontSize => (float)GetValue(_assemFontSize, _assemFontSizeDefault);

        private readonly string _assemBoxWidth = "assemBoxWidth";
        private readonly int _assemBoxWidthDefault = 150;
        public int AssemBoxWidth => GetValue(_assemBoxWidth, _assemBoxWidthDefault);

        private readonly string _assemBoxHeight = "assemboxHeight";
        private readonly int _assemBoxHeightDefault = 0;
        public int AssemBoxHeight => GetValue(_assemBoxHeight, _assemBoxHeightDefault);
        #endregion

        #region Plugin Sets
        private readonly string _showPlugin = "showPlugin";
        private readonly bool _showPluginDefault = false;
        public bool IsShowPlugin => GetValue(_showPlugin, _showPluginDefault);

        private readonly string _pluginColor = "pluginColor";
        private readonly Color _pluginColorDefault = Color.FromArgb(19, 34, 122);
        public Color PluginHighLightColor => GetValue(_pluginColor, _pluginColorDefault);

        private readonly string _highLightRadius = "highLightRadius";
        private readonly int _highLightRadiusDefault = 8;
        public int HighLightRadius => GetValue(_highLightRadius, _highLightRadiusDefault);
        #endregion

        #region Calculate
        public List<Guid> normalExceptionGuid;
        public List<Guid> pluginExceptionGuid;

        private bool _isFirst = true;
        private List<ExceptionProxy> _allProxy;
        public List<ExceptionProxy> AllProxy
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

        private bool Run = true;
        #endregion
        #endregion

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public InfoGlassesComponent()
          : base(GetTransLation(new string[] { "InfoGlasses", "信息眼镜" }), GetTransLation(new string[] { "Info", "信息" }),
                GetTransLation(new string[] { "To show the components' advances information.Right click to have advanced options", "显示电池的高级信息。右键可以获得更多选项。" }),
              "Params", "Showcase Tools", windowsType: typeof(ExceptionWindow))
        {
            LanguageChanged += ResponseToLanguageChanged;
            ResponseToLanguageChanged(this, new EventArgs());
            //this.Window = new ExceptionWindow(this);

            int width = 24;

            var funcs = WinformControlHelper.GetInnerRectRightFunc(1, 3, new SizeF(width, width));

            ClickButtonIcon<LangWindow> CateButton = new ClickButtonIcon<LangWindow>(_cateSetName, this, funcs(0), true, Properties.Resources.Category, _cateDefaultValue, 
                tips: new string[] { "Click to choose whether to show the component's category.", "点击以选择是否要显示运算器的类别位置。" }, 
                createMenu:()=> 
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                    WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Category Options", "类别选项" }), this.IsShowCategory ? Color.FromArgb(19, 34, 122) : Color.FromArgb(110, 110, 110), margin:5);
                    WinFormPlus.AddCheckBoxItem(menu, GetTransLation(new string[] { "Full Name Category", "全名显示运算器类别" }), GetTransLation(new string[] { "When checked, it will show full name of category on box.", "当选中时，将会在每个运算器的顶部显示其类别的全名。" }),
                        null, this, _fullCategory, _fullCateDefault);

                    WinFormPlus.AddCheckBoxItem(menu, GetTransLation(new string[] { "Merge Category Box", "合并显示类别气泡框" }), GetTransLation(new string[] { "When checked, it will merge two boxes as one.", "当选中时， 每个运算器上显示类别的气泡框将会合成一个。" }),
                        null, this, _mergeCateBox, _mergeCateDefault);

                    return menu;
                });
            ClickButtonIcon<LangWindow> AssemButton = new ClickButtonIcon<LangWindow>(_assemSetName, this, funcs(1), true, Properties.Resources.Assembly, _assemDefaultValue,
                tips: new string[] { "Click to choose whether to show the component's assembly.", "点击以选择是否要显示运算器的类库位置。" }, 
                createMenu:()=>
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                    WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Assembly Options", "类库选项" }), GetValue(_assemSetName, _cateDefaultValue) ? Color.FromArgb(19, 34, 122) : Color.FromArgb(110, 110, 110), margin:5);
                    WinFormPlus.AddCheckBoxItem(menu, GetTransLation(new string[] { "Auto Assembly Height", "自动设置类库高度" }), GetTransLation(new string[] { "When checked, it will Automaticly change assembly's height.", "当选中时， 将会自动调整类库气泡框到运算器的距离。" }),
                        null, this, _autoAssemHeight, _autoAssemHeightDefault);
                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Assembly FontSize", "设置类库字体大小" }), GetTransLation(new string[] { "Set the assembly box's font size.", "设置类库气泡框的字体大小。" }), ArchiTed_Grasshopper.Properties.Resources.TextIcon,
                        true, _assemFontSizeDefault, 4, 50, _assemFontSize);
                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Assembly Width", "设置类库宽度" }), GetTransLation(new string[] {"Set the assembly box's width.","设置类库气泡框的宽度。"}), ArchiTed_Grasshopper.Properties.Resources.SizeIcon,
                        true, _assemBoxWidthDefault, 50, 1000, _assemBoxWidth);

                    GH_DocumentObject.Menu_AppendSeparator(menu);

                    WinFormPlus.AddCheckBoxItem(menu, GetTransLation(new string[] { "Avoid Profiler", "避开计时器" }), GetTransLation(new string[] { "When checked, it will Automaticly avoid profiler.", "当选中时，将会自动避开计时器。" }),
                        null, this, _avoidProfiler, _avoidProDefault, this.IsAutoAssem);

                    GH_DocumentObject.Menu_AppendSeparator(menu);

                    WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Assembly Distance", "设置类库距离" }), GetTransLation(new string[] { "Set the distance between assembly box's top and the component's buttom, whose unit is Line spacing.","设置类库气泡框到运算器的距离，单位为行间距。" }), 
                        ArchiTed_Grasshopper.Properties.Resources.DistanceIcon, !this.IsAutoAssem, _assemBoxHeightDefault, 0, 64, _assemBoxHeight);
                    return menu;
                });
            ClickButtonIcon<LangWindow> pluginBUtton = new ClickButtonIcon<LangWindow>(_showPlugin, this, funcs(2), true, Properties.Resources.PlugInIcon, _showPluginDefault,
               tips: new string[] { "Click to choose whether to show if the componet is plugin.", "点击以选择是否要显示运算器是否是插件。" },
               createMenu: () =>
               {
                   ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                   WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Plug-in Options", "插件选项" }), this.IsShowPlugin ? Color.FromArgb(19, 34, 122) : Color.FromArgb(110, 110, 110), margin: 5);
                   WinFormPlus.AddColorBoxItem(menu, this, GetTransLation(new string[] { "Highlight Color", "高亮显示颜色" }), GetTransLation(new string[] { "Modify the highlight color.", "修改高亮显示颜色。" }),
                       ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, _pluginColorDefault, _pluginColor);

                   WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Highlight Radius", "高亮显示半径" }), GetTransLation(new string[] { "Modify the highlight corner radius.", "修改高亮显示导角半径。" }),
                        ArchiTed_Grasshopper.Properties.Resources.SizeIcon, true, _highLightRadiusDefault, 2, 100, _highLightRadius);

                   return menu;
               });

            this.Controls = new IRespond[] { CateButton, AssemButton, pluginBUtton};
        }
        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem exceptionsItem = new ToolStripMenuItem(GetTransLation(new string[] { "Exceptions", "除去项" }), Properties.Resources.ExceptionIcon, exceptionClick);
            exceptionsItem.ToolTipText = GetTransLation(new string[] { "Except for the following selected component.", "除了以下选中的运算器" });
            exceptionsItem.Font = GH_FontServer.StandardBold;
            exceptionsItem.ForeColor = Color.FromArgb(19, 34, 122);

            void exceptionClick(object sender, EventArgs e)
            {
                CreateWindow();
            }
            menu.Items.Add(exceptionsItem);

            GH_DocumentObject.Menu_AppendSeparator(menu);

            WinFormPlus.AddCheckBoxItem(menu, GetTransLation(new string[] { "Show NickName", "展示昵称" }), GetTransLation(new string[] { "When checked, it will show the nickname instead of name of the component.", "当选中时，将会显示运算器的昵称而不是全名。" }),
                null, this, _showNickName, _showNickNameDefault);
            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Name Distance", "设置命名距离" }), GetTransLation(new string[] { "Set the distance between name box's top and the component's buttom.", "设置名称气泡框到运算器的距离。" }),
                ArchiTed_Grasshopper.Properties.Resources.DistanceIcon, true, _nameBoxDistanceDefault, 0, 500, _nameBoxDistance);
            WinFormPlus.AddNumberBoxItem(menu, this, GetTransLation(new string[] { "Set Name FontSize", "设置命名字体大小" }), GetTransLation(new string[] { "Set the name box's font size.", "设置名称气泡框的字体大小。" }),
                ArchiTed_Grasshopper.Properties.Resources.TextIcon, true, _nameBoxFontSizeDefault, 4, 50, _nameBoxFontSize);

            WinFormPlus.ItemSet<Color>[] sets = new WinFormPlus.ItemSet<Color>[] {

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Text Color", "文字颜色" }),GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" }),
                    null, true, _textColorDefault, _textColor),

                    new WinFormPlus.ItemSet<Color>( GetTransLation(new string[] { "Background Color", "背景颜色" }), GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" }),
                    null, true, _backgroundColorDefault, _backgroundColor),

                    new WinFormPlus.ItemSet<Color>(GetTransLation(new string[] { "Boundary Color", "边框颜色" }),
                            GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" }), null, true,
                            _boundaryColorDefault, _boundaryColor),
                    };
            WinFormPlus.AddColorBoxItems(menu, this, GetTransLation(new string[] { "Colors", "颜色" }),
            GetTransLation(new string[] { "Adjust color.", "调整颜色。" }), ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);
        }

        protected override void AppendHelpMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddURLItem(menu, "插件介绍视频（黄同学制作）", "点击以到B站查看黄同学制作的插件介绍视频。", WinFormPlus.ItemIconType.Bilibili, "https://www.bilibili.com/video/BV11y4y1z7VS");
            WinFormPlus.AddURLItem(menu, "插件介绍文章（开发者：秋水）", "单击即可跳转至参数化凯同学的微信公众号了解该运算器的基本操作", WinFormPlus.ItemIconType.Wechat,
                        "https://mp.weixin.qq.com/s?__biz=MzU3NDc4MjI3NQ==&mid=2247483964&idx=1&sn=9c1fe846520b57afdca9c25f4b91277c&chksm=fd2c6c10ca5be5065038705521eeb77578ea09c9a29d147b48a74d7deb83b080a1a4a817f96a&mpshare=1&scene=1&srcid=1025v9SZiJtnV5tcmCpqgBNc&sharer_sharetime=1603621384950&sharer_shareid=b1dc247fd3ccb65500d435199339c711&key=790d61d00982c950cb9020ba69f98cf7e300d07e3c4d1b7a079ead9ed6e790b91f1f901c6be2654222f4a9b849a7efcf5a8e968f47632c997f65f47f7e8215518fc889e540d2347a70648b42484afc09421ff209bfe4dc6a5da2beb71cf599cf451c7cb97bf81ebcdb5a702124ba5628123cc181bb2bae1f9ab262fb7707b348&ascene=1&uin=MTM1MzY5MzQ0OA%3D%3D&devicetype=Windows+10+x64&version=6300002f&lang=zh_CN&exportkey=A4zfRKU1KHDq4mkPUYIuZxY%3D&pass_ticket=xa6OBhggzPS9hmUCt7guGoazp5hqvioaGdM0jYD121qy7KgK7fU2s3hilWZvTEa5&wx_header=0");

            base.AppendHelpMenuItems(menu);
        }
        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            string[] input = new string[] { GetTransLation(new string[] { "Run", "启动" }), GetTransLation(new string[] { "R", "启动" }), GetTransLation(new string[] { "Run", "启动" }) };

            ChangeComponentAtt(this, new string[] {GetTransLation(new string[] { "InfoGlasses", "信息眼镜" }), GetTransLation(new string[] { "Info", "信息" }),
                GetTransLation(new string[] { "To show the components' advances information.Right click to have advanced options", "显示电池的高级信息。右键可以获得更多选项。" }) },
                new string[][] { input }, new string[][] {  });

            this.ExpireSolution(true);
        }

        #region Calculate
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("", "", "", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0,ref Run);

            this.RenderObjs = new List<IRenderable>();
            this.RenderObjsUnderComponent = new List<IRenderable>();
            this.OnPingDocument().ObjectsAdded -= InfeGlassesComponent_ObjectsAdded;

            if (_isFirst)
            {
                _isFirst = false;

                if(normalExceptionGuid == null || pluginExceptionGuid == null)
                {
                    normalExceptionGuid = new List<Guid>();
                    pluginExceptionGuid = new List<Guid>();

                    Readtxt();
                }


            }

            if (Run)
            {
                foreach (var obj in this.OnPingDocument().Objects)
                {
                    this.AddOneObject(obj);
                }
                this.OnPingDocument().ObjectsAdded += InfeGlassesComponent_ObjectsAdded;
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }
        }

        private void InfeGlassesComponent_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (var obj in e.Objects)
            {
                this.AddOneObject(obj);
            }
        }

        private void AddOneObject(IGH_DocumentObject obj)
        {
            bool showNormal = !normalExceptionGuid.Contains(obj.ComponentGuid);
            bool showPlugin = !pluginExceptionGuid.Contains(obj.ComponentGuid);
            if (showNormal)
            {

                Font nameFont = new Font(GH_FontServer.Standard.FontFamily, NameBoxFontSize);
                TextBoxRenderSet nameSet = new TextBoxRenderSet(BackGroundColor, BoundaryColor, nameFont, TextColor);
                Func<SizeF, RectangleF, RectangleF> layout = (x, y) =>
                {
                 PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance);
                 return CanvasRenderEngine.MiddleDownRect(pivot, x);
                };
                this.RenderObjs.Add(new NickNameOrNameTextBox(this.IsShowNickName, obj, layout, nameSet));



                string cate = IsShowFullCate ? obj.Category : Grasshopper.Instances.ComponentServer.GetCategoryShortName(obj.Category);
                string subcate = obj.SubCategory;

                if (this.IsShowCategory)
                {
                    if (IsMergeCateBox)
                    {
                        string cateName = cate + " - " + subcate;
                        this.RenderObjs.Add(new TextBox(cateName, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - (x.Height + 3));
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, nameSet));
                    }
                    else
                    {
                        this.RenderObjs.Add(new TextBox(subcate, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - (x.Height + 3));
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, nameSet));

                        this.RenderObjs.Add(new TextBox(cate, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - (x.Height + 3) * 2);
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, nameSet));
                    }
                }
            }


            if ((this.IsShowAssem) || (this.IsShowPlugin && showPlugin))
            {
                string fullName = "";
                string location = "";
                
                Type type = obj.GetType();
                if (type != null)
                {
                    fullName = type.FullName;

                    GH_AssemblyInfo info = null;
                    foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
                    {
                        if (lib.Assembly == obj.GetType().Assembly)
                        {
                            info = lib;
                            break;
                        }
                    }
                    if(info != null)
                    {
                        location = info.Location;
                        if (!info.IsCoreLibrary)
                        {
                            if (IsShowPlugin && showPlugin)
                            {
                                this.RenderObjsUnderComponent.Add(new HighLightRect(obj, PluginHighLightColor, HighLightRadius));
                            }
                        }
                    }


                }
                
                if(!string.IsNullOrEmpty(fullName) && this.IsShowAssem)
                {
                    float height = AssemBoxHeight * 14;
                    if (IsAutoAssem)
                    {
                        if (obj is IGH_Component)
                        {
                            IGH_Component com = obj as IGH_Component;
                            height = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)obj.Attributes.Bounds.Width);
                        }
                        else
                        {
                            height = 0;
                        }

                        if (IsAvoidProfiler)
                        {
                            if (height == 0)
                                height = Math.Max(height, 16);
                            else
                                height = Math.Max(height, 32);
                        }
                    }
                    height += 5;

                    Font assemFont = new Font(GH_FontServer.Standard.FontFamily, AssemFontSize);
                    TextBoxRenderSet assemSet = new TextBoxRenderSet(Color.FromArgb(BackGroundColor.A / 2, BackGroundColor), BoundaryColor, assemFont, TextColor);
                    string fullStr = fullName;
                    if (location != null)
                        fullStr += "\n \n" + location;

                    this.RenderObjs.Add(new TextBox(fullStr, obj, (x, y) =>
                    {
                        PointF pivot = new PointF(y.Left + y.Width / 2, y.Bottom + height);
                        return CanvasRenderEngine.MiddleUpRect(pivot, x);
                    }, assemSet, (x, y, z) =>
                    {
                        return x.MeasureString(y, z, AssemBoxWidth);
                    }, showFunc: () => { return obj.Attributes.Selected; }));
                }
            }


        }

        #endregion

        #region After Calculate
        public override void RemovedFromDocument(GH_Document document)
        {
            LanguageChanged -= ResponseToLanguageChanged;
            try
            {
                this.OnPingDocument().ObjectsAdded -= InfeGlassesComponent_ObjectsAdded;
            }
            catch
            {

            }
            
            base.RemovedFromDocument(document);
        }

        public void UpdateAllProxy()
        {
            _allProxy = new List<ExceptionProxy>();
            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (!proxy.Obsolete && proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    _allProxy.Add(new ExceptionProxy(proxy, this));
                }
            }
        }

        #region ReadWrite
        public override bool Write(GH_IWriter writer)
        {

            writer.SetInt32("NormalCount", normalExceptionGuid.Count);
            for (int i = 0; i < normalExceptionGuid.Count; i++)
            {
                writer.SetGuid("Normal" + i.ToString(), normalExceptionGuid[i]);
            }

            writer.SetInt32("PluginCount", pluginExceptionGuid.Count);
            for (int j = 0; j < pluginExceptionGuid.Count; j++)
            {
                writer.SetGuid("Plugin" + j.ToString(), pluginExceptionGuid[j]);
            }

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {

            int Ncount = 0;
            if (reader.TryGetInt32("NormalCount", ref Ncount))
            {
                normalExceptionGuid = new List<Guid>();
                for (int i = 0; i < Ncount; i++)
                {
                    Guid guid = new Guid();
                    if (reader.TryGetGuid("Normal" + i.ToString(), ref guid))
                    {
                        normalExceptionGuid.Add(guid);
                    }

                }
            }


            int Pcount = 0;
            if (reader.TryGetInt32("PluginCount", ref Pcount))
            {
                pluginExceptionGuid = new List<Guid>();
                for (int j = 0; j < Pcount; j++)
                {
                    Guid guid = new Guid();
                    if (reader.TryGetGuid("Plugin" + j.ToString(), ref guid))
                    {
                        pluginExceptionGuid.Add(guid);
                    }
                }
            }

            return base.Read(reader);
        }
        #endregion

        #region IO
        public void Writetxt()
        {
            string name = "Infoglasses_Default";
            string path = System.IO.Path.GetDirectoryName( this.GetType().Assembly.Location) + "\\" + name + ".txt";

            Writetxt(path);
        }


        public void Writetxt(string path)
        {

            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string normalGuids = "";
            if (normalExceptionGuid.Count > 0)
            {
                normalGuids = this.normalExceptionGuid[0].ToString();
                for (int i = 1; i < normalExceptionGuid.Count; i++)
                {
                    normalGuids += ',';
                    normalGuids += normalExceptionGuid[i].ToString();
                }
            }

            string pluginGuids = "";
            if (pluginExceptionGuid.Count > 0)
            {
                pluginGuids = this.pluginExceptionGuid[0].ToString();
                for (int i = 1; i < pluginExceptionGuid.Count; i++)
                {
                    pluginGuids += ',';
                    pluginGuids += pluginExceptionGuid[i].ToString();
                }
            }

            sw.Write(normalGuids + "\n" + pluginGuids);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public void Readtxt()
        {
            string name = "Infoglasses_Default";
            normalExceptionGuid = new List<Guid>();
            pluginExceptionGuid = new List<Guid>();
            string path = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\" + name + ".txt";

            Readtxt(path);
        }

        /// <summary>
        /// Need Feed Back!
        /// </summary>
        /// <param name="path"></param>
        public string Readtxt(string path)
        {
            int succeedCount = 0;

            int failCount = 0;

            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);

                try
                {
                    string[] strs = sr.ReadLine().Split(',');
                    foreach (var guid in strs)
                    {
                        if (guid != "")
                        {
                            try
                            {
                                Guid uid = new Guid(guid);
                                if (!normalExceptionGuid.Contains(uid))
                                {
                                    normalExceptionGuid.Add(uid);
                                    succeedCount++;
                                }
                                    
                            }
                            catch
                            {
                                failCount++;
                            }

                        }
                    }
                }
                catch
                {
                    
                }



                try
                {
                    string[] strs2 = sr.ReadLine().Split(',');
                    foreach (var guid in strs2)
                    {
                        if (guid != "")
                        {
                            try
                            {
                                Guid uid = new Guid(guid);
                                if (!pluginExceptionGuid.Contains(uid))
                                {
                                    pluginExceptionGuid.Add(uid);
                                    succeedCount++;
                                }
                                    
                            }
                            catch
                            {
                                failCount++;
                            }

                        }
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }

            string all = succeedCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported successfully!", "个数据导入成功！" });
            if (failCount > 0)
            {
                all += "\n" + failCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported failed!", "个数据导入失败！" });
            }
            return all;
        }
        #endregion

        #endregion

        public override void CreateWindow()
        {
            WinformControlHelper.CreateWindow(Activator.CreateInstance(this.WindowsType, this) as LangWindow, this);
        }
    }
}