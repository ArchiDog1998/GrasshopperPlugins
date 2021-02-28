/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformMenu
{
    internal class InfoGlassesMenuItem : ToolStripMenuItem
    {
        internal enum InfoGlassesProps
        {
            IsUseInfoGlass,

            NormalExceptionGuid,
            PluginExceptionGuid,

            ShowFont,
            TextColor,
            BackgroundColor,
            BoundaryColor,

            ShowName,
            ShowNameType,
            ShowNameDistance,
        }

        public static SaveableSettings<InfoGlassesProps> Settings { get; } = new SaveableSettings<InfoGlassesProps>(new SettingsPreset<InfoGlassesProps>[]
        {
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsUseInfoGlass, true, (value) =>
            {

            }),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.NormalExceptionGuid, new List<Guid>()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.PluginExceptionGuid, new List<Guid>()),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowFont, GH_FontServer.Standard),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.TextColor, Color.Black),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BackgroundColor, Color.WhiteSmoke),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BoundaryColor, Color.FromArgb(30, 30, 30)),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowName, true, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowNameType, 0, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowNameDistance, 3, (value)=>Grasshopper.Instances.ActiveCanvas.Refresh()),

        }, Grasshopper.Instances.Settings);

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

            this.DropDown.Items.Add(WinFormPlus.CreateOneItem(new string[] { "Exceptions", "除去项" },
                new string[] { "Click to open the exceptions window.", "单击以打开除去项窗口。" }, Properties.Resources.ExceptionIcon));
            this.DropDown.Items.Add(WinFormPlus.CreateFontSelector(new string[] { "Display Font", "展示字体" },
                new string[] { "Click to change the display font.", "单击以修改展示字体。" }, Settings, InfoGlassesProps.ShowFont));
            this.DropDown.Items.Add(GetInfoGlassesColourItem());

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(GetNameItem());

            Grasshopper.Instances.ActiveCanvas.Document_ObjectsAdded += ActiveCanvas_Document_ObjectsAdded;
            Grasshopper.Instances.DocumentServer.DocumentAdded += (x, y) => ResetRenderables();
            Grasshopper.Instances.ActiveCanvas.DocumentChanged += (x, y) => ResetRenderables();
        }



        public ToolStripMenuItem GetInfoGlassesColourItem()
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

        public ToolStripMenuItem GetNameItem()
        {
            ToolStripMenuItem item = WinFormPlus.CreateCheckItem(new string[] { "Show Name", "展示名称"},
                new string[] { "Click to show component's name.", "单击以显示运算器的名称"}, 
                Properties.Resources.ShowName, Settings, InfoGlassesProps.ShowName);

            item.DropDownItems.Add(WinFormPlus.CreateLoopBoxItem(null, null, Settings, InfoGlassesProps.ShowNameType, new string[][]
            {
                new string[]{ "Use Name", "使用名称"}, 
                new string[]{ "Use Nickname", "使用昵称"},
            }));

            item.DropDownItems.Add(WinFormPlus.CreateNumberBox(new string[] { "Name Distance", "名称距离" },
                new string[] { "Click to set Name Distance.", "点击以设置名称方框距离" }, Properties.Resources.DistanceIcon, Settings,
                InfoGlassesProps.ShowNameDistance, 500, 0));

            return item;
        }

        public List<IRenderable> Renderables { get; private set; } = new List<IRenderable>();


        #region Major

        private void ResetRenderables()
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
                TextBoxRenderSet nameSet = new TextBoxRenderSet((Color)Settings.GetProperty(InfoGlassesProps.BackgroundColor),
                    (Color)Settings.GetProperty(InfoGlassesProps.BoundaryColor),
                    (Font)Settings.GetProperty(InfoGlassesProps.ShowFont),
                    (Color)Settings.GetProperty(InfoGlassesProps.TextColor));
                if ((bool)Settings.GetProperty(InfoGlassesProps.ShowName))
                {
                    Func<SizeF, RectangleF, RectangleF> layout = (x, y) =>
                    {
                        //((decimal)Settings.GetProperty(InfoGlassesProps.ShowNameDistance))
                        PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - (int)Settings.GetProperty(InfoGlassesProps.ShowNameDistance));
                        return CanvasRenderEngine.MiddleDownRect(pivot, x);
                    };
                    this.Renderables.Add(new NickNameOrNameTextBox((int)Settings.GetProperty(InfoGlassesProps.ShowNameType)==1, obj, layout, nameSet));
                }

                //string cate = IsShowFullCate ? obj.Category : Grasshopper.Instances.ComponentServer.GetCategoryShortName(obj.Category);
                //string subcate = obj.SubCategory;

                //if (this.IsShowCategory)
                //{

                //    if (IsMergeCateBox)
                //    {
                //        string cateName = cate + " - " + subcate;
                //        this.RenderObjs.Add(new TextBox(cateName, obj, (x, y) =>
                //        {
                //            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3));
                //            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                //        }, nameSet));
                //    }
                //    else
                //    {
                //        this.RenderObjs.Add(new TextBox(subcate, obj, (x, y) =>
                //        {
                //            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3));
                //            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                //        }, nameSet));

                //        this.RenderObjs.Add(new TextBox(cate, obj, (x, y) =>
                //        {
                //            PointF pivot = new PointF(y.Left + y.Width / 2, y.Top - NameBoxDistance - ((this.IsShowName ? x.Height : 0) + 3) * 2);
                //            return CanvasRenderEngine.MiddleDownRect(pivot, x);
                //        }, nameSet));
                //    }
                //}
            }


            //if ((this.IsShowAssem) || (this.IsShowPlugin && showPlugin))
            //{
            //    string fullName = "";
            //    string location = "";

            //    Type type = obj.GetType();
            //    if (type != null)
            //    {
            //        fullName = type.FullName;

            //        GH_AssemblyInfo info = null;
            //        foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
            //        {
            //            if (lib.Assembly == obj.GetType().Assembly)
            //            {
            //                info = lib;
            //                break;
            //            }
            //        }
            //        if (info != null)
            //        {
            //            location = info.Location;
            //            if (!info.IsCoreLibrary)
            //            {
            //                if (IsShowPlugin && showPlugin)
            //                {
            //                    this.RenderObjsUnderComponent.Add(new HighLightRect(obj, PluginHighLightColor, HighLightRadius));
            //                }
            //            }
            //        }


            //    }

            //    if (!string.IsNullOrEmpty(fullName) && this.IsShowAssem)
            //    {
            //        float height = AssemBoxHeight * 14;
            //        if (IsAutoAssem)
            //        {
            //            if (obj is IGH_Component)
            //            {
            //                IGH_Component com = obj as IGH_Component;
            //                height = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)obj.Attributes.Bounds.Width);
            //            }
            //            else
            //            {
            //                height = 0;
            //            }

            //            if (IsAvoidProfiler)
            //            {
            //                if (height == 0)
            //                    height = Math.Max(height, 16);
            //                else
            //                    height = Math.Max(height, 32);
            //            }
            //        }
            //        height += 5;

            //        Font assemFont = new Font(GH_FontServer.Standard.FontFamily, AssemFontSize);
            //        TextBoxRenderSet assemSet = new TextBoxRenderSet(Color.FromArgb(BackGroundColor.A / 2, BackGroundColor), BoundaryColor, assemFont, TextColor);
            //        string fullStr = fullName;
            //        if (location != null)
            //            fullStr += "\n \n" + location;

            //        this.RenderObjs.Add(new TextBox(fullStr, obj, (x, y) =>
            //        {
            //            PointF pivot = new PointF(y.Left + y.Width / 2, y.Bottom + height);
            //            return CanvasRenderEngine.MiddleUpRect(pivot, x);
            //        }, assemSet, (x, y, z) =>
            //        {
            //            return x.MeasureString(y, z, AssemBoxWidth);
            //        }, showFunc: () => { return obj.Attributes.Selected; }));
            //    }
            //}


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
