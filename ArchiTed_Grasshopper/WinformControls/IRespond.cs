using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface IRespond: IRenderable
    {
        ControllableComponent Owner { get; }
        bool Enable { get; set; }

        void Layout(RectangleF innerRect, RectangleF outerRect);
        bool RespondToMouseDownPub(GH_Canvas sender, GH_CanvasMouseEvent e);
        bool RespondToMouseMovePub(GH_Canvas sender, GH_CanvasMouseEvent e);
        bool RespondToMouseUpPub(GH_Canvas sender, GH_CanvasMouseEvent e);
        bool RespondToMouseDoubleClickPub(GH_Canvas sender, GH_CanvasMouseEvent e);

    }

}
