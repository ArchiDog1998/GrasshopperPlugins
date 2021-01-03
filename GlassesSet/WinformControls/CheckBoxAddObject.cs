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
using TextBox = ArchiTed_Grasshopper.WinformControls.TextBox;

namespace InfoGlasses.WinformControls
{
    class CheckBoxAddObject<TGoo>: ClickButtonBase<LangWindow>, IAddObjectParam, IDisposable where TGoo : class, IGH_Goo
    {
        public GH_Param<TGoo> Target { get; }
        public int Width => 20;

        public new bool Enable => MyProxies.Length != 0;

        private AddProxyParams[] _myProxies;
        public AddProxyParams[] MyProxies
        {
            get 
            { 
                if(_myProxies == null)
                {
                    foreach (var set in Owner.CreateProxyDict)
                    {
                        if (set.Key == this.Target.Type.FullName)
                        {
                            _myProxies = set.Value;
                            return _myProxies;
                        }
                    }
                    _myProxies = new AddProxyParams[] { };
                }
                return _myProxies; 
            }
        }
        public RectangleF IconButtonBound => AddObjectHelper.GetIconBound(this.Bounds, 2);


        public new ParamGlassesComponent Owner { get; }

        public CheckBoxAddObject(GH_Param<TGoo> target, ParamGlassesComponent owner, bool enable,
            string[] tips = null, int tipsRelay = 5000, Func<ToolStripDropDownMenu> createMenu = null, bool isToggle = true,
            bool renderLittleZoom = false)
            : base(null, owner, null, enable, true, tips, tipsRelay, createMenu, isToggle, renderLittleZoom)
        {
            this.Target = target;
            this.Owner = owner;
        }

        private void ActiveCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            GH_Viewport vp = Grasshopper.Instances.ActiveCanvas.Viewport;
            if (vp.Zoom >= 0.5f && this.IconButtonBound.Contains(vp.UnprojectPoint(e.Location)))
            {
                if(MyProxies.Length == 1)
                {
                    this.CreateNewObject(0);
                }
                else if(MyProxies.Length > 1)
                {
                    ContextMenuStrip menu = new ContextMenuStrip() { ShowImageMargin = true };
                    for (int i = 0; i < MyProxies.Length; i++)
                    {
                        void Item_Click(object sender1, EventArgs e1, int index)
                        {
                            this.CreateNewObject(index);
                        }
                        WinFormPlus.AddClickItem(menu, MyProxies[i].Name, null, MyProxies[i].Icon.GetIcon(true, true), i,Item_Click, false);
                    }
                    menu.Show(Grasshopper.Instances.ActiveCanvas, e.Location);
                }
            }
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= ActiveCanvas_MouseClick;
            if (Target.SourceCount > 0 || !this.Enable)
            {
                return false;
            }
            else
            {
                Grasshopper.Instances.ActiveCanvas.MouseDown += ActiveCanvas_MouseClick;
            }
            Layout(new RectangleF(), Target.Attributes.Bounds);
            return base.IsRender(canvas, graphics, renderLittleZoom);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Objects)
            {
                if (MyProxies.Length != 1)
                {
                    ParamControlHelper.RenderParamButtonIcon(graphics, this.Target.Icon_24x24, this.IconButtonBound);
                }
                else if(MyProxies.Length == 1)
                {
                    ParamControlHelper.RenderParamButtonIcon(graphics, MyProxies[0].Icon.GetIcon(!this.Target.Locked, true), this.IconButtonBound);
                }
                
            }
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            //float small = -2;
            //RectangleF rect = CanvasRenderEngine.MaxSquare(ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect));
            //rect.Inflate(small, small);
            //this.Bounds = rect;

            this.Bounds = outerRect;
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseDown -= ActiveCanvas_MouseClick;
        }

        private void CreateNewObject(int index)
        {
            AddObjectHelper.CreateNewObject(this.MyProxies, this.Target, index);
        }



    }
}
