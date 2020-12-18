using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using ArchiTed_Grasshopper;
using System.Windows.Forms;
using Grasshopper.Kernel.Attributes;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using System.Drawing.Drawing2D;
using Grasshopper.GUI;
using System.Drawing.Text;
using Grasshopper.GUI.Base;
using GH_IO.Serialization;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace InfoGlasses
{
    public class NamedViewPPTComponent : LanguagableComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.septenary;

        private List<GH_NamedView> viewList;
        private int? viewIndex = 0;
        private string showString = "Double Click Me!";

        public class NamedViewPPTComponentAttributes : GH_ComponentAttributes
        {
            public new NamedViewPPTComponent Owner;

            protected RectangleF leftButton;
            protected RectangleF rightButton;
            protected RectangleF nameRect;

            public NamedViewPPTComponentAttributes(NamedViewPPTComponent owner) : base(owner)
            {
                Owner = owner;

            }

            protected override void Layout()
            {
            }

            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                if (channel == GH_CanvasChannel.Overlay)
                {
                    Color bodyColor = Owner.Attributes.Selected ? Color.FromArgb(255, Owner.GetValue("backgroundColor", Color.FromArgb(100, Color.WhiteSmoke))) : Owner.GetValue("backgroundColor", Color.FromArgb(100, Color.WhiteSmoke));
                    bodyColor = Owner.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count > 0 ? Color.Red : bodyColor;
                    Color textColor = Owner.Attributes.Selected ? Color.FromArgb(255, Owner.GetValue("textColor", Color.FromArgb(128, Color.Black))) : Owner.GetValue("textColor", Color.FromArgb(128, Color.Black));
                    Color lineColor = Owner.GetValue("boundaryColor", Color.DarkGray);
                    DrawTheWhole(graphics, Owner.showString, (float)Owner.GetValue("DisSize", 1f), (float)Owner.GetValue("sideDistance", 15f), textColor, bodyColor, lineColor, Owner.GetValue("arrowColor", Color.FromArgb(150, Color.Wheat)), Owner.GetValue("showOnTop", false));
                }
                    
                //base.Render(canvas, graphics, channel);
            }
            private void DrawTheWhole(Graphics graphics, string name, float size, float sideDistance, Color nameColor, Color backgroundColor, Color lineColor, Color triColor, bool top)
            {
                DrawTheWhole(graphics, name, 50 * size, 40 * size, 15 * size, sideDistance, 3 * size, nameColor, backgroundColor, lineColor, triColor, top);
            }


            private void DrawTheWhole(Graphics graphics, string name, float height, float padding, float cornerRadius, float sideDistance, float lineWeight, Color nameColor, Color backgroundColor, Color lineColor, Color triColor, bool top)
            {
                float zoom = Grasshopper.Instances.ActiveCanvas.Viewport.Zoom;

                height /= zoom;
                padding /= zoom;
                cornerRadius /= zoom;
                lineWeight /= zoom;
                padding += height * 0.2f;
                Font font = new Font(GH_FontServer.Standard.FontFamily, height/2);
                float triangleWidth = height * (float)Math.Pow(3, 0.5) / 2;
                PointF pivot = top ? GetMiddleUpPt(sideDistance / zoom + height) : GetMiddleDownPt(sideDistance / zoom);

                float nameBoxwidth = Math.Min(graphics.MeasureString(name, font).Width, Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion.Width - 4 * triangleWidth);
                nameRect = CanvasRenderEngine.MiddleDownRect(pivot, new SizeF(nameBoxwidth, height));
                CanvasRenderEngine.DrawTextBox_Obsolete(graphics, nameRect, backgroundColor, lineColor, name, font, nameColor, lineWeight, -40, cornerRadius);

                SizeF triangleSize = new SizeF(triangleWidth, height);
                float distance = nameBoxwidth / 2 + padding + triangleWidth / 2;
                leftButton = CanvasRenderEngine.MiddleDownRect(new PointF(pivot.X - distance, pivot.Y), triangleSize);
                rightButton = CanvasRenderEngine.MiddleDownRect(new PointF(pivot.X + distance, pivot.Y), triangleSize);

                
                DrawBothTriangle(graphics, lineWeight, lineColor, triColor, cornerRadius / 2f);

                this.Bounds = CanvasRenderEngine.MiddleDownRect(pivot, new SizeF(nameBoxwidth + triangleWidth * 2 + padding * 2, height));
            }

            private void DrawBothTriangle(Graphics graphics, float width, Color lineColor, Color color, float radius)
            {

                float infla = leftButton.Height * 0.2f;
                leftButton.Inflate(infla, infla);
                rightButton.Inflate(infla, infla);
                GraphicsPath pathLeft = CreatePathLeft(radius);
                GraphicsPath pathRight = CreatePathRight(radius);
                Brush linearBrush = new LinearGradientBrush(leftButton, color, color.LightenColor(-40), LinearGradientMode.Vertical);
                //leftButton.Inflate(-infla, -infla);
                //rightButton.Inflate(-infla, -infla);
                DrawPath(graphics, pathLeft, width, lineColor, linearBrush);
                DrawPath(graphics, pathRight, width, lineColor, linearBrush);
            }

            private GraphicsPath CreatePathLeft(float radius)
            {

                GraphicsPath path = new GraphicsPath();
                PointF pt1 = new PointF(leftButton.X, leftButton.Y + leftButton.Height / 2);
                PointF pt2 = new PointF(leftButton.X + leftButton.Width, leftButton.Y + leftButton.Height);
                PointF pt3 = new PointF(leftButton.X + leftButton.Width, leftButton.Y);

                float sqr3 = (float)Math.Pow(3, 0.5);

                path.AddArc(pt1.X + radius, pt1.Y - radius, 2 * radius, 2 * radius, 120, 120);
                path.AddLine(pt1.X + radius * 1.5f, pt1.Y - radius * sqr3 / 2, pt3.X - radius * 1.5f, pt3.Y + radius * sqr3 / 2);
                path.AddArc(pt3.X - 2 * radius, pt3.Y + sqr3 * radius - radius, 2 * radius, 2 * radius, 240, 120);
                path.AddLine(pt3.X, pt3.Y + radius * sqr3, pt2.X, pt2.Y - radius * sqr3);
                path.AddArc(pt2.X - 2 * radius, pt2.Y - radius - radius * sqr3, 2 * radius, 2 * radius, 0, 120);
                path.AddLine(pt2.X - 1.5f * radius, pt2.Y - radius * sqr3 / 2, pt1.X + radius * 1.5f, pt1.Y + radius * sqr3 / 2);
                return path;
            }

            private GraphicsPath CreatePathRight(float radius)
            {
                GraphicsPath path = new GraphicsPath();
                PointF pt1 = new PointF(rightButton.X + rightButton.Width, rightButton.Y + rightButton.Height / 2);
                PointF pt2 = new PointF(rightButton.X, rightButton.Y + rightButton.Height);
                PointF pt3 = new PointF(rightButton.X, rightButton.Y);

                float sqr3 = (float)Math.Pow(3, 0.5);

                path.AddArc(pt1.X - 3 * radius, pt1.Y - radius, 2 * radius, 2 * radius, -60, 120);
                path.AddLine(pt1.X - radius * 1.5f, pt1.Y + radius * sqr3 / 2, pt2.X + 1.5f * radius, pt2.Y - sqr3 * radius / 2);
                path.AddArc(pt2.X, pt2.Y - radius - sqr3 * radius, 2 * radius, 2 * radius, 60, 120);
                path.AddLine(pt2.X, pt2.Y - radius * sqr3, pt3.X, pt3.Y + radius * sqr3);
                path.AddArc(pt3.X, pt3.Y + radius * sqr3 - radius, 2 * radius, 2 * radius, 180, 120);
                path.AddLine(pt3.X + 1.5f * radius, pt3.Y + sqr3 * radius / 2, pt1.X - 1.5f * radius, pt1.Y - radius * sqr3 / 2);

                return path;
            }

            private PointF GetMiddleDownPt(float height)
            {
                RectangleF rect = Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion;
                return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height - height);
            }

            private PointF GetMiddleUpPt(float height)
            {
                RectangleF rect = Grasshopper.Instances.ActiveCanvas.Viewport.VisibleRegion;
                return new PointF(rect.X + rect.Width / 2, rect.Y + height);
            }

            private void DrawPath(Graphics graphics, GraphicsPath path, float width, Color lineColor, Brush linearBrush)
            {
               
                graphics.FillPath(linearBrush, path);
                graphics.DrawPath(new Pen(lineColor, width), path);
            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if(nameRect.Contains(e.CanvasLocation))
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        if (!Owner.viewIndex.HasValue)
                            Owner.viewIndex = 0;
                        Owner.Update();
                        return GH_ObjectResponse.Release;
                    }
                       
                    else if(e.Button == MouseButtons.Right)
                    {
                        Owner.viewIndex = 0;
                        Owner.Update();
                        return GH_ObjectResponse.Release;
                    }
                    
                }
                else if (e.Button == MouseButtons.Left && (leftButton.Contains(e.CanvasLocation) || rightButton.Contains(e.CanvasLocation)))
                    return GH_ObjectResponse.Release;

                return base.RespondToMouseDoubleClick(sender, e);
            }

            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left && (leftButton.Contains(e.CanvasLocation) || rightButton.Contains(e.CanvasLocation)))
                    return GH_ObjectResponse.Handled;
                return base.RespondToMouseDown(sender, e);
            }

            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if(e.Button == MouseButtons.Left)
                {
                    if (leftButton.Contains(e.CanvasLocation))
                    {
                        if (!Owner.viewIndex.HasValue)
                            Owner.viewIndex = 0;
                        Owner.viewIndex -= 1;
                        Owner.Update();
                        return GH_ObjectResponse.Release;
                    }
                    else if (rightButton.Contains(e.CanvasLocation))
                    {
                        if (!Owner.viewIndex.HasValue)
                            Owner.viewIndex = 0;
                        Owner.viewIndex += 1;
                        Owner.Update();
                        return GH_ObjectResponse.Release;
                    }
                }

                return base.RespondToMouseUp(sender, e);
            }
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new NamedViewPPTComponentAttributes(this);
        }

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public NamedViewPPTComponent()
          : base(GetTransLation(new string[] { "NamedViewPPT", "已命名视图幻灯片" }), GetTransLation(new string[] { "ppt", "幻灯片" }),
              GetTransLation(new string[] { "Easy to switch between named views. Right click to have advanced options", "方便在不同已命名视图之间切换。右键可以获得更多选项。" }),
              "Params", "Showcase Tools")
        {
            LanguageChanged += ResponseToLanguageChanged;
            

        }

        private void ObjectsAddedAction(object sender, GH_DocObjectEventArgs e)
        {
            foreach (GH_DocumentObject docObj in e.Objects)
            {
                if (docObj is NamedViewPPTComponent && docObj != this)
                {
                    NamedViewPPTComponent pptObj = docObj as NamedViewPPTComponent;
                    string message = GetTransLation(new string[] { "Please place only one NamedViewPPT component per document! Or recompute this component.", "请在每个文档中只放置一个已命名视图幻灯片运算器！或者，重新计算一下此运算器。" });
                    pptObj.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                }
            }
        }


        public override void RemovedFromDocument(GH_Document document)
        {
            
            try
            {
                this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
                LanguageChanged -= ResponseToLanguageChanged;
            }
            catch
            {

            }

            base.RemovedFromDocument(document);
        }

        protected override void ResponseToLanguageChanged(object sender, EventArgs e)
        {
            ChangeComponentAtt(this, new string[] {GetTransLation(new string[] { "NamedViewPPT", "已命名视图幻灯片" }), GetTransLation(new string[] { "ppt", "幻灯片" }),
                GetTransLation(new string[] { "Easy to switch between named views. Right click to have advanced options", "方便在不同已命名视图之间切换。右键可以获得更多选项。" }) },
                new string[][] { }, new string[][] { });

            this.ExpireSolution(true);
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
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
            //this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;

            //foreach (GH_DocumentObject docObj in this.OnPingDocument().Objects)
            //{
            //    if (docObj is NamedViewPPTComponent && docObj != this)
            //    {
            //        NamedViewPPTComponent pptObj = docObj as NamedViewPPTComponent;
            //        string message = GetTransLation(new string[] { "Please place only one NamedViewPPT component per document! Or recompute this component.", "请在每个文档中只放置一个已命名视图幻灯片运算器！或者，重新计算一下此运算器。" });
            //        pptObj.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
            //        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
            //    }
            //}
            this.OnPingDocument().ObjectsAdded -= ObjectsAddedAction;
            this.OnPingDocument().ObjectsAdded += ObjectsAddedAction;
            UpdateviewList();
            CheckIndex();
            ChangeName();

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
                return Properties.Resources.NamedViewPPT;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("57b4f5e5-4e42-4641-b84f-5668abb62254"); }
        }


        private void Update()
        {
            UpdateviewList();
            CheckIndex();
            SetToView();
            ChangeName();
            this.ExpireSolution(true);
        }

        private void CheckIndex()
        {
            for (int i = 0; i < viewList.Count; i++)
            {
                SetValue("View" + i.ToString(), false);
            }

            if(viewList.Count == 0)
            {
                viewIndex = null;
            }
            else
            {
                viewIndex += viewList.Count;
                viewIndex %= viewList.Count;
                SetValue("View" + viewIndex.ToString(), true);
            }
            
        }

        private void ChangeName()
        {
            if (viewIndex.HasValue) showString = viewList[viewIndex.Value].Name;
            else showString = "No Named View";
        }

        private void SetToView()
        {
            if(viewIndex.HasValue)
            {
                viewList[viewIndex.Value].SetToViewport(Grasshopper.Instances.ActiveCanvas, GetValue("interval", 300));
            }
            //this.ExpireSolution(true);
        }

        private void UpdateviewList()
        {
            viewList = Grasshopper.Instances.ActiveCanvas.Document.Properties.ViewList;
        }

        protected override void AppendAdditionComponentMenuItems(ToolStripDropDown menu)
        {
            WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "View Options", "视图选项" }), Color.FromArgb(19, 34, 122));

            GH_DocumentObject.Menu_AppendItem(menu, GetTransLation(new string[] { "Reset", "重置" }), ResetClick, Properties.Resources.ResetLogo, true, false).ToolTipText =
                GetTransLation(new string[] { "Reset to the first named view.", "重置并跳转到第一个已命名视图。" });
            void ResetClick(object sender, EventArgs e)
            {
                viewIndex = 0;
                Update();
            }


            ToolStripMenuItem viewListItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "View List", "视图列表" }),
                GetTransLation(new string[] { "Click to see the whole view list.", "点击以打开整个视图列表。" }), Properties.Resources.ListIcon);
            UpdateviewList(viewListItem);
            menu.Items.Add(viewListItem);

            ToolStripMenuItem intervalItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Interval", "时长" }),
                GetTransLation(new string[] { "View switching duration.Value must be in 10 - 5000. Default it 300.", "视图切换时长。输入值域为10 - 5000, 默认为300。" }), Properties.Resources.IntervalIcon);
            GH_DocumentObject.Menu_AppendTextItem(intervalItem.DropDown, GetValue("interval", 300).ToString(), menu_KeyDown, TextChanged, enabled: true, 100, true);
            void TextChanged(GH_MenuTextBox sender, string newText)
            {
                int result = 300;
                int min = 10;
                int max = 5000;
                if (int.TryParse(newText, out result))
                {
                    result = result >= min ? result : min;
                    result = result <= max ? result : max;
                }
                else
                {
                    result = 300;
                }
                SetValue("interval", result);
            }
            menu.Items.Add(intervalItem);

            GH_DocumentObject.Menu_AppendSeparator(menu);
            {


                WinFormPlus.AddLabelItem(menu, GetTransLation(new string[] { "Display Options", "显示选项" }), Color.FromArgb(19, 34, 122));

                ToolStripMenuItem colorItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Colors", "颜色" }),
                    GetTransLation(new string[] { "Adjust interface color.", "调整界面颜色。" }), Properties.Resources.ColorIcon);

                ToolStripMenuItem textcolorItem = new ToolStripMenuItem(GetTransLation(new string[] { "Text Color", "文字颜色" }));
                textcolorItem.ToolTipText = GetTransLation(new string[] { "Adjust text color.", "调整文字颜色。" });
                GH_DocumentObject.Menu_AppendColourPicker(textcolorItem.DropDown, GetValue("textColor", Color.FromArgb(128, Color.Black)), textcolorChange);
                void textcolorChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    SetValue("textColor", e.Colour);
                }
                colorItem.DropDown.Items.Add(textcolorItem);

                ToolStripMenuItem backgroundItem = new ToolStripMenuItem(GetTransLation(new string[] { "Background Color", "背景颜色" }));
                backgroundItem.ToolTipText = GetTransLation(new string[] { "Adjust background color.", "调整背景颜色。" });
                GH_DocumentObject.Menu_AppendColourPicker(backgroundItem.DropDown, GetValue("backgroundColor", Color.FromArgb(100, Color.WhiteSmoke)), backgroundItemChange);
                void backgroundItemChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    SetValue("backgroundColor", e.Colour);
                }
                colorItem.DropDown.Items.Add(backgroundItem);

                ToolStripMenuItem boundaryItem = new ToolStripMenuItem(GetTransLation(new string[] { "Boundary Color", "边框颜色" }));
                boundaryItem.ToolTipText = GetTransLation(new string[] { "Adjust boundary color.", "调整边框颜色。" });
                GH_DocumentObject.Menu_AppendColourPicker(boundaryItem.DropDown, GetValue("boundaryColor", Color.DarkGray), boundarycolorChange);
                void boundarycolorChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    SetValue("boundaryColor", e.Colour);
                }
                colorItem.DropDown.Items.Add(boundaryItem);

                ToolStripMenuItem arrowItem = new ToolStripMenuItem(GetTransLation(new string[] { "Arrow Color", "箭头颜色" }));
                arrowItem.ToolTipText = GetTransLation(new string[] { "Adjust arrow color.", "调整箭头颜色。" });
                GH_DocumentObject.Menu_AppendColourPicker(arrowItem.DropDown, GetValue("arrowColor", Color.FromArgb(150, Color.Wheat)), arrowcolorChange);
                void arrowcolorChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
                {
                    SetValue("arrowColor", e.Colour);
                }
                colorItem.DropDown.Items.Add(arrowItem);

                GH_DocumentObject.Menu_AppendItem(colorItem.DropDown, GetTransLation(new string[] { "Reset Color", "重置颜色" }), resetClick).ToolTipText =
                    GetTransLation(new string[] { "Click to reset colors.", "点击以重置颜色。" });
                void resetClick(object sender, EventArgs e)
                {
                    SetValue("textColor", Color.FromArgb(128, Color.Black));
                    SetValue("backgroundColor", Color.FromArgb(100, Color.WhiteSmoke));
                    SetValue("boundaryColor", Color.DarkGray);
                    SetValue("arrowColor", Color.FromArgb(150, Color.Wheat));
                    this.ExpireSolution(true);
                }
                menu.Items.Add(colorItem);
            }

            

            GH_DocumentObject.Menu_AppendItem(menu, GetValue("showOnTop", false) ? GetTransLation(new string[] { "Top Display", "顶部展示" }) : GetTransLation(new string[] { "Bottom Display", "底部展示" }), 
                upClick, GetValue("showOnTop", false) ? Properties.Resources.UpIcon:Properties.Resources.DownIcon).ToolTipText =
                GetValue("showOnTop", false) ? GetTransLation(new string[] { "Click to switch to bottom display.", "点击以切换至底部显示。" }) : GetTransLation(new string[] { "Click to switch to top display.", "点击以切换至顶部显示。" });
            void upClick(object sender, EventArgs e)
            {
                SetValue("showOnTop", !GetValue("showOnTop", false));

                ToolStripMenuItem item = sender as ToolStripMenuItem;
                item.Text = GetValue("showOnTop", false) ? GetTransLation(new string[] { "Top Display", "顶部展示" }) : GetTransLation(new string[] { "Bottom Display", "底部展示" });
                item.ToolTipText = GetValue("showOnTop", false) ? GetTransLation(new string[] { "Click to switch to bottom display.", "点击以切换至底部显示。" }) : GetTransLation(new string[] { "Click to switch to top display.", "点击以切换至顶部显示。" });
                this.ExpireSolution(true);
            }

            ToolStripMenuItem distanceItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Side Distance", "边缘距离" }),
                GetTransLation(new string[] { "The distance between component's side and canvas side.Value must be in 0 - 1000. Default it 15.", "电池边缘到画布边缘的距离。输入值域为0 - 1000, 默认为15。" }), Properties.Resources.DistanceIcon);
            GH_DocumentObject.Menu_AppendTextItem(distanceItem.DropDown, GetValue("sideDistance", 15f).ToString("f2"), menu_KeyDown, distanceChanged, enabled: true, 100, true);
            void distanceChanged(GH_MenuTextBox sender, string newText)
            {
                float result = 15;
                float min = 0;
                float max = 1000;
                if (float.TryParse(newText, out result))
                {
                    result = result >= min ? result : min;
                    result = result <= max ? result : max;
                }
                else
                {
                    result = 15;
                }
                SetValue("sideDistance", result);
                
                this.ExpireSolution(true);
            }
            menu.Items.Add(distanceItem);


            ToolStripMenuItem sizeItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Display Size", "显示尺寸" }),
                GetTransLation(new string[] { "Value must be in 0.2 - 10. Default it 1.", "输入值域为0.2 - 10, 默认为1。" }), Properties.Resources.SizeIcon);
            GH_DocumentObject.Menu_AppendTextItem(sizeItem.DropDown, GetValue("DisSize", 1f).ToString("f2"), menu_KeyDown, sizeChanged, enabled: true, 100, true);
            void sizeChanged(GH_MenuTextBox sender, string newText)
            {
                float result = 1;
                float min = 0.2f;
                float max = 10;
                if (float.TryParse(newText, out result))
                {
                    result = result >= min ? result : min;
                    result = result <= max ? result : max;
                }
                else
                {
                    result = 1;
                }
                SetValue("DisSize", result);
                this.ExpireSolution(true);
            }
            menu.Items.Add(sizeItem);
        }





        private void menu_KeyDown(GH_MenuTextBox sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) sender.CloseEntireMenuStructure();
        }

        private void UpdateviewList(ToolStripMenuItem viewListItem)
        {
            viewListItem.DropDownItems.Clear();
            for (int i = 0; i < viewList.Count; i++)
            {
                AddAnItem(viewListItem, i);
            }
        }

        private void AddAnItem(ToolStripMenuItem menu ,int i)
        {
            GH_DocumentObject.Menu_AppendItem(menu.DropDown, viewList[i].Name, viewClick, true, GetValue("View" + i.ToString(), false))
                    .ToolTipText = GetTransLation(new string[] { "Click to go to view " + viewList[i].Name, "点击以跳转至视图" + viewList[i].Name });

            void viewClick(object sender, EventArgs e)
            {
                viewIndex = i;
                Update();
            }
        }



        protected override void AppendHelpMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddURLItem(menu, GetTransLation(new string[] { "See Source Code", "查看源代码" }), GetTransLation(new string[] { "Click to the GitHub to see source code.", "点击以到GitHub查看源代码。" }),
                WinFormPlus.ItemIconType.GitHub, "https://github.com/ArchiDog1998/Showcase-Tools");
        }

        public override void CreateWindow()
        {
            throw new NotImplementedException();
        }
    }
}
