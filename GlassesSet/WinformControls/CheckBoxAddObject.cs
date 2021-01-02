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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformControls
{
    class CheckBoxAddObject<TGoo>: ClickButtonBase<LangWindow>, IDisposable where TGoo : class, IGH_Goo
    {
        public GH_Param<TGoo> Target { get; }
        public int Width => 20;

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
                this.CreateNewObject();
            }
        }

        protected override bool IsRender(GH_Canvas canvas, Graphics graphics, bool renderLittleZoom = false)
        {
            Grasshopper.Instances.ActiveCanvas.MouseClick -= ActiveCanvas_MouseClick;
            if (Target.SourceCount > 0)
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
                Color showColor = this.Enable ? ColorExtension.OffColor : ColorExtension.UnableColor;
                Pen drawPen = new Pen(showColor, 3);
                graphics.DrawLine(drawPen, new Point((int)this.Bounds.Left, (int)(this.Bounds.Top + this.Bounds.Height / 2)),
                    new Point((int)this.Bounds.Right, (int)(this.Bounds.Top + this.Bounds.Height / 2)));
                graphics.DrawLine(drawPen, new Point((int)(this.Bounds.Left + this.Bounds.Width / 2), (int)this.Bounds.Top),
                     new Point((int)(this.Bounds.Left + this.Bounds.Width / 2), (int)this.Bounds.Bottom));
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

        private void CreateNewObject()
        {
            float leftMove = 100;
            //Finde the right action and invoke it!
            foreach (var set in Owner.CreateProxyDict)
            {
                if(set.Key == this.Target.Type.FullName)
                {

                    IGH_DocumentObject obj = Grasshopper.Instances.ComponentServer.EmitObject(set.Value.Guid);
                    if (obj == null)
                    {
                        return;
                    }
                    if (obj is GH_Component)
                    {
                        GH_Component com = obj as GH_Component;

                        PointF comRightCenter = new PointF(Target.Attributes.Bounds.Left - leftMove,
                                Target.Attributes.Bounds.Top + Target.Attributes.Bounds.Height / 2);

                        this.AddAComponentToCanvas(com, comRightCenter, false);

                        Target.AddSource(com.Params.Output[set.Value.OutIndex]);
                        com.Params.Output[set.Value.OutIndex].Recipients.Add(this.Target);

                        this.Target.OnPingDocument().NewSolution(false);
                    }
                    else
                    {
                        Target.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, LanguagableComponent.GetTransLation(new string[]
                        {
                                    "The added object is not a Component!", "添加的对象不是一个运算器！",
                        }));
                    }
                    return;
                }
            }

            this.Target.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, LanguagableComponent.GetTransLation(new string[]
            {
                "Need to define which component should be created!", "需要定义哪个运算器是用来添加的！",
            }));
        }

        private void AddAComponentToCanvas(IGH_DocumentObject obj, PointF pivot, bool update)
        {
            var functions = typeof(GH_Canvas).GetRuntimeMethods().Where(m => m.Name.Contains("InstantiateNewObject") && !m.IsPublic).ToArray();
            if(functions.Length > 0)
            {
                functions[0].Invoke(Grasshopper.Instances.ActiveCanvas, new object[] { obj, null, pivot, update });
            }
        }
    }
}
