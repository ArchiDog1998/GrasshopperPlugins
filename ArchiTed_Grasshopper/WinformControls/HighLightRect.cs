/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

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
    /// Height light a GH_DocumentObject.
    /// </summary>
    public class HighLightRect : RenderItem
    {

        /// <summary>
        /// HighLight color.
        /// </summary>
        public virtual Color Color { get; }

        /// <summary>
        /// cornerRadius.
        /// </summary>
        public virtual int Radius { get; }

        /// <summary>
        /// disable when the object first add to the document.
        /// </summary>
        private bool IsNotFirstRender = false;

        /// <summary>
        ///  Height light a GH_DocumentObject.
        /// </summary>
        /// <param name="target">the GH_DocumentObject that needs to high light. </param>
        /// <param name="color">HighLight color.</param>
        /// <param name="radius"> cornerRadius. </param>
        /// <param name="showFunc"> whether to show. </param>
        /// <param name="renderLittleZoom">  whether render when zoom is less than 0.5. </param>
        public HighLightRect(IGH_DocumentObject target, Color color, int radius, Func<bool> showFunc = null, 
            bool renderLittleZoom = false)
            : base(target,showFunc, renderLittleZoom)
        {
            this.Color = color;
            this.Radius = radius;
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            //flag = Math.Pow(this.Bounds.X + this.Bounds.Width / 2, 2) + Math.Pow(this.Bounds.Y + this.Bounds.Height / 2, 2) > Math.Pow(3, 2);
            if (channel == GH_CanvasChannel.Groups && IsNotFirstRender)
            {
                GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(this.Bounds, GH_Palette.Pink, this.Radius, 0);
                gH_Capsule.Render(graphics, this.Color);
                gH_Capsule.Dispose();
            }
            IsNotFirstRender = true;
        }

        protected override RectangleF Layout(GH_Canvas canvas, Graphics graphics, RectangleF rect)
        {
            RectangleF newRect = rect;
            if (this.Target is GH_Component)
            {
                GH_Component com = this.Target as GH_Component;
                int height = CanvasRenderEngine.MessageBoxHeight(com.Message, (int)rect.Width);
                newRect = new RectangleF(rect.Location, new SizeF(rect.Width, rect.Height + height));
            }
            newRect.Inflate(this.Radius, this.Radius);
            return newRect;
        }
    }
}
