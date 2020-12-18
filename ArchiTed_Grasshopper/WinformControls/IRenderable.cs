﻿using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface IRenderable
    {
        RectangleF Bounds { get; }
        
        void RenderToCanvas(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel);
    }
}
