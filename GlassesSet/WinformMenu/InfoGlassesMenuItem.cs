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
            ShowFont,
            TextColor,
            BackgroundColor,
            BoundaryColor,
            NormalExceptionGuid,
            PluginExceptionGuid,
        }

        public static SaveableSettings<InfoGlassesProps> Settings { get; } = new SaveableSettings<InfoGlassesProps>(new SettingsPreset<InfoGlassesProps>[]
        {
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.IsUseInfoGlass, true, (value) =>
            {

            }),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.ShowFont, GH_FontServer.Standard),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.TextColor, Color.Black),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BackgroundColor, Color.WhiteSmoke),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.BoundaryColor, Color.FromArgb(30, 30, 30)),

            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.NormalExceptionGuid, new List<Guid>()),
            new SettingsPreset<InfoGlassesProps>(InfoGlassesProps.PluginExceptionGuid, new List<Guid>()),

        }, Grasshopper.Instances.Settings);

        public InfoGlassesMenuItem(): base(Properties.Resources.InfoGlasses)
        {
            this.SetItemLangChange(new string[] { "InfoGlasses", "信息眼镜" }, null);
            this.AddCheckProperty(Settings, InfoGlassesProps.IsUseInfoGlass);

            this.DropDown.Items.Add(WinFormPlus.CreateFontSelector(new string[] { "Display Font", "展示字体" },
                new string[] { "Click to change the display font.", "单击以修改展示字体。" }, Settings, InfoGlassesProps.ShowFont));
            this.DropDown.Items.Add(GetInfoGlassesColourItem());

            //Add all Events.
            AddPaintActions();
        }

        #region Colour
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
        #endregion

        public List<IRenderable> Renderables { get; private set; } = new List<IRenderable>();

        #region InfoGlasses

        #region Major
        public ToolStripMenuItem InfoGlassesMajorMenuItem { get; }

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

        #endregion



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
