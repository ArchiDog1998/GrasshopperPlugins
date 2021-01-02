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
    class CheckBoxAddObject<TGoo>: ClickButtonBase<LangWindow>, IDisposable where TGoo : class, IGH_Goo
    {
        public GH_Param<TGoo> Target { get; }
        public int Width => 20;

        private AddProxyParams[] _myProxies;

        public new bool Enable => MyProxies.Length != 0;

        public AddProxyParams[] MyProxies
        {
            get 
            { 
                if(_myProxies == null)
                {
                    FindProxies();
                }
                return _myProxies; 
            }
        }


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
            if (vp.Zoom >= 0.5f && this.Bounds.Contains(vp.UnprojectPoint(e.Location)))
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
            Grasshopper.Instances.ActiveCanvas.MouseClick -= ActiveCanvas_MouseClick;
            if (Target.SourceCount > 0 || !this.Enable)
            {
                return false;
            }
            else
            {
                Grasshopper.Instances.ActiveCanvas.MouseClick += ActiveCanvas_MouseClick;
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
                    ParamControlHelper.RenderParamButtonIcon(graphics, this.Target.Icon_24x24, this.Bounds);

                    //float size = 8;
                    //float width = 2;
                    //float dis = 4;

                    //Color showColor = ColorExtension.OnColor;
                    //RectangleF rect = new RectangleF(new PointF(this.Bounds.Location.X - size - dis, this.Bounds.Location.Y), new SizeF(size, size));
                    //Pen drawPen = new Pen(showColor, width);
                    //graphics.DrawLine(drawPen, new Point((int)rect.Left, (int)(rect.Top + rect.Height / 2)),
                    //    new Point((int)rect.Right, (int)(rect.Top + rect.Height / 2)));
                    //graphics.DrawLine(drawPen, new Point((int)(rect.Left + rect.Width / 2), (int)rect.Top),
                    //     new Point((int)(rect.Left + rect.Width / 2), (int)rect.Bottom));
                }
                else if(MyProxies.Length == 1)
                {
                    ParamControlHelper.RenderParamButtonIcon(graphics, MyProxies[0].Icon.GetIcon(!this.Target.Locked, true), this.Bounds);
                }
                
            }
        }

        public override void Layout(RectangleF innerRect, RectangleF outerRect)
        {
            float small = -2;
            RectangleF rect = CanvasRenderEngine.MaxSquare(ParamControlHelper.ParamLayoutBase(this.Target.Attributes, Width, outerRect));
            rect.Inflate(small, small);
            this.Bounds = rect;
        }

        public void Dispose()
        {
            Grasshopper.Instances.ActiveCanvas.MouseClick -= ActiveCanvas_MouseClick;
        }

        private void FindProxies()
        {
            foreach (var set in Owner.CreateProxyDict)
            {
                if (set.Key == this.Target.Type.FullName)
                {
                    _myProxies = set.Value;
                    return;
                }
            }
            _myProxies = new AddProxyParams[] { };
        }

        private void CreateNewObject(int index)
        {
            CreateNewObject(this.MyProxies, this.Target, index);
        }


        internal static void CreateNewObject(AddProxyParams[] proxies, IGH_Param target, int index = 0)
        {
            if (proxies.Length < index) return;


            IGH_DocumentObject obj = Grasshopper.Instances.ComponentServer.EmitObject(proxies[index].Guid);
            if (obj == null)
            {
                return;
            }

            CreateNewObject(obj, target, proxies[index].OutIndex);
        }

        internal static void CreateNewObject(IGH_DocumentObject obj, IGH_Param target, int outIndex = 0, float leftMove = 100, string init = null)
        {

            if (obj == null)
            {
                return;
            }

            PointF comRightCenter = new PointF(target.Attributes.Bounds.Left - leftMove,
                target.Attributes.Bounds.Top + target.Attributes.Bounds.Height / 2);
            if (obj is GH_Component)
            {
                GH_Component com = obj as GH_Component;

                AddAObjectToCanvas(com, comRightCenter, false, init);

                target.AddSource(com.Params.Output[outIndex]);
                com.Params.Output[outIndex].Recipients.Add(target);

                target.OnPingDocument().NewSolution(false);
            }
            else if (obj is IGH_Param)
            {
                IGH_Param param = obj as IGH_Param;

                AddAObjectToCanvas(param, comRightCenter, false, init);

                target.AddSource(param);
                param.Recipients.Add(target);

                target.OnPingDocument().NewSolution(false);
            }
            else
            {
                target.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, LanguagableComponent.GetTransLation(new string[]
                {
                    "The added object is not a Component or Parameters!", "添加的对象不是一个运算器或参数！",
                }));
            }
        }


        internal static void AddAObjectToCanvas(IGH_DocumentObject obj, PointF pivot, bool update, string init = null)
        {
            var functions = typeof(GH_Canvas).GetRuntimeMethods().Where(m => m.Name.Contains("InstantiateNewObject") && !m.IsPublic).ToArray();
            if(functions.Length > 0)
            {
                functions[0].Invoke(Grasshopper.Instances.ActiveCanvas, new object[] { obj, init, pivot, update });
            }
        }
    }
}
