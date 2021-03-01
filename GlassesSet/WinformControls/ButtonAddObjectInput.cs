/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextBox = ArchiTed_Grasshopper.WinformControls.TedTextBox;

namespace InfoGlasses.WinformControls
{
    public class ButtonAddObjectInput<TGoo> : ClickButtonBase<LangWindow>, IAddObjectParam<TGoo> where TGoo : class, IGH_Goo
    {
        public GH_Param<TGoo> Target { get; }
        public int Width => 20;

        public new bool Enable => MyProxies.Length != 0;

        public string initStr => "";

        private AddProxyParams[] _myProxies;

        public AddProxyParams[] MyProxies
        {
            get
            {
                _myProxies = _myProxies ?? ParamControlHelper.GetAddProxyInputParams(this);
                return new AddProxyParams[] { new AddProxyParams(Target.ComponentGuid, 0) }.Union(_myProxies).ToArray();
            }
        }
        public RectangleF IconButtonBound => ParamControlHelper.GetIconBound(this.Bounds, true, 2);
        public new ParamGlassesComponent Owner { get; }

        public ButtonAddObjectInput(GH_Param<TGoo> target, ParamGlassesComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
            bool renderLittleZoom = false)
            : base(null, owner, null, enable, true, tips, tipsRelay, createMenu, isToggle, renderLittleZoom)
        {
            this.Target = target;
            this.Owner = owner;
        }

        public void RespondToMouseDown(object sender, MouseEventArgs e)
        {
            ParamControlHelper.AddObjectMouseDown(this, sender, e, true, init: initStr);
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            return ParamControlHelper.IsRender(this, canvas, graphics, renderLittleZoom) && base.IsRender(canvas, graphics, renderLittleZoom);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            ParamControlHelper.IconRender(this, canvas, graphics, channel);
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            this.Bounds = outerRect;
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= RespondToMouseDown;
        }

    }
}
