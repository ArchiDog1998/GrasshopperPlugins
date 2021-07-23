using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public abstract class Respondable : Renderable_Old
    {
        public Respondable()
        {

        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            Grasshopper.Instances.ActiveCanvas.MouseClick -= MouseClick;
            Grasshopper.Instances.ActiveCanvas.MouseDoubleClick -= MouseDoubleClick;
            bool result = base.IsRender(canvas, graphics, renderLittleZoom);
            if (result)
            {
                Grasshopper.Instances.ActiveCanvas.MouseClick += MouseClick;
                Grasshopper.Instances.ActiveCanvas.MouseDoubleClick += MouseDoubleClick;
            }
            return result;
        }

        private void MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GH_Viewport vp = Grasshopper.Instances.ActiveCanvas.Viewport;
            if (vp.Zoom <= 0.5f) return;
            PointF mouseLoc = vp.UnprojectPoint(e.Location);
            if (!this.Bounds.Contains(mouseLoc)) return;

            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    break;
            }
        }

        private void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GH_Viewport vp = Grasshopper.Instances.ActiveCanvas.Viewport;
            if (vp.Zoom <= 0.5f) return;
            PointF mouseLoc = vp.UnprojectPoint(e.Location);
            if (!this.Bounds.Contains(mouseLoc)) return;

            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    break;
            }
        }
    }
}
