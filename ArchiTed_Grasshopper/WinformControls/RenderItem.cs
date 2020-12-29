/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.GUI.Canvas;

namespace ArchiTed_Grasshopper.WinformControls
{
    /// <summary>
    /// Define a class that only need to render on GH_Canvas.
    /// </summary>
    public abstract class RenderItem : Renderable
    {
        /// <summary>
        /// Which documentObject that should rely on.
        /// </summary>
        public IGH_DocumentObject Target { get; }

        /// <summary>
        /// Define whether to show it. 
        /// </summary>
        protected Func<bool> ShowFunc { get; }

        /// <summary>
        /// Define a class that only need to render on GH_Canvas.
        /// </summary>
        /// <param name="target">Which documentObject that should rely on.</param>
        /// <param name="showFunc">Define whether to show it. </param>
        /// <param name="renderLittleZoom">whether to render when viewport's zoom is less than 0.5.</param>
        public RenderItem(IGH_DocumentObject target, Func<bool> showFunc = null, bool renderLittleZoom = false)
            : base(renderLittleZoom)
        {
            this.Target = target;
            this.ShowFunc = showFunc ?? (() => { return true; });
        }

        /// <summary>
        /// Define the Bounds.
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="rect">The target's bounds.</param>
        /// <returns> this bounds. </returns>
        protected abstract RectangleF Layout(GH_Canvas canvas, Graphics graphics, RectangleF rect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas">canvas from render in component.</param>
        /// <param name="graphics">graphics from render in component.</param>
        /// <param name="renderLittleZoom">whether to render when viewport's zoom is less than 0.5.</param>
        /// <returns> whether to render. </returns>
        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            if (Target.OnPingDocument() == null || !this.ShowFunc())
                return false;
            this.Bounds = Layout(canvas, graphics, this.Target.Attributes.Bounds);
            return base.IsRender(canvas, graphics, renderLittleZoom);
        }
    }
}
