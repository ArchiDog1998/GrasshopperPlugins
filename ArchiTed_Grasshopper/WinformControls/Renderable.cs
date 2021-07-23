using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public abstract class Renderable: IDisposable
    {
        /// <summary>
        /// The scope of this renderable class that maybe rendered.
        /// </summary>
        public RectangleF Bounds { get; protected set; }

        /// <summary>
        /// whether to render when viewport's zoom is less than 0.5.
        /// </summary>
        private bool _renderLittleZoom { get; }

        /// <summary>
        /// Define a class that can be rendered on the GH_Canvas.
        /// </summary>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public Renderable(bool renderLittleZoom = false)
        {
            this._renderLittleZoom = renderLittleZoom;
            Grasshopper.Instances.ActiveCanvas.CanvasPaintBegin += IsRender;
        }

        #region Render
        /// <summary>
        /// Check Whether to render.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="renderLittleZoom">whether to render when viewport's zoom is less than 0.5.</param>
        /// <returns></returns>
        protected virtual void IsRender(GH_Canvas canvas)
        {
            RemoveRenderEvent();
            if (this.Bounds == new RectangleF()) return;
            RectangleF rec = this.Bounds;
            bool result = canvas.Viewport.IsVisible(ref rec, 10f);
            if (!_renderLittleZoom)
            {
                result = result && canvas.Viewport.Zoom >= 0.5f;
            }
            if (result) AddRenderEvent();
        }

        /// <summary>
        /// How to render.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="channel">channel from render in component.</param>
        protected abstract void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel);

        private void AddRenderEvent()
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintGroups += ActiveCanvas_CanvasPostPaintGroups;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects += ActiveCanvas_CanvasPostPaintObjects;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires += ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintOverlay += ActiveCanvas_CanvasPostPaintOverlay;
        }

        private void ActiveCanvas_CanvasPostPaintOverlay(GH_Canvas sender)=>
            Render(sender, sender.Graphics, GH_CanvasChannel.Overlay);

        private void ActiveCanvas_CanvasPostPaintWires(GH_Canvas sender)=>
            Render(sender, sender.Graphics, GH_CanvasChannel.Wires);

        private void ActiveCanvas_CanvasPostPaintObjects(GH_Canvas sender)=>
            Render(sender, sender.Graphics, GH_CanvasChannel.Objects);


        private void ActiveCanvas_CanvasPostPaintGroups(GH_Canvas sender) =>
            Render(sender, sender.Graphics, GH_CanvasChannel.Groups);

        private void RemoveRenderEvent()
        {
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintGroups -= ActiveCanvas_CanvasPostPaintGroups;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintObjects -= ActiveCanvas_CanvasPostPaintObjects;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintWires -= ActiveCanvas_CanvasPostPaintWires;
            Grasshopper.Instances.ActiveCanvas.CanvasPostPaintOverlay -= ActiveCanvas_CanvasPostPaintOverlay;
        }

        public void Dispose()
        {
            try
            {
                Grasshopper.Instances.ActiveCanvas.CanvasPaintBegin -= IsRender;

            }
            catch { }
        }
        #endregion
    }
}
