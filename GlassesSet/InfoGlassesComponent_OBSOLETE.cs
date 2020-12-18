using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using Grasshopper.GUI;
using Grasshopper;
using Grasshopper.Kernel.Parameters;
using System.IO;
using GH_IO.Serialization;
using ArchiTed_Grasshopper;
using Grasshopper.GUI.Base;
using System.Windows.Interop;
using System.Windows.Media;
using System.Text;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace InfoGlasses
{
    public class InfoGlassesComponent_OBSOLETE : LanguagableComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;


        public class InfoGlassesComponentAttributes : GH_ComponentAttributes
        {
            public new InfoGlassesComponent_OBSOLETE Owner;

            RectangleF cateRect;
            RectangleF assemRect;
            RectangleF pluginRect;

            public InfoGlassesComponentAttributes(InfoGlassesComponent_OBSOLETE owner) : base(owner)
            {
                Owner = owner;
            }

            protected override void Layout()
            {
                int width = 25;
                int heightThick = 3;
                int upDownPad = 3;
                int leftRightPad = 3;

                Pivot = GH_Convert.ToPoint(Pivot);
                m_innerBounds = LayoutComponentBox(base.Owner);
                LayoutInputParams(base.Owner, m_innerBounds);
                LayoutOutputParams(base.Owner, m_innerBounds);

                RectangleF outbound = Owner.Params.Output[0].Attributes.Bounds;
                Owner.Params.Output[0].Attributes.Bounds = new RectangleF(outbound.X + width + 2 * leftRightPad, Owner.Params.Input[2].Attributes.Bounds.Y - Owner.Params.Input[2].Attributes.Bounds.Height / 2,
                    Math.Max(outbound.Width, width + leftRightPad), Owner.Params.Input[2].Attributes.Bounds.Height * 1.5f);

                Bounds = LayoutBounds(base.Owner, m_innerBounds);




                //this.Bounds = new RectangleF(this.Bounds.Location, new SizeF(this.Bounds.Width + width + 2 * leftRightPad, this.Bounds.Height));

                float x = (this.Bounds.Right - width - leftRightPad - Owner.Params.Output[0].Attributes.Bounds.Width);
                float height = ((this.Bounds.Height - 2 * upDownPad - heightThick) / 2);
                cateRect = new RectangleF(x, this.Bounds.Y + upDownPad, width, height);
                assemRect = new RectangleF(x, this.Bounds.Y + upDownPad + height + heightThick, width, height);
                pluginRect = new RectangleF(x + width + leftRightPad, cateRect.Y, width, height);
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                if (channel == GH_CanvasChannel.Objects && !Owner.Locked && canvas.Viewport.Zoom > 0.5f)
                {
                    DrawObject(graphics);
                }
                switch (channel)
                {
                    case GH_CanvasChannel.Wires:
                        foreach (IGH_Param item in base.Owner.Params.Input)
                        {
                            item.Attributes.RenderToCanvas(canvas, GH_CanvasChannel.Wires);
                        }
                        break;
                    case GH_CanvasChannel.Objects:

                        RenderComponentCapsule(canvas, graphics, drawComponentBaseBox: true, drawComponentNameBox: true, drawJaggedEdges: false, drawParameterGrips: true, drawParameterNames: true, drawZuiElements: true);

                        CanvasRenderEngine.RenderButtonIcon_Obsolete(graphics, Owner, cateRect, !Owner.Locked && Owner.cateSet, Properties.Resources.Category, Properties.Resources.Category_Locked, normalPalette: GH_Palette.Hidden);
                        CanvasRenderEngine.RenderButtonIcon_Obsolete(graphics, Owner, assemRect, !Owner.Locked && Owner.assemSet, Properties.Resources.Assembly, Properties.Resources.Assembly_Locked, normalPalette: GH_Palette.Hidden);
                        CanvasRenderEngine.RenderButtonIcon_Obsolete(graphics, Owner, pluginRect, !Owner.Locked && Owner.GetValue("pluginButton", false), Properties.Resources.PlugInIcon, Properties.Resources.PlugInIcon_Locked, normalPalette: GH_Palette.Hidden);
                        break;
                    case GH_CanvasChannel.First:
                        if (Owner.GetValue("pluginButton", false))
                            foreach (ComInfo obj in Owner.allComInfo)
                            {
                                if(obj.IsShowPlugin && obj.Typeinfo.IsPlugin)
                                    CanvasRenderEngine.HighLightObject(graphics, obj.Self, Owner.GetValue("HighlightColor", System.Drawing.Color.FromArgb(19, 34, 122)), Owner.GetValue("cornerHighLightRadius", 8), Owner.GetValue("cornerHighLightRadius", 8));
                            }
                        break;
                }
            }

            internal void DrawObject(Graphics graphics)
            {
                foreach (var obj in Owner.allComInfo)
                {
                    if (obj.IsShowNormal)
                        DrawOneObject(graphics, obj);
                }
            }

            internal void DrawOneObject(Graphics graphics, ComInfo info)
            {
                PointF pivot;
                if (info.Typeinfo.Name != "")
                {
                    RectangleF rect = DrawName(graphics, info, 3);
                    pivot = new PointF(rect.X + rect.Width / 2, rect.Y);
                }
                else
                    pivot = new PointF(info.Self.Attributes.Bounds.X + info.Self.Attributes.Bounds.Width / 2, info.Self.Attributes.Bounds.Y);

                if (Owner.cateSet)
                {
                    if (Owner.GetValue("MergeBox", @default: true))
                        DrawCategory(graphics, info, pivot, 3);
                    else
                        DrawCategory(graphics, info, pivot, 10, 3);

                }
                if (Owner.assemSet && info.Self.Attributes.Selected)
                {
                    DrawAssembly(graphics, info, Owner.GetValue("AssemblyWidth", 150), 3);
                }
            }

            internal void DrawAssembly(Graphics graphics, ComInfo info, int width, int height)
            {
                Font font = new Font(GH_FontServer.Standard.FontFamily, Owner.GetValue("AssemblySize", 5));
                string locText = info.ShowLocation;
                string infoText = info.FullName;

                IGH_DocumentObject obj = info.Self;
                int addiTionHeight = Owner.addHeightMul * 14;
                if (Owner.GetValue("AssemHeight", @default: true))
                {
                    if (obj is GH_Cluster)
                    {
                        GH_Cluster cluster = obj as GH_Cluster;
                        addiTionHeight = CanvasRenderEngine.MessageBoxHeight(cluster.Message, (int)obj.Attributes.Bounds.Width);
                    }
                    else if (obj is GH_Component)
                    {
                        GH_Component com = obj as GH_Component;
                        addiTionHeight = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)obj.Attributes.Bounds.Width);
                    }
                }

                if (Owner.GetValue("AvoProfiler", @default: false))
                {
                    addiTionHeight = addiTionHeight > 16 ? addiTionHeight : 16;
                }


                int firstHeight = height + addiTionHeight + 2;

                SizeF locSize = graphics.MeasureString(locText, font, width);
                SizeF infoSize = graphics.MeasureString(infoText, font, width);

                float locHeight = locSize.Height;
                float infoHeight = infoSize.Height;

                PointF locPivot = new PointF(obj.Attributes.Bounds.X + obj.Attributes.Bounds.Width / 2, obj.Attributes.Bounds.Y + obj.Attributes.Bounds.Height + locHeight + firstHeight);
                PointF infoPivot = new PointF(locPivot.X, locPivot.Y + infoHeight + height);

                int maxwidth = (int)(width * 0.9);
                int lineOffset = Math.Min((int)(obj.Attributes.Bounds.Width / 2), maxwidth) / 2;
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(Owner.boundaryColor.A / 2, Owner.boundaryColor), 2);

                graphics.DrawLine(pen, new PointF(locPivot.X + lineOffset, locPivot.Y - locHeight), new PointF(locPivot.X + lineOffset, locPivot.Y - firstHeight - locHeight));
                graphics.DrawLine(pen, new PointF(locPivot.X - lineOffset, locPivot.Y - locHeight), new PointF(locPivot.X - lineOffset, locPivot.Y - firstHeight - locHeight));

                graphics.DrawLine(pen, new PointF(infoPivot.X + lineOffset, infoPivot.Y - infoHeight), new PointF(infoPivot.X + lineOffset, infoPivot.Y - height - infoHeight));
                graphics.DrawLine(pen, new PointF(infoPivot.X - lineOffset, infoPivot.Y - infoHeight), new PointF(infoPivot.X - lineOffset, infoPivot.Y - height - infoHeight));

                RectangleF locRect = CanvasRenderEngine.MiddleDownRect(locPivot, locSize);
                RectangleF infoRect = CanvasRenderEngine.MiddleDownRect(infoPivot, infoSize);


                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, locRect, System.Drawing.Color.FromArgb(Owner.backColor.A / 2, Owner.backColor), Owner.boundaryColor, locText, font, Owner.textColor);
                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, infoRect, System.Drawing.Color.FromArgb(Owner.backColor.A / 2, Owner.backColor), Owner.boundaryColor, infoText, font, Owner.textColor);
            }

            internal void DrawCategory(Graphics graphics, ComInfo info, PointF pivot, int height)
            {
                bool flag = Owner.GetValue("FullNameCategory", @default: false);

                string cate = flag ? info.Typeinfo.Category : Instances.ComponentServer.GetCategoryShortName(info.Typeinfo.Category);
                string subcate = info.Typeinfo.Subcategory;

                string uniStr = cate + " - " + subcate;
                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, new PointF(pivot.X, pivot.Y - height), Owner.backColor, Owner.boundaryColor, uniStr, new Font(GH_FontServer.Standard.FontFamily, Owner.GetValue("NameSize", 8)), Owner.textColor);
            }

            internal void DrawCategory(Graphics graphics, ComInfo info, PointF pivot, int width, int height)
            {
                bool flag = Owner.GetValue("FullNameCategory", @default: false);
                string cate = flag ? info.Typeinfo.Category : Instances.ComponentServer.GetCategoryShortName(info.Typeinfo.Category);
                string subcate = info.Typeinfo.Subcategory;

                SizeF cateSize = graphics.MeasureString(cate, GH_FontServer.Standard);
                SizeF subSize = graphics.MeasureString(subcate, GH_FontServer.Standard);
                float totalWidth = cateSize.Width + subSize.Width + width;

                PointF catePivot = new PointF(pivot.X - (totalWidth / 2 - cateSize.Width / 2), pivot.Y - height);
                PointF subPivot = new PointF(pivot.X + (totalWidth / 2 - subSize.Width / 2), pivot.Y - height);

                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, catePivot, Owner.backColor, Owner.boundaryColor, cate, GH_FontServer.Standard, Owner.textColor);
                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, subPivot, Owner.backColor, Owner.boundaryColor, subcate, GH_FontServer.Standard, Owner.textColor);

            }

            internal RectangleF DrawName(Graphics graphics, ComInfo info, int height)
            {
                var obj = info.Self;
                PointF downLoc = new PointF(obj.Attributes.Bounds.X + obj.Attributes.Bounds.Width / 2, obj.Attributes.Bounds.Y - height - Owner.GetValue("NameHeight", 3)); 
                return CanvasRenderEngine.DrawTextBox_Obsolete(graphics, downLoc, Owner.backColor, Owner.boundaryColor, info.Typeinfo.Name, new Font(GH_FontServer.Standard.FontFamily, Owner.GetValue("NameSize", 8)), Owner.textColor);
            }




            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (m_innerBounds.Contains(e.CanvasLocation) && e.Button == MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.CreateWindow();
                    return GH_ObjectResponse.Release;
                }
                return base.RespondToMouseDoubleClick(sender, e);

            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if ((cateRect.Contains(e.CanvasLocation) || assemRect.Contains(e.CanvasLocation) || pluginRect.Contains(e.CanvasLocation)) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    return GH_ObjectResponse.Handled;
                }
                return base.RespondToMouseDown(sender, e);
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (cateRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.cateSet = !Owner.cateSet;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                else if (assemRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.assemSet = !Owner.assemSet;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                else if (pluginRect.Contains(e.CanvasLocation) && e.Button == System.Windows.Forms.MouseButtons.Left && sender.Viewport.Zoom >= 0.5f)
                {
                    Owner.SetValue("pluginButton", !Owner.GetValue("pluginButton", false));
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                return base.RespondToMouseUp(sender, e);
            }


        }

        public override void CreateAttributes()
        {
            base.m_attributes = new InfoGlassesComponentAttributes(this);
        }

        protected System.Drawing.Color textColor;
        protected System.Drawing.Color backColor;
        protected System.Drawing.Color boundaryColor;

        public List<ComTypeInfo> allObjectTypes = new List<ComTypeInfo>();
        public bool RemovePlugins = false;
        public List<ComTypeInfo> normalExceptionGuid = null;
        public List<ComTypeInfo> pluginExceptionGuid = null;

        protected List<ComInfo> allComInfo = new List<ComInfo>();

        private bool IsFirst = true;

        protected bool cateSet = false;
        protected bool assemSet = false;

        protected int addHeightMul = 0;

        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            string[] input1 = new string[] { GetTransLation(new string[] { "TextColor", "文字颜色" }), GetTransLation(new string[] { "Tc", "文字颜色" }), GetTransLation(new string[] { "TextColor", "文字颜色" }) };
            string[] input2 = new string[] { GetTransLation(new string[] { "BoundaryColor", "边框颜色" }), GetTransLation(new string[] { "Bc", "边框颜色" }), GetTransLation(new string[] { "BoundaryColor", "边框颜色" }) };
            string[] input3 = new string[] { GetTransLation(new string[] { "BackgroundColor", "背景颜色" }), GetTransLation(new string[] { "bc", "背景颜色" }), GetTransLation(new string[] { "BackgroundColor", "背景颜色" }) };

            string[] output1 = new string[] { GetTransLation(new string[] { "Plug-in use", "插件使用" }), GetTransLation(new string[] { "Pu", "插件" }), GetTransLation(new string[] { "Plug-in use", "插件使用" }) };

            ChangeComponentAtt(this, new string[] {GetTransLation(new string[] { "InfoGlasses", "信息眼镜" }), GetTransLation(new string[] { "Info", "信息" }),
                GetTransLation(new string[] { "To show the components' advances information.Right click to have advanced options", "显示电池的高级信息。右键可以获得更多选项。" }) },
                new string[][] { input1, input2, input3 }, new string[][] { output1 });

            this.ExpireSolution(true);
        }

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public InfoGlassesComponent_OBSOLETE()
          : base(GetTransLation(new string[] { "InfoGlasses", "信息眼镜" }), GetTransLation(new string[] { "Info", "信息" }),
                GetTransLation(new string[] { "To show the components' advances information.Right click to have advanced options", "显示电池的高级信息。右键可以获得更多选项。" }),
              "Params", "Showcase Tools")
        {
            LanguageChanged += ResponseToLanguageChanged;
            ResponseToLanguageChanged(this, new EventArgs());
        }



        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter(GetTransLation(new string[] { "TextColor", "文字颜色" }), GetTransLation(new string[] { "Tc", "文字颜色" }), GetTransLation(new string[] { "TextColor", "文字颜色" }),
                GH_ParamAccess.item, System.Drawing.Color.Black);
            pManager.AddColourParameter(GetTransLation(new string[] { "BoundaryColor", "边框颜色" }), GetTransLation(new string[] { "Bc", "边框颜色" }), GetTransLation(new string[] { "BoundaryColor", "边框颜色" }),
                GH_ParamAccess.item, System.Drawing.Color.FromArgb(30, 30, 30));
            pManager.AddColourParameter(GetTransLation(new string[] { "BackgroundColor", "背景颜色" }), GetTransLation(new string[] { "bc", "背景颜色" }), GetTransLation(new string[] { "BackgroundColor", "背景颜色" }),
                GH_ParamAccess.item, System.Drawing.Color.WhiteSmoke);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(GetTransLation(new string[] { "Plug-in use", "插件使用" }), GetTransLation(new string[] { "Pu", "插件" }), GetTransLation(new string[] { "Plug-in use", "插件使用" }),
                GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData(0, ref textColor);
            DA.GetData(1, ref boundaryColor);
            DA.GetData(2, ref backColor);

            if (IsFirst)
            {
                GetProxy();

                if (normalExceptionGuid == null && pluginExceptionGuid == null)
                    Readtxt();
                IsFirst = false;
            }

            allComInfo.Clear();
            foreach (GH_DocumentObject docObj in this.OnPingDocument().Objects)
            {
                AddDocObjectInfo(docObj);
            }

            this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
            this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;
            this.OnPingDocument().ObjectsDeleted -= ObjectsDeletedAction;
            this.OnPingDocument().ObjectsDeleted += ObjectsDeletedAction;

            DA.SetDataList(0, GetAssemblyToString());

        }

        private void GetProxy()
        {
            allObjectTypes.Clear();
            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies.ToList())
            {
                if (!proxy.Obsolete && proxy.SDKCompliant && proxy.Exposure!= GH_Exposure.hidden)
                {
                    ComTypeInfo type = new ComTypeInfo(proxy);
                    if(type.Isvalid)
                        allObjectTypes.Add(type);
                }
                    
            }
        }

        /// <summary>
        /// ForDA
        /// </summary>
        #region

        private void ObjectsDeletedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                RemoveDocObjectInfo(docObj);

            }

        }

        private void ObjectsAddedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                AddDocObjectInfo(docObj);
            }
        }

        private void RemoveDocObjectInfo(IGH_DocumentObject docObject)
        {
            foreach (ComInfo item in allComInfo)
            {
                if(item.Self == docObject)
                {
                    allComInfo.Remove(item);
                    return;
                }
            }
        }

        internal void AddDocObjectInfo(IGH_DocumentObject docObject)
        {
            if (!IsObjectValid(docObject)) return;

            allComInfo.Add(new ComInfo(docObject, allObjectTypes, normalExceptionGuid, pluginExceptionGuid));

        }

        private bool IsObjectValid(IGH_DocumentObject docObject)
        {
            if (docObject.OnPingDocument() != this.OnPingDocument()) return false;
            if (docObject is InfoGlassesComponent_OBSOLETE && docObject != this)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, GetTransLation(new string[] { "Please place only one InfoGlasses component per document!", "请在每个文档中只放置一个信息眼镜运算器！" }));
            }
            if (docObject is GH_Group)
            {
                GH_Group group = docObject as GH_Group;
                if (group.Attributes.Bounds == new RectangleF()) return false;
            }
            return true;
        }

        public void UpdateIsShow()
        {
            foreach (var item in allComInfo)
            {
                item.UpdateIs(normalExceptionGuid, pluginExceptionGuid);
            }
        }

        #endregion


        protected List<string> GetAssemblyToString()
        {
            
            List<string> outlist = new List<string>();
            List<ComTypeInfo> infos = new List<ComTypeInfo>();
            foreach (var item in allComInfo)
            {
                if (item.Typeinfo.IsPlugin)
                {
                    bool flag = true;
                    foreach (var info in infos)
                    {
                        if (info.AssemblyName == item.Typeinfo.AssemblyName)
                            flag = false;
                    }
                    if (flag)
                        infos.Add(item.Typeinfo);
                }
            }

            foreach (ComTypeInfo info in infos)
            {
                string normalInfo = (info.AssemblyName != null ? (GetTransLation(new string[] { "Plug-in Name:    ", "插件名:    " }) + info.AssemblyName) : GetTransLation(new string[] { "    <Unnamed>", "    <未命名>" }))
                    + (info.AssemblyAuthor != null ? (GetTransLation(new string[] { "    |    Plug-in Author:    ", "    |    插件作者:    " }) + info.AssemblyAuthor) : "");
                outlist.Add(normalInfo + "\n" + info.PluginLocation + "\n ");
            }
            return outlist;
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.InfoGlasses;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("99afb7b1-8c56-4aa4-9792-38452dd85d0f"); }
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            try
            {
                this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
                this.OnPingDocument().ObjectsDeleted -= ObjectsDeletedAction;
                LanguageChanged -= ResponseToLanguageChanged;

            }
            catch
            {
            }
            
            base.RemovedFromDocument(document);
        }


        /// <summary>
        /// Menu
        /// </summary>
        /// <param name="menu"></param>
        #region

        public override void CreateWindow()
        {

            ExceptionWindow_OBSOLETE window = new ExceptionWindow_OBSOLETE(this);
            WindowInteropHelper ownerHelper = new WindowInteropHelper(window);
            ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;
            window.Show();
            LanguageChanged += window.WindowLanguageChanged;
        }


        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem exceptionsItem = new ToolStripMenuItem(GetTransLation(new string[] { "Exceptions", "除去项" }), Properties.Resources.ExceptionIcon, exceptionClick);
            exceptionsItem.ToolTipText = GetTransLation(new string[] { "Except for the following selected component.", "除了以下选中的运算器" });
            exceptionsItem.Font = GH_FontServer.StandardBold;
            exceptionsItem.ForeColor = System.Drawing.Color.FromArgb(19, 34, 122);

            void exceptionClick(object sender, EventArgs e)
            {
                CreateWindow();
            }
            menu.Items.Add(exceptionsItem);

            GH_DocumentObject.Menu_AppendSeparator(menu);
            {
                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Name Options", "命名选项" }), System.Drawing.Color.FromArgb(19, 34, 122));
                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(GetTransLation(new string[] { "Set Name Distance", "设置命名距离" }));
                toolStripMenuItem.ToolTipText = GetTransLation(new string[] { "Set the distance between name box's top and the component's buttom. Value must be in 0 - 500. Default it 3.",
                "设置名称气泡框到运算器的距离，输入值域为0 - 500, 默认为3。"});
                GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem.DropDown, GetValue("NameHeight", 3).ToString(), menu_KeyDown, TextChanged2, true, 100, true);
                void TextChanged2(GH_MenuTextBox sender, string newText)
                {
                    int result;
                    int max = 500;
                    if (int.TryParse(sender.Text, out result))
                    {
                        result = result >= 0 ? result : 0;
                        result = result <= max ? result : max;
                        SetValue("NameHeight", result);
                    }
                    else
                        SetValue("NameHeight", 3);
                    this.ExpireSolution(true);
                }
                menu.Items.Add(toolStripMenuItem);

                ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(GetTransLation(new string[] { "Set Name FontSize", "设置命名字体大小" }));
                toolStripMenuItem2.ToolTipText = GetTransLation(new string[] { "Set the name box's font size. Value must be in 4 - 50. Default it 8.",
                "设置名称气泡框的字体大小，输入值域为4 - 50, 默认为8。"});
                GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem2.DropDown, GetValue("NameSize", 8).ToString(), menu_KeyDown, TextChanged3, true, 100, true);
                void TextChanged3(GH_MenuTextBox sender, string newText)
                {
                    int result;
                    int max = 50;
                    if (int.TryParse(sender.Text, out result))
                    {
                        result = result >= 4 ? result : 4;
                        result = result <= max ? result : max;
                        SetValue("NameSize", result);
                    }
                    else
                        SetValue("NameSize", 8);
                    this.ExpireSolution(true);
                }
                menu.Items.Add(toolStripMenuItem2);
            }

            GH_DocumentObject.Menu_AppendSeparator(menu);

            {
                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Category Options", "类别选项" }), cateSet? System.Drawing.Color.FromArgb(19, 34, 122): System.Drawing.Color.FromArgb(110, 110, 110));

                GH_DocumentObject.Menu_AppendItem(menu, GetTransLation(new string[] { "Full Name Category", "全名显示运算器类别" }), Menu_OutPutClicked, enabled: cateSet, GetValue("FullNameCategory", @default: false)).ToolTipText =
                   GetTransLation(new string[] { "When checked, it will show full name of category on box.", "当选中时，将会在每个运算器的顶部显示其类别的全名。" });
                GH_DocumentObject.Menu_AppendItem(menu, GetTransLation(new string[] { "Merge Category Box", "合并显示类别气泡框" }), Menu_OutPutClicked2, enabled: cateSet, GetValue("MergeBox", @default: true)).ToolTipText =
                     GetTransLation(new string[] { "When checked, it will merge two boxes as one.", "当选中时， 每个运算器上显示类别的气泡框将会合成一个。" });
            }
                GH_DocumentObject.Menu_AppendSeparator(menu);
            {

                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Assembly Options", "类库选项" }), assemSet ? System.Drawing.Color.FromArgb(19, 34, 122) : System.Drawing.Color.FromArgb(110, 110, 110));

                GH_DocumentObject.Menu_AppendItem(menu, GetTransLation(new string[] { "Auto Assembly Height", "自动设置类库高度" }), Menu_OutPutClicked4, enabled: assemSet, GetValue("AssemHeight", @default: true)).ToolTipText =
                   GetTransLation(new string[] { "When checked, it will Automaticly change assembly's height.", "当选中时， 将会自动调整类库气泡框到运算器的距离。" });
                GH_DocumentObject.Menu_AppendItem(menu, GetTransLation(new string[] { "Avoid Profiler", "避开计时器" }), Menu_OutPutClicked5, enabled: GetValue("AssemHeight", @default: true) && assemSet, GetValue("AvoProfiler", @default: false)).ToolTipText =
                    GetTransLation(new string[] { "When checked, it will Automaticly avoid profiler.", "当选中时，将会自动避开计时器。" });

                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(GetTransLation(new string[] { "Set Assembly Distance", "设置类库距离" }));
                toolStripMenuItem.Enabled = assemSet && !GetValue("AssemHeight", @default: true);
                toolStripMenuItem.ToolTipText = GetTransLation(new string[] { "Set the distance between assembly box's top and the component's buttom, whose unit is Line spacing. Value must be in 0 - 64. Default it 0.",
                "设置类库气泡框到运算器的距离，单位为行间距，输入值域为0 - 64, 默认为0。"});
                GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem.DropDown, addHeightMul.ToString(), menu_KeyDown, TextChanged, enabled: assemSet && !GetValue("AssemHeight", @default: true), 100, true);
                menu.Items.Add(toolStripMenuItem);

                ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(GetTransLation(new string[] { "Set Assembly FontSize", "设置类库字体大小" }));
                toolStripMenuItem2.ToolTipText = GetTransLation(new string[] { "Set the assembly box's font size. Value must be in 4 - 50. Default it 5.",
                "设置类库气泡框的字体大小，输入值域为4 - 50, 默认为5。"});
                toolStripMenuItem2.Enabled = assemSet;
                GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem2.DropDown, GetValue("AssemblySize", 5).ToString(), menu_KeyDown, TextChanged3, enabled: assemSet, 100, true);
                void TextChanged3(GH_MenuTextBox sender, string newText)
                {
                    int result;
                    int max = 50;
                    if (int.TryParse(sender.Text, out result))
                    {
                        result = result >= 4 ? result : 4;
                        result = result <= max ? result : max;
                        SetValue("AssemblySize", result);
                    }
                    else
                        SetValue("AssemblySize", 5);
                    this.ExpireSolution(true);
                }
                menu.Items.Add(toolStripMenuItem2);

                ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem(GetTransLation(new string[] { "Set Assembly Width", "设置类库宽度" }));
                toolStripMenuItem3.ToolTipText = GetTransLation(new string[] { "Set the assembly box's width. Value must be in 50 - 1000. Default it 150.",
                "设置类库气泡框的宽度，输入值域为50 - 1000, 默认为150。"});
                toolStripMenuItem3.Enabled = assemSet;
                GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem3.DropDown, GetValue("AssemblyWidth", 150).ToString(), menu_KeyDown, TextChanged4, enabled: assemSet, 100, true);
                void TextChanged4(GH_MenuTextBox sender, string newText)
                {
                    int result;
                    int min = 50;
                    int max = 1000;
                    if (int.TryParse(sender.Text, out result))
                    {
                        result = result >= min ? result : min;
                        result = result <= max ? result : max;
                        SetValue("AssemblyWidth", result);
                    }
                    else
                        SetValue("AssemblyWidth", 150);
                    this.ExpireSolution(true);
                }
                menu.Items.Add(toolStripMenuItem3);

            }
            GH_DocumentObject.Menu_AppendSeparator(menu);

            {
                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Plug-in Options", "插件选项" }), GetValue("pluginButton", false) ? System.Drawing.Color.FromArgb(19, 34, 122) : System.Drawing.Color.FromArgb(110, 110, 110));

                ToolStripMenuItem highLightColorItem = new ToolStripMenuItem(GetTransLation(new string[] { "Highlight Color", "高亮显示颜色" }));
                highLightColorItem.ToolTipText = GetTransLation(new string[] { "Modify the highlight color.", "修改高亮显示颜色。" });
                highLightColorItem.Enabled = GetValue("pluginButton", false);
                GH_DocumentObject.Menu_AppendColourPicker(highLightColorItem.DropDown, GetValue("HighlightColor", System.Drawing.Color.FromArgb(19, 34, 122)), textcolorChange);
                void textcolorChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    SetValue("HighlightColor", e.Colour);
                }
                menu.Items.Add(highLightColorItem);


                ToolStripMenuItem cornerRadiusItem = new ToolStripMenuItem(GetTransLation(new string[] { "Highlight Radius", "高亮显示半径" }));
                cornerRadiusItem.ToolTipText = GetTransLation(new string[] { "Modify the highlight corner radius.Value must be in 2 - 100. Default it 8.", "修改高亮显示导角半径。输入值域为5 - 100, 默认为8。" });
                cornerRadiusItem.Enabled = GetValue("pluginButton", false);
                GH_DocumentObject.Menu_AppendTextItem(cornerRadiusItem.DropDown, GetValue("cornerHighLightRadius", 8).ToString(), menu_KeyDown, distanceChanged, enabled: true, 100, true);
                void distanceChanged(GH_MenuTextBox sender, string newText)
                {
                    int defult = 8;
                    int min = 2;
                    int max = 100;

                    int result = defult;
                    if (int.TryParse(newText, out result))
                    {
                        result = result >= min ? result : min;
                        result = result <= max ? result : max;
                    }
                    else
                    {
                        result = defult;
                    }
                    SetValue("cornerHighLightRadius", result);

                    this.ExpireSolution(true);
                }
                menu.Items.Add(cornerRadiusItem);
            }

        }

 
        protected override void AppendHelpMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddURLItem(menu, "插件介绍视频（黄同学制作）", "点击以到B站查看黄同学制作的插件介绍视频。",
            WinFormPlus.ItemIconType.Bilibili, "https://www.bilibili.com/video/BV11y4y1z7VS");
            WinFormPlus.AddURLItem(menu, "插件介绍文章（开发者：秋水）", "单击即可跳转至参数化凯同学的微信公众号了解该运算器的基本操作", WinFormPlus.ItemIconType.Wechat,
                        "https://mp.weixin.qq.com/s?__biz=MzU3NDc4MjI3NQ==&mid=2247483964&idx=1&sn=9c1fe846520b57afdca9c25f4b91277c&chksm=fd2c6c10ca5be5065038705521eeb77578ea09c9a29d147b48a74d7deb83b080a1a4a817f96a&mpshare=1&scene=1&srcid=1025v9SZiJtnV5tcmCpqgBNc&sharer_sharetime=1603621384950&sharer_shareid=b1dc247fd3ccb65500d435199339c711&key=790d61d00982c950cb9020ba69f98cf7e300d07e3c4d1b7a079ead9ed6e790b91f1f901c6be2654222f4a9b849a7efcf5a8e968f47632c997f65f47f7e8215518fc889e540d2347a70648b42484afc09421ff209bfe4dc6a5da2beb71cf599cf451c7cb97bf81ebcdb5a702124ba5628123cc181bb2bae1f9ab262fb7707b348&ascene=1&uin=MTM1MzY5MzQ0OA%3D%3D&devicetype=Windows+10+x64&version=6300002f&lang=zh_CN&exportkey=A4zfRKU1KHDq4mkPUYIuZxY%3D&pass_ticket=xa6OBhggzPS9hmUCt7guGoazp5hqvioaGdM0jYD121qy7KgK7fU2s3hilWZvTEa5&wx_header=0");
            GH_DocumentObject.Menu_AppendSeparator(menu.DropDown);

            WinFormPlus.AddURLItem(menu, GetTransLation(new string[] {"See Source Code", "查看源代码" }) , GetTransLation(new string[] { "Click to the GitHub to see source code.", "点击以到GitHub查看源代码。" }), 
                WinFormPlus.ItemIconType.GitHub, "https://github.com/ArchiDog1998/Showcase-Tools");
        }

        private void TextChanged(GH_MenuTextBox sender, string newText)
        {
            int result;
            int max = 64;
            if (int.TryParse(sender.Text, out result))
            {
                result = result >= 0 ? result : 0;
                result = result <= max ? result : max;
                addHeightMul = result;
            }
            else
                addHeightMul = 0;
            this.ExpireSolution(true);
        }

        private void menu_KeyDown(GH_MenuTextBox sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) sender.CloseEntireMenuStructure();
        }

        private void Menu_OutPutClicked(object sender, EventArgs e)
        {
            if (GetValue("FullNameCategory", @default: false))
            {
                RecordUndoEvent("FullNameCategory");
               
            }
            else
            {
                RecordUndoEvent("FullNameCategory");
            }
            SetValue("FullNameCategory", !GetValue("FullNameCategory", @default: false));
            ExpireSolution(recompute: true);
        }


        private void Menu_OutPutClicked2(object sender, EventArgs e)
        {
            if (GetValue("MergeBox", @default: true))
            {
                RecordUndoEvent("MergeBox");
            }
            else
            {
                RecordUndoEvent("MergeBox");
            }
            SetValue("MergeBox", !GetValue("MergeBox", @default: true));
            ExpireSolution(recompute: true);
        }

        private void Menu_OutPutClicked4(object sender, EventArgs e)
        {
            if (GetValue("AssemHeight", @default: true))
            {
                RecordUndoEvent("AssemHeight");
            }
            else
            {
                RecordUndoEvent("AssemHeight");
            }
            SetValue("AssemHeight", !GetValue("AssemHeight", @default: true));
            ExpireSolution(recompute: true);
        }

        private void Menu_OutPutClicked5(object sender, EventArgs e)
        {
            if (GetValue("AvoProfiler", @default: false))
            {
                RecordUndoEvent("AvoProfiler");
            }
            else
            {
                RecordUndoEvent("AvoProfiler");
            }
            SetValue("AvoProfiler", !GetValue("AvoProfiler", @default: false));
            ExpireSolution(recompute: true);
        }
        #endregion

        //IO
        #region
        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("CateSet", cateSet);
            writer.SetBoolean("AssemSet", assemSet);
            writer.SetInt32("Height", addHeightMul);

            writer.SetInt32("NormalCount", normalExceptionGuid.Count);
            for (int i = 0; i < normalExceptionGuid.Count; i++)
            {
                writer.SetGuid("Normal" + i.ToString(), normalExceptionGuid[i].Guid);
            }

            writer.SetInt32("PluginCount", pluginExceptionGuid.Count);
            for (int j = 0; j < pluginExceptionGuid.Count; j++)
            {
                writer.SetGuid("Plugin" + j.ToString(), pluginExceptionGuid[j].Guid);
            }

            return base.Write(writer);
        }

        internal void Writetxt()
        {
            string name = "Infoglasses_Default";
            string path = Grasshopper.Folders.DefaultAssemblyFolder + name + ".txt";
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string normalGuids = "";
            if(normalExceptionGuid.Count > 0)
            {
                normalGuids = this.normalExceptionGuid[0].ToString();
                for (int i = 1; i < normalExceptionGuid.Count; i++)
                {
                    normalGuids += ',';
                    normalGuids += normalExceptionGuid[i].Guid.ToString();
                }
            }

            string pluginGuids = "";
            if (pluginExceptionGuid.Count > 0)
            {
                pluginGuids = this.pluginExceptionGuid[0].ToString();
                for (int i = 1; i < pluginExceptionGuid.Count; i++)
                {
                    pluginGuids += ',';
                    pluginGuids += pluginExceptionGuid[i].Guid.ToString();
                }
            }

            sw.Write(normalGuids + "\n" + pluginGuids);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void Readtxt()
        {
            string name = "Infoglasses_Default";
            normalExceptionGuid = new List<ComTypeInfo>();
            pluginExceptionGuid = new List<ComTypeInfo>();
            string path = Grasshopper.Folders.DefaultAssemblyFolder + name + ".txt";

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
                                ComTypeInfo comTypeInfo = null;
                                comTypeInfo = FindComTypeInfo(new Guid(guid));
                                if (comTypeInfo != null)
                                    normalExceptionGuid.Add(comTypeInfo);
                            }
                            catch
                            {

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
                                ComTypeInfo comTypeInfo = null;
                                comTypeInfo = FindComTypeInfo(new Guid(guid));
                                if (comTypeInfo != null)
                                    pluginExceptionGuid.Add(comTypeInfo);
                            }
                            catch
                            {

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
        
        }

        private ComTypeInfo FindComTypeInfo(Guid guid)
        {
            foreach (var item in allObjectTypes)
            {
                if (item.Guid == guid)
                    return item;
            }
            return null;
        }

        public override bool Read(GH_IReader reader)
        {
            reader.TryGetBoolean("CateSet", ref cateSet);
            reader.TryGetBoolean("AssemSet", ref assemSet);
            reader.TryGetInt32("Height", ref addHeightMul);


            int Ncount = 0;
            if (reader.TryGetInt32("NormalCount", ref Ncount))
            {
                normalExceptionGuid = new List<ComTypeInfo>();
                for (int i = 0; i < Ncount; i++)
                {
                    Guid guid = new Guid();
                    if (reader.TryGetGuid("Normal" + i.ToString(), ref guid))
                    {
                        ComTypeInfo comTypeInfo = null;
                        comTypeInfo = FindComTypeInfo(guid);
                        if (comTypeInfo != null)
                            normalExceptionGuid.Add(comTypeInfo);
                    }

                }
            }


            int Pcount = 0;
            if (reader.TryGetInt32("PluginCount", ref Pcount))
            {
                pluginExceptionGuid = new List<ComTypeInfo>();
                for (int j = 0; j < Pcount; j++)
                {
                    Guid guid = new Guid();
                    if (reader.TryGetGuid("Plugin" + j.ToString(), ref guid))
                    {
                        ComTypeInfo comTypeInfo = null;
                        comTypeInfo = FindComTypeInfo(guid);
                        if (comTypeInfo != null)
                            pluginExceptionGuid.Add(comTypeInfo);
                    }
                }
            }


            var result = base.Read(reader);
            return result;
        }

        #endregion


    }

    public class ComTypeInfo
    {
        public string Category { get; set; }
        public string Subcategory { get; set; }

        public GH_Exposure Exposure { get; set; }
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public object Tip { get; set; }
        public Guid Guid { get; set; }

        public bool Isvalid { get; set; }
        public string FullName { get; set; }
        public string ShowLocation { get; set; }
        public bool IsPlugin { get; set; }
        public string  PluginLocation { get; set; }
        public string Description { get; set; }

        public string AssemblyName { get; set; }
        public string AssemblyAuthor { get; set; }

        public ComTypeInfo(IGH_ObjectProxy proxy)
        {
            this.Category = proxy.Desc.HasCategory ? proxy.Desc.Category : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Category>", "<未命名类别>" });
            this.Subcategory = proxy.Desc.HasSubCategory ? proxy.Desc.SubCategory : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Subcategory>", "<未命名子类>" });
            this.Name = proxy.Desc.Name;
            this.Description = proxy.Desc.Description;
            #region
            Bitmap bitmap = new Bitmap(proxy.Icon, 20, 20);
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
            #endregion

            this.Exposure = proxy.Exposure;
            this.Guid = proxy.Guid;

            Isvalid = true;

            Type type = proxy.Type;
            GH_AssemblyInfo lib =null;

            if (type == null)
                this.FullName = "";
            else
            {
                this.FullName = type.FullName;
                lib = GetGHLibrary(type);
            }
                
            this.IsPlugin = checkisPlugin(proxy, lib);

            if (lib != null)
            {
                this.AssemblyName = lib.Name;
                this.AssemblyAuthor = lib.AuthorName;
            }

            this.Tip = CreateTip();
        }
        public ComTypeInfo(IGH_DocumentObject obj)
        {
            this.Category = obj.HasCategory? obj.Category: LanguagableComponent.GetTransLation(new string[] { "<Unnamed Category>", "<未命名类别>" });
            this.Subcategory = obj.HasSubCategory? obj.SubCategory : LanguagableComponent.GetTransLation(new string[] { "<Unnamed Subcategory>", "<未命名子类>" });
            this.Name = obj.Name;
            this.Description = obj.Description;
            #region
            Bitmap bitmap = new Bitmap(obj.Icon_24x24, 20, 20);
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
            #endregion

            this.Exposure = obj.Exposure;
            this.Guid = obj.ComponentGuid;
            

            Type type = obj.GetType();
            GH_AssemblyInfo lib = null;

            Isvalid = true;

            if (type == null)
                this.FullName = "";
            else
            {
                this.FullName = type.FullName;
                lib = GetGHLibrary(type);
            }

            this.PluginLocation = lib.Location;
            this.ShowLocation = this.PluginLocation;
            this.IsPlugin = !lib.IsCoreLibrary;

            if (lib != null)
            {
                this.AssemblyName = lib.Name;
                this.AssemblyAuthor = lib.AuthorName;
            }

            this.Tip = CreateTip();
        }

        private object CreateTip()
        {

            string result = "";
            result = this.Name;
            result += "\n" + this.Description;
            if (this.AssemblyName != null)
                result += ("\n \n" + this.AssemblyName + "        " + this.AssemblyAuthor);


            return result;
        }

        private bool checkisPlugin(IGH_ObjectProxy proxy, GH_AssemblyInfo lib)
        {
            PluginLocation = proxy.Location;
            if(lib == null)
            {
                ShowLocation = PluginLocation;
                return true;
            }

            string loc2 = lib.Location;
          

            if (PluginLocation != loc2)
            {
                ShowLocation = loc2 + "\n \n" + PluginLocation;
                return true;
            }
            else
            {
                ShowLocation = PluginLocation;
                return !lib.IsCoreLibrary;
            }
        }

        public static GH_AssemblyInfo GetGHLibrary(Type type)
        {
            foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if (lib.Assembly == type.Assembly)
                {
                    return lib;
                }
            }
            return null;
        }

        protected void GetObjectAssemblyLoc(IGH_DocumentObject docObject, ref string loc, ref string info)
        {
            Type type = docObject.GetType();
            info = type.FullName;

        }
    }

    public class ComInfo
    {
        public IGH_DocumentObject Self { get; set; }

        public bool IsShowNormal { get; set; }
        public bool IsShowPlugin { get; set; }
        public ComTypeInfo Typeinfo { get; set; }

        public string FullName { get; set; }
        public string ShowLocation { get; set; }

        public ComInfo(IGH_DocumentObject obj, List<ComTypeInfo> infoList, List<ComTypeInfo> exceptNormal, List<ComTypeInfo> exceptPlugin)
        {
            this.Self = obj;
            this.Typeinfo = FindTypeInfo(obj, infoList);

            if(this.Typeinfo.FullName == "")
            {
                this.FullName = obj.GetType().FullName;


                this.ShowLocation = this.Typeinfo.ShowLocation + "\n \n" + ComTypeInfo.GetGHLibrary(obj.GetType()).Location;
            }
            else
            {
                this.FullName = this.Typeinfo.FullName;
                this.ShowLocation = this.Typeinfo.ShowLocation;
            }
            UpdateIs(exceptNormal, exceptPlugin);

        }

        private ComTypeInfo FindTypeInfo(IGH_DocumentObject obj, List<ComTypeInfo> infoList)
        {
            foreach (ComTypeInfo info in infoList)
            {
                if (obj.Name == info.Name && obj.Description == info.Description)
                    return info;
            }

            foreach (ComTypeInfo info in infoList)
            {
                if (obj.ComponentGuid == info.Guid)
                    return info;
            }

            //System.Windows.Forms.MessageBox.Show(obj.Name + "|" + obj.Description);
            ComTypeInfo typeinfo = new ComTypeInfo(obj);
            infoList.Add(typeinfo);
            return typeinfo;
        }

        public void UpdateIs(List<ComTypeInfo> exceptNormal, List<ComTypeInfo> exceptPlugin)
        {
            this.IsShowNormal = !exceptNormal.Contains(Typeinfo);
            this.IsShowPlugin = !exceptPlugin.Contains(Typeinfo);
        }

    }


}
