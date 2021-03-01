/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using InfoGlasses.WinformControls;
using InfoGlasses.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformMenu
{
    public class InfoGlassesMenuItem : ToolStripMenuItem
    {
        public enum InfoGlassesProps
        {
            IsUseInfoGlass,

            NormalExceptionGuid,
            PluginExceptionGuid,

            ShowFont,
            BoundaryWidth,
            BoundaryRadius,
            TextColor,
            BackgroundColor,
            BoundaryColor,

            ShowName,
            IsShowNickName,
            ShowNameDistance,

            ShowCategory,
            IsFullCategory,
            IsMergeCateBox,

            ShowAssembly,
            AssemblyFontSize,
            AssemblyWidth,
            AssemblyDistance,

            ShowPlugin,
            PluginColor,
            PluginRadius,
        }

        public static SaveableSettings<InfoGlassesProps> Settings { get; } = new SaveableSettings<InfoGlassesProps>(new SettingsPreset<InfoGlassesProps>[]
        {
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsUseInfoGlass, true, (value) =>
            {

            }),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.NormalExceptionGuid, new List<Guid>()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.PluginExceptionGuid, new List<Guid>()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowFont, GH_FontServer.Standard),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BoundaryWidth, 1.0, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BoundaryRadius, 3.0, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.TextColor, Color.Black, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BackgroundColor, Color.WhiteSmoke, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BoundaryColor, Color.FromArgb(30, 30, 30), (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowName, true),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsShowNickName, false, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowNameDistance, 3, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowCategory, false),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsFullCategory, true),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsMergeCateBox, true),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowAssembly, true),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.AssemblyFontSize, 5.0, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.AssemblyWidth, 150, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.AssemblyDistance, 0, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowPlugin, false),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.PluginColor, ColorExtension.OnColor, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.PluginRadius, 8, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
        }, Grasshopper.Instances.Settings);

        #region ForWPF
        private List<ExceptionProxyPlus> _allProxy;
        public List<ExceptionProxyPlus> AllProxy
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
        public void UpdateAllProxy()
        {
            _allProxy = new List<ExceptionProxyPlus>();
            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (!proxy.Obsolete && proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    _allProxy.Add(new ExceptionProxyPlus(proxy, this));
                }
            }
        }
        #region IO

        public string Writetxt(string path)
        {
            List<Guid> normalExceptionGuid = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList();
            List<Guid> pluginExceptionGuid = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList();

            if (string.IsNullOrEmpty(path)) return LanguagableComponent.GetTransLation(new string[] { "Failed to get path.", "获取位置失败！" });
            IO_Helper.WriteString(path, () =>
            {
                string normalGuids = "";
                if (normalExceptionGuid.Count > 0)
                {
                    normalGuids = normalExceptionGuid[0].ToString();
                    for (int i = 1; i < normalExceptionGuid.Count; i++)
                    {
                        normalGuids += ',';
                        normalGuids += normalExceptionGuid[i].ToString();
                    }
                }

                string pluginGuids = "";
                if (pluginExceptionGuid.Count > 0)
                {
                    pluginGuids = pluginExceptionGuid[0].ToString();
                    for (int i = 1; i < pluginExceptionGuid.Count; i++)
                    {
                        pluginGuids += ',';
                        pluginGuids += pluginExceptionGuid[i].ToString();
                    }
                }

                return normalGuids + "\n" + pluginGuids;
            });


            return LanguagableComponent.GetTransLation(new string[] { "Export successfully! \n Location: ", "导出成功！\n 位置：" }) + path;
        }


        /// <summary>
        /// Need Feed Back!
        /// </summary>
        /// <param name="path"></param>
        public string Readtxt(string path)
        {
            List<Guid> normalExceptionGuid = new List<Guid>();
            List<Guid> pluginExceptionGuid = new List<Guid>();

            if (path == null) return null;

            int succeedCount = 0;
            int failCount = 0;

            IO_Helper.ReadFileInLine(path, (str, index) =>
            {
                succeedCount = 0;
                failCount = 0;
                string[] strs = str.Split(',');
                foreach (var guid in strs)
                {
                    if (guid != "")
                    {
                        try
                        {
                            Guid uid = new Guid(guid);
                            switch (index)
                            {
                                case 0:
                                    if (!normalExceptionGuid.Contains(uid))
                                    {
                                        normalExceptionGuid.Add(uid);
                                    }
                                    break;
                                case 1:
                                    if (!pluginExceptionGuid.Contains(uid))
                                    {
                                        pluginExceptionGuid.Add(uid);

                                    }
                                    break;
                            }
                            succeedCount++;
                        }
                        catch
                        {
                            failCount++;
                        }

                    }
                }
                Settings.SetProperty(InfoGlassesProps.NormalExceptionGuid, normalExceptionGuid);
                Settings.SetProperty(InfoGlassesProps.PluginExceptionGuid, pluginExceptionGuid);
            });

            string all = succeedCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported successfully!", "个数据导入成功！" });
            if (failCount > 0)
            {
                all += "\n" + failCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported failed!", "个数据导入失败！" });
            }
            return all;
        }
        #endregion
        #endregion
        public InfoGlassesMenuItem(): base(Properties.Resources.InfoGlasses)
        {
            this.SetItemLangChange(new string[] { "InfoGlasses", "信息眼镜" }, null);
            this.BoundAndCheckProperty(Settings, InfoGlassesProps.IsUseInfoGlass, (item)=> 
            {
                RemovePaintActions();
                if (item.Checked)
                    AddPaintActions();
                Grasshopper.Instances.ActiveCanvas.Refresh();
            });

            this.DropDown.Items.Add(WinFormPlus.CreateClickItem(new string[] { "Exceptions", "除去项" },
                new string[] { "Click to open the exceptions window.", "单击以打开除去项窗口。" }, Properties.Resources.ExceptionIcon, (x, y)=>
                {
                    new ExceptionWindowPlus(this).Show();
                }));

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(WinFormPlus.CreateFontSelector(new string[] { "Display Font", "展示字体" },
                new string[] { "Click to change the display font.", "单击以修改展示字体。" }, Settings, InfoGlassesProps.ShowFont));
            this.DropDown.Items.Add(GetBoundarySize());
            this.DropDown.Items.Add(GetInfoGlassesColourItem());

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(GetNameItem());
            this.DropDown.Items.Add(GetCategoryItem());
            this.DropDown.Items.Add(GetAssemblyItem());
            this.DropDown.Items.Add(GetPluginItem());

            Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += ActiveCanvas_Document_ObjectsAdded;
            Grasshopper.Instances.DocumentServer.DocumentAdded += (x, y) => ResetRenderables();
            Grasshopper.Instances.ActiveCanvas.DocumentChanged += (x, y) => ResetRenderables();
        }

        #region RenderSet
        private ToolStripMenuItem GetBoundarySize()
        {
            ToolStripMenuItem item = WinFormPlus.CreateOneItem(new string[] { "BoundarySize", "边线尺寸" },
                new string[] { "Set the boundary size.", "修改边缘尺寸" },
                Properties.Resources.SizeIcon);

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Thickness", "厚度" }, new string[] { "Click to set the boundary's thickness.", "单击以修改边线厚度。" },
                null, Settings, InfoGlassesProps.BoundaryWidth, 10.0, 0.1));
            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Radius", "半径" }, new string[] { "Click to set the boundary's radius.", "单击以修改边线半径。" },
                null, Settings, InfoGlassesProps.BoundaryRadius, 20, 0));

            return item;
        }

        private ToolStripMenuItem GetInfoGlassesColourItem()
        {
            WinFormPlus.ItemSet<InfoGlassesProps>[] sets = new WinFormPlus.ItemSet<InfoGlassesProps>[]
            {
                new WinFormPlus.ItemSet<InfoGlassesProps>(new string[] { "Text Color", "文字颜色" }, new string[] { "Adjust text color.", "调整文字颜色。" },
                    null, true, InfoGlassesProps.TextColor),

                new WinFormPlus.ItemSet<InfoGlassesProps>(new string[] { "Background Color", "背景颜色" }, new string[] { "Adjust background color.", "调整背景颜色。" },
                    null, true, InfoGlassesProps.BackgroundColor),

                new WinFormPlus.ItemSet<InfoGlassesProps>(new string[] { "Boundary Color", "边框颜色"  }, new string[] { "Adjust boundary color.", "调整边框颜色。" },
                    null, true, InfoGlassesProps.BoundaryColor),
            };

            return WinFormPlus.CreateColorBoxItems(Settings, new string[] { "Colors", "颜色" }, new string[] { "Adjust color.", "调整颜色。" },
                ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);
        }
        #endregion


        #region Departments
        private ToolStripMenuItem GetNameItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Show Name", "展示名称"},
                new string[] { "Click to show component's name.", "单击以显示运算器的名称"}, 
                Properties.Resources.ShowName, Settings, InfoGlassesProps.ShowName);

            item.DropDownItems.Add(WinFormPlus.CreateCheckItem(new string[] { "Use Nickname", "使用昵称" }, 
                new string[] { "Check to choose whether show nickname instead of name.", "点击以选择是否显示昵称而不是全称。"}, null, Settings, InfoGlassesProps.IsShowNickName));

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Name Distance", "名称距离" },
                new string[] { "Click to set Name Distance.", "点击以设置名称方框距离" }, Properties.Resources.DistanceIcon, Settings,
                InfoGlassesProps.ShowNameDistance, 500, 0));

            item.CheckedChanged += (x, y) => ResetRenderables();
            return item;
        }

        private ToolStripMenuItem GetCategoryItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Show Category", "展示类别" },
                new string[] { "Click to show component's category & subcategory.", "单击以显示运算器的类别和子类别。" },
                Properties.Resources.Category, Settings, InfoGlassesProps.ShowCategory);
            item.CheckedChanged += (x, y) => ResetRenderables();

            ToolStripMenuItem fullItem = WinFormPlus.CreateCheckItem(new string[] { "Full Category", "类别全称" },
                new string[] { "Click to choose whether show full category name.", "点击以选择是否显示类别的全称。" }, null, Settings, InfoGlassesProps.IsFullCategory);
            fullItem.CheckedChanged += (x, y) => ResetRenderables();
            item.DropDownItems.Add(fullItem);

            ToolStripMenuItem mergeItem = WinFormPlus.CreateCheckItem(new string[] { "Merge Box", "合并气泡" },
                new string[] { "Click to choose whether merge the category's box.", "点击以选择是否合成类别气泡。" }, null, Settings, InfoGlassesProps.IsMergeCateBox);
            mergeItem.CheckedChanged += (x, y) => ResetRenderables();
            item.DropDownItems.Add(mergeItem);

            return item;
        }

        private ToolStripMenuItem GetAssemblyItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Show Assembly", "展示类库" },
                new string[] { "Click to show component's assembly.", "单击以显示运算器的类库。" },
                Properties.Resources.Assembly, Settings, InfoGlassesProps.ShowAssembly);
            item.CheckedChanged += (x, y) => ResetRenderables();

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Font Size", "字体大小" },
                new string[] { "Click to set Assembly Font Size.", "点击以设置类库字体大小。" }, ArchiTed_Grasshopper.Properties.Resources.TextIcon, Settings,
                InfoGlassesProps.AssemblyFontSize, 20, 0.1));

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Assembly Width", "类库宽度" },
                new string[] { "Click to set Assembly Box Width.", "点击以设置类库气泡宽度。" }, ArchiTed_Grasshopper.Properties.Resources.SizeIcon, Settings,
                InfoGlassesProps.AssemblyWidth, 500, 10));

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Assembly Distance", "类库距离" },
                new string[] { "Click to set Assembly Box distance to component.", "点击以设置类库气泡到运算器的距离。" }, ArchiTed_Grasshopper.Properties.Resources.DistanceIcon, Settings,
                InfoGlassesProps.AssemblyDistance, 50, 0));
            return item;
        }

        private ToolStripMenuItem GetPluginItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Show Plugin", "展示插件" },
                new string[] { "Click to show whether component is plugin.", "单击以显示运算器是否为插件。" },
                Properties.Resources.PlugInIcon, Settings, InfoGlassesProps.ShowPlugin);
            item.CheckedChanged += (x, y) => ResetRenderables();

            item.DropDownItems.Add(WinFormPlus.CreateColorBoxItem(Settings, new string[] { "Plugin Color", "插件颜色" },
                new string[] { "Click to set the back highlight rectangle's color.", "单击以修改高亮矩形的颜色。" }, ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, InfoGlassesProps.PluginColor));

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Plugin Radius", "类库半径" },
                new string[] { "Click to set the back highlight rectangle's radius.", "单击以修改高亮矩形的半径。" }, ArchiTed_Grasshopper.Properties.Resources.SizeIcon, Settings,
                InfoGlassesProps.PluginRadius, 50, 1));

            return item;
        }

        #endregion

        public List<IRenderable> Renderables { get; private set; } = new List<IRenderable>();

        #region Major

        public void ResetRenderables()
        {
            this.Renderables.Clear();
            try
            {
                foreach (IGH_DocumentObject obj in Grasshopper.Instances.ActiveCanvas.Document.Objects)
                {
                    this.AddOneObject(obj);
                }
            }
            catch { }
            Grasshopper.Instances.ActiveCanvas.Refresh();
        }

        private void ActiveCanvas_Document_ObjectsAdded(GH_Document sender, GH_DocObjectEventArgs e)
        {
            foreach (var obj in e.Objects)
            {
                this.AddOneObject(obj);
            }
        }

        private void AddOneObject(IGH_DocumentObject obj)
        {
            bool showNormal = !((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).Contains(obj.ComponentGuid);
            bool showPlugin = !((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).Contains(obj.ComponentGuid);
            if (showNormal)
            {
                if ((bool)Settings.GetProperty(InfoGlassesProps.ShowName))
                {
                    Func<SizeF, RectangleF, RectangleF> layout = (x, y) =>
                    {
                        PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - (int)Settings.GetProperty(InfoGlassesProps.ShowNameDistance));
                        return CanvasRenderEngine.MiddleDownRect(pivot, x);
                    };
                    this.Renderables.Add(new NickNameOrNameTextBox(obj, layout));
                }

                string cate = (bool)Settings.GetProperty(InfoGlassesProps.IsFullCategory) ? obj.Category : Grasshopper.Instances.ComponentServer.GetCategoryShortName(obj.Category);
                string subcate = obj.SubCategory;

                if ((bool)Settings.GetProperty(InfoGlassesProps.ShowCategory))
                {

                    if ((bool)Settings.GetProperty(InfoGlassesProps.IsMergeCateBox))
                    {
                        string cateName = cate + " - " + subcate;
                        this.Renderables.Add(new TedTextBox(cateName, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - (int)Settings.GetProperty(InfoGlassesProps.ShowNameDistance) - 
                                (((bool)Settings.GetProperty(InfoGlassesProps.ShowName) ? x.Height : 0) + 3));
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, new NameBoxRenderSet()));
                    }
                    else
                    {
                        this.Renderables.Add(new TedTextBox(subcate, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - (int)Settings.GetProperty(InfoGlassesProps.ShowNameDistance) - 
                                (((bool)Settings.GetProperty(InfoGlassesProps.ShowName) ? x.Height : 0) + 3));
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, new NameBoxRenderSet()));

                        this.Renderables.Add(new TedTextBox(cate, obj, (x, y) =>
                        {
                            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - (int)Settings.GetProperty(InfoGlassesProps.ShowNameDistance) - 
                                (((bool)Settings.GetProperty(InfoGlassesProps.ShowName) ? x.Height : 0) + 3) * 2);
                            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                        }, new NameBoxRenderSet()));
                    }
                }
            }


            if ((bool)Settings.GetProperty(InfoGlassesProps.ShowAssembly) || ((bool)Settings.GetProperty(InfoGlassesProps.ShowPlugin) && showPlugin))
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
                    if (info != null)
                    {
                        location = info.Location;
                        if (!info.IsCoreLibrary)
                        {
                            if ((bool)Settings.GetProperty(InfoGlassesProps.ShowPlugin) && showPlugin)
                            {
                                this.Renderables.Add(new PluginHiRect(obj));
                            }
                        }
                    }


                }

                if (!string.IsNullOrEmpty(fullName) && (bool)Settings.GetProperty(InfoGlassesProps.ShowAssembly))
                {

                    //if (IsAutoAssem)
                    //{
                    //    if (obj is IGH_Component)
                    //    {
                    //        IGH_Component com = obj as IGH_Component;
                    //        height = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)obj.Attributes.Bounds.Width);
                    //    }
                    //    else
                    //    {
                    //        height = 0;
                    //    }

                    //    if (IsAvoidProfiler)
                    //    {
                    //        if (height == 0)
                    //            height = Math.Max(height, 16);
                    //        else
                    //            height = Math.Max(height, 32);
                    //    }
                    //}
                    //height += 5;

                    string fullStr = fullName;
                    if (location != null)
                        fullStr += "\n \n" + location;

                    this.Renderables.Add(new TedTextBox(fullStr, obj, (x, y) =>
                    {
                        float height = (int)Settings.GetProperty(InfoGlassesProps.AssemblyDistance) * 14 + 5;
                        PointF pivot = new PointF(y.Left + y.Width / 2, y.Bottom + height);
                        return CanvasRenderEngine.MiddleUpRect(pivot, x);
                    }, new AssemblyRenderSet(), (x, y, z) =>
                    {
                        return x.MeasureString(y, z, (int)Settings.GetProperty(InfoGlassesProps.AssemblyWidth));
                    }, showFunc: () => { return obj.Attributes.Selected; }));
                }
            }


        }
        #endregion


        #region PaintActions
        private void AddPaintActions()
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintGroups += ActiveCanvas_CanvasPostPaintGroups;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects += ActiveCanvas_CanvasPostPaintObjects;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintOverlay += ActiveCanvas_CanvasPrePaintOverlay;
        }

        private void RemovePaintActions()
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintGroups -= ActiveCanvas_CanvasPostPaintGroups;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects -= ActiveCanvas_CanvasPostPaintObjects;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintOverlay -= ActiveCanvas_CanvasPrePaintOverlay;
        }

        private void ActiveCanvas_CanvasPostPaintGroups(GH_Canvas sender)
        {
            ActiveRender(sender, GH_CanvasChannel.Groups);
        }

        private void ActiveCanvas_CanvasPostPaintWires(GH_Canvas sender)
        {
            ActiveRender(sender, GH_CanvasChannel.Wires);
        }

        private void ActiveCanvas_CanvasPostPaintObjects(GH_Canvas sender)
        {
            ActiveRender(sender, GH_CanvasChannel.Objects);
        }
        private void ActiveCanvas_CanvasPrePaintOverlay(GH_Canvas sender)
        {
            ActiveRender(sender, GH_CanvasChannel.Overlay);
        }
        private void ActiveRender(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            foreach (var rendarable in this.Renderables)
            {
                rendarable.RenderToCanvas(canvas, canvas.Graphics, channel);
            }
        }
        #endregion
    }
}
