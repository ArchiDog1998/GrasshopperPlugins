using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Define a class that can be rendered on the GH_Canvas.
    /// </summary>
    public abstract class Renderable: IRenderable
    {
        /// <summary>
        /// The scope of this renderable class that maybe rendered.
        /// </summary>
        public RectangleF Bounds { get; protected set; }

        /// <summary>
        /// whether to render when viewport's zoom is less than 0.5.
        /// </summary>
        private bool RenderLittleZoom { get; }

        /// <summary>
        /// Define a class that can be rendered on the GH_Canvas.
        /// </summary>
        /// <param name="renderLittleZoom">Whether to render when viewport's zoom is less than 0.5.</param>
        public Renderable(bool renderLittleZoom = false)
        {
            this.RenderLittleZoom = renderLittleZoom;
        }


        #region Render
        /// <summary>
        /// Check Whether to render.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="renderLittleZoom">whether to render when viewport's zoom is less than 0.5.</param>
        /// <returns></returns>
        protected virtual bool IsRender(GH_Canvas canvas,  Graphics graphics,  bool renderLittleZoom = false)
        {
           
            RectangleF rec = Bounds;
            bool result = canvas.Viewport.IsVisible(ref rec, 10f);
            this.Bounds = rec;
            if (!renderLittleZoom)
            {
                result = result && canvas.Viewport.Zoom >= 0.5f;
            }
            return result;
        }

        /// <summary>
        /// Render to canvas to raise up.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="channel">channel from render in component.</param>
        public void RenderToCanvas(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (IsRender(canvas,graphics, RenderLittleZoom))
                Render(canvas, graphics, channel);
        }

        /// <summary>
        /// How to render.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="channel">channel from render in component.</param>
        protected abstract void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel);
        #endregion
    }
}
