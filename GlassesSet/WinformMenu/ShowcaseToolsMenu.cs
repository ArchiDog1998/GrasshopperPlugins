/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;

namespace InfoGlasses.WinformMenu
{
    public enum ShowToolsProps
    {
        IsFixCategoryIcon,
        FixCategoryFolder,
        IsUseInfoGlass,
        NormalExceptionGuid,
        PluginExceptionGuid,
        TextColor,
        BackgroundColor,
        BoundaryColor,
    }
    public class ShowcaseToolsMenu
    {
        public static SaveableSettings<ShowToolsProps> Settings { get; } = new SaveableSettings<ShowToolsProps>(new SettingsPreset<ShowToolsProps>[]
        {
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.IsFixCategoryIcon, true, (value)=>
            {
                switch (value)
                    {
                        case true:
                            foreach (string cateName in CanChangeCategoryIcon.Keys)
                            {
                                Grasshopper.Instances.ComponentServer.AddCategoryIcon(cateName, CanChangeCategoryIcon[cateName]);
                            }
                            break;
                        case false:
                            foreach (string cateName in CanChangeCategoryIcon.Keys)
                            {
                                AlreadyHave.Remove(cateName);
                            }
                            break;
                    }
                GH_ComponentServer.UpdateRibbonUI();
            }),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.FixCategoryFolder, Grasshopper.Folders.UserObjectFolders[0]),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.IsUseInfoGlass, true, (value) =>
            {

            }),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.NormalExceptionGuid, new List<Guid>() ),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.PluginExceptionGuid, new List<Guid>()),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.TextColor, Color.Black),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.BackgroundColor, Color.WhiteSmoke),
            new SettingsPreset<ShowToolsProps>(ShowToolsProps.BoundaryColor, Color.FromArgb(30, 30, 30)),

        }, Grasshopper.Instances.Settings);

        public ToolStripMenuItem CreateMajor()
        {
            ToolStripMenuItem item = WinFormPlus.CreateOneItem(new string[] { "ShowcaseTools", "展示工具" }, null, Properties.Resources.ShowcaseTools);
            //Add items.
            item.DropDown.Items.AddRange(GetFixCategoryIcons());

            GH_DocumentObject.Menu_AppendSeparator(item.DropDown);

            item.DropDown.Items.Add(InfoGlassesMajorMenuItem);
            item.DropDown.Items.Add(InfoGlassesColourMenuItem);

            GH_DocumentObject.Menu_AppendSeparator(item.DropDown);

            item.DropDown.Items.AddRange(LanguageSetting.ExtraItems);

            return item;
        }

        public ShowcaseToolsMenu()
        {
            //Add all Events.
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintGroups += ActiveCanvas_CanvasPostPaintGroups;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects += ActiveCanvas_CanvasPostPaintObjects;
            Grasshopper.Instances.ActiveCanvas.CanvasPrePaintOverlay += ActiveCanvas_CanvasPrePaintOverlay;

            //this.DropDown.MaximumSize = new Size(150, int.MaxValue);

            //Create items.
            this.InfoGlassesMajorMenuItem = GetInfoGlassesMajorItem();
            this.InfoGlassesColourMenuItem = GetInfoGlassesColourItem();



            //Change Language.
            //ResponseToLanguageChanged();
        }
        #region CategoryIcon
        private ToolStripMenuItem[] GetFixCategoryIcons()
        {
            ToolStripMenuItem folderItem = WinFormPlus.CreateClickItem(new string[] { "Change Icons' Folder", "修改图标所在文件" },
                new string[] { "Click to change the folder.\n", "单击以修改路径。\n" }, null, (x, y) =>
                {
                    //Find Dictionary Dialog.
                    IO_Helper.OpenDirectionaryDialog((folder) => Settings.SetProperty(ShowToolsProps.FixCategoryFolder, folder));

                    //Change ToolTips.
                    ((ToolStripMenuItem)x).ToolTipText = ((ToolStripMenuItem)x).ToolTipText.Split('\n')[0] + "\n \n"
                        + (string)Settings.GetProperty(ShowToolsProps.FixCategoryFolder);

                }, enable: (bool)Settings.GetProperty(ShowToolsProps.IsFixCategoryIcon));

            ToolStripMenuItem fixMajor = WinFormPlus.CreateCheckItem(new string[] { "Fix Catogory Icon", "修复类别图标" },
                new string[] { "Fix as most category icon as possible.", "修复尽可能多的类别图标。" }, Grasshopper.Instances.ComponentServer.GetCategoryIcon("Params"),
                Settings, ShowToolsProps.IsFixCategoryIcon);

            //return fixMajor.AddSubItems(folderItem);

            fixMajor.DropDownItems.Add(folderItem);
            return new ToolStripMenuItem[] { fixMajor };
        }
        #region FixCategoryIcon

        private static SortedList<string, Bitmap> _alreadyhave = null;

        private static SortedList<string, Bitmap> AlreadyHave
        {
            get
            {
                if (_alreadyhave == null)
                {
                    //find the m_guids private field.
                    IEnumerable<FieldInfo> infos = typeof(GH_ComponentServer).GetRuntimeFields().Where((info) => info.Name.Contains("_categoryIcons"));

                    // check whether to find the info.
                    if (infos.Count() == 0) throw new Exception(nameof(GH_ComponentServer) + "'s _categoryIcons can not be found!");

                    //Get the already dictionary
                    _alreadyhave = infos.ElementAt(0).GetValue(Grasshopper.Instances.ComponentServer) as SortedList<string, Bitmap>;
                }
                return _alreadyhave;
            }
        }


        private static Dictionary<string, Bitmap> _canChangeCategoryIcon = null;

        /// <summary>
        /// hold the categoryicon that can be changed!
        /// </summary>
        private static Dictionary<string, Bitmap> CanChangeCategoryIcon
        {
            get
            {
                if (_canChangeCategoryIcon == null)
                {
                    _canChangeCategoryIcon = new Dictionary<string, Bitmap>();

                    foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
                    {
                        //skip two condition.
                        if (AlreadyHave.ContainsKey(proxy.Desc.Category)) continue;
                        if (_canChangeCategoryIcon.ContainsKey(proxy.Desc.Category)) continue;
                        if (proxy.Kind != GH_ObjectType.CompiledObject) continue;

                        //Other function
                        string folderPath = (string)Settings.GetProperty(ShowToolsProps.FixCategoryFolder);
                        if (Directory.Exists(folderPath))
                        {
                            bool isSucceed = false;
                            foreach (string itemFilePath in Directory.GetFiles(folderPath))
                            {
                                if (!itemFilePath.Contains(proxy.Desc.Category)) continue;
                                try
                                {
                                    Bitmap bitmap = new Bitmap(itemFilePath);
                                    _canChangeCategoryIcon.Add(proxy.Desc.Category, bitmap);
                                    isSucceed = true;
                                    break;
                                }
                                catch { continue; }
                            }
                            if (isSucceed) continue;
                        }

                        //Find the library.
                        GH_AssemblyInfo info = Grasshopper.Instances.ComponentServer.FindAssembly(proxy.LibraryGuid);
                        if (info == null) continue;
                        if (info.Icon == null) continue;

                        //add it to dictionary.
                        _canChangeCategoryIcon.Add(proxy.Desc.Category, info.Icon);
                    }
                }
                return _canChangeCategoryIcon;
            }
        }



        #endregion
        #endregion

        public List<IRenderable> Renderables { get; set; } = new List<IRenderable>();

        #region InfoGlasses

        #region Major
        public ToolStripMenuItem InfoGlassesMajorMenuItem { get; }
        private ToolStripMenuItem GetInfoGlassesMajorItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateOneItem("", "", Properties.Resources.InfoGlasses);

            MakeRespondItem(item, ShowToolsProps.IsUseInfoGlass, () =>
            {
                //Grasshopper.Instances.ActiveCanvas.DocumentChanged += ActiveCanvas_DocumentChanged;
                //Grasshopper.Instances.ActiveCanvas.Document.ObjectsAdded += InfeGlasses_ObjectsAdded;
                //FixCategoryIconFolder.Enabled = false;
            }, () =>
            {
                //Grasshopper.Instances.ActiveCanvas.DocumentChanged -= ActiveCanvas_DocumentChanged;
                //Grasshopper.Instances.ActiveCanvas.Document.ObjectsAdded -= InfeGlasses_ObjectsAdded;
                //FixCategoryIconFolder.Enabled = false;
            });

            return item;
        }

        private void ActiveCanvas_DocumentChanged(GH_Canvas sender, GH_CanvasDocumentChangedEventArgs e)
        {
            try
            {
                e.OldDocument.ObjectsAdded -= InfeGlasses_ObjectsAdded;
            }
            catch { }
            e.NewDocument.ObjectsAdded += InfeGlasses_ObjectsAdded;
        }

        private void InfeGlasses_ObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            //foreach (var obj in e.Objects)
            //{
            //    this.AddOneObject(obj);
            //}
        }

        //private void AddOneObject(IGH_DocumentObject obj)
        //{
        //    bool showNormal = !((List<Guid>)Settings.GetProperty(ShowcaseToolsProperties.NormalExceptionGuid)).Contains(obj.ComponentGuid);
        //    bool showPlugin = !((List<Guid>)Settings.GetProperty(ShowcaseToolsProperties.PluginExceptionGuid)).Contains(obj.ComponentGuid);
        //    if (showNormal)
        //    {

        //        Font nameFont = new Font(GH_FontServer.Standard.FontFamily, NameBoxFontSize);
        //        TextBoxRenderSet nameSet = new TextBoxRenderSet((Color)Settings.GetProperty(ShowcaseToolsProperties.BackgroundColor),
        //            (Color)Settings.GetProperty(ShowcaseToolsProperties.BoundaryColor), nameFont, 
        //            (Color)Settings.GetProperty(ShowcaseToolsProperties.TextColor));
        //        if (this.IsShowName)
        //        {
        //            Func<SizeF, RectangleF, RectangleF> layout = (x, y) =>
        //            {
        //                PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance);
        //                return CanvasRenderEngine.MiddleDownRect(pivot, x);
        //            };
        //            this.RenderObjs.Add(new NickNameOrNameTextBox(this.IsShowNickName, obj, layout, nameSet));
        //        }

        //        string cate = IsShowFullCate ? obj.Category : Grasshopper.Instances.ComponentServer.GetCategoryShortName(obj.Category);
        //        string subcate = obj.SubCategory;

        //        if (this.IsShowCategory)
        //        {

        //            if (IsMergeCateBox)
        //            {
        //                string cateName = cate + " - " + subcate;
        //                this.RenderObjs.Add(new TextBox(cateName, obj, (x, y) =>
        //                {
        //                    PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3));
        //                    return CanvasRenderEngine.MiddleDownRect(pivot, x);
        //                }, nameSet));
        //            }
        //            else
        //            {
        //                this.RenderObjs.Add(new TextBox(subcate, obj, (x, y) =>
        //                {
        //                    PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3));
        //                    return CanvasRenderEngine.MiddleDownRect(pivot, x);
        //                }, nameSet));

        //                this.RenderObjs.Add(new TextBox(cate, obj, (x, y) =>
        //                {
        //                    PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3) * 2);
        //                    return CanvasRenderEngine.MiddleDownRect(pivot, x);
        //                }, nameSet));
        //            }
        //        }
        //    }


        //    if ((this.IsShowAssem) || (this.IsShowPlugin && showPlugin))
        //    {
        //        string fullName = "";
        //        string location = "";

        //        Type type = obj.GetType();
        //        if (type != null)
        //        {
        //            fullName = type.FullName;

        //            GH_AssemblyInfo info = null;
        //            foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
        //            {
        //                if (lib.Assembly == obj.GetType().Assembly)
        //                {
        //                    info = lib;
        //                    break;
        //                }
        //            }
        //            if (info != null)
        //            {
        //                location = info.Location;
        //                if (!info.IsCoreLibrary)
        //                {
        //                    if (IsShowPlugin && showPlugin)
        //                    {
        //                        this.RenderObjsUnderComponent.Add(new HighLightRect(obj, PluginHighLightColor, HighLightRadius));
        //                    }
        //                }
        //            }


        //        }

        //        if (!string.IsNullOrEmpty(fullName) && this.IsShowAssem)
        //        {
        //            float height = AssemBoxHeight * 14;
        //            if (IsAutoAssem)
        //            {
        //                if (obj is IGH_Component)
        //                {
        //                    IGH_Component com = obj as IGH_Component;
        //                    height = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)obj.Attributes.Bounds.Width);
        //                }
        //                else
        //                {
        //                    height = 0;
        //                }

        //                if (IsAvoidProfiler)
        //                {
        //                    if (height == 0)
        //                        height = Math.Max(height, 16);
        //                    else
        //                        height = Math.Max(height, 32);
        //                }
        //            }
        //            height += 5;

        //            Font assemFont = new Font(GH_FontServer.Standard.FontFamily, AssemFontSize);
        //            TextBoxRenderSet assemSet = new TextBoxRenderSet(Color.FromArgb(BackGroundColor.A / 2, BackGroundColor), BoundaryColor, assemFont, TextColor);
        //            string fullStr = fullName;
        //            if (location != null)
        //                fullStr += "\n \n" + location;

        //            this.RenderObjs.Add(new TextBox(fullStr, obj, (x, y) =>
        //            {
        //                PointF pivot = new PointF(y.Left + y.Width / 2, y.Bottom + height);
        //                return CanvasRenderEngine.MiddleUpRect(pivot, x);
        //            }, assemSet, (x, y, z) =>
        //            {
        //                return x.MeasureString(y, z, AssemBoxWidth);
        //            }, showFunc: () => { return obj.Attributes.Selected; }));
        //        }
        //    }


        //}
        #endregion

        #region Colour
        public ToolStripMenuItem InfoGlassesColourMenuItem { get; }
        public ToolStripMenuItem GetInfoGlassesColourItem()
        {
            WinFormPlus.ItemSet<ShowToolsProps>[] sets = new WinFormPlus.ItemSet<ShowToolsProps>[]
            {
                new WinFormPlus.ItemSet<ShowToolsProps>(new string[] { "Text Color", "文字颜色" }, new string[] { "Adjust text color.", "调整文字颜色。" },
                    null, true, ShowToolsProps.TextColor),

                new WinFormPlus.ItemSet<ShowToolsProps>(new string[] { "Background Color", "背景颜色" }, new string[] { "Adjust background color.", "调整背景颜色。" },
                    null, true, ShowToolsProps.BackgroundColor),

                new WinFormPlus.ItemSet<ShowToolsProps>(new string[] { "Boundary Color", "边框颜色"  }, new string[] { "Adjust boundary color.", "调整边框颜色。" },
                    null, true, ShowToolsProps.BoundaryColor),
            };

            return WinFormPlus.CreateColorBoxItems(Settings, new string[] { "Colors", "颜色" }, new string[] { "Adjust color.", "调整颜色。" }, 
                ArchiTed_Grasshopper.Properties.Resources.ColorIcon, true, sets);
        }
        #endregion

        #endregion

        private void MakeRespondItem(ToolStripMenuItem item, ShowToolsProps name, Action checkAction, Action uncheckAction)
        {
            #region Define event.
            void Item_Click(object sender, EventArgs e)
            {
                Settings.SetProperty(name, !(bool)Settings.GetProperty(name));
                item.Checked = (bool)Settings.GetProperty(name);
            }

            void Item_CheckedChanged(object sender, EventArgs e)
            {
                switch (item.Checked)
                {
                    case true:
                        checkAction.Invoke();
                        break;
                    case false:
                        uncheckAction.Invoke();
                        break;
                }
            }
            #endregion

            item.Checked = (bool)Settings.GetProperty(name);
            item.Click += Item_Click;
            item.CheckedChanged += Item_CheckedChanged;
            Item_CheckedChanged(null, new EventArgs());
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
    }
}
