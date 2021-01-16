/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using System.Linq;

namespace ArchiTed_Grasshopper
{
    public abstract class ControllableComponent : GH_Component
    {
        public List<IRenderable> RenderObjsUnderComponent { get; protected set; }

        public List<IRenderable> RenderObjs { get; protected set; }

        public IRespond[] Controls { get; protected set; }

        public Type WindowsType { get; }

        public PointF InputLayoutMove { get; protected set; } 

        public PointF OutputLayoutMove { get; protected set; }

        public override void CreateAttributes()
        {
            base.m_attributes = new ControllableComponentAttribute(this);
        }

        /// <summary>
        /// Initializes a new instance of the ButtonableComponent class.
        /// </summary>
        protected ControllableComponent(string name, string nickname, string description, string category, string subcategory, Type windowsType = null)
          : base(name, nickname, description, category, subcategory)
        {
            RenderObjsUnderComponent = new List<IRenderable>();
            RenderObjs = new List<IRenderable>();
            Controls = new IRespond[] { };

            if (typeof(LangWindow).IsAssignableFrom(windowsType))
                this.WindowsType = windowsType;
            else if (windowsType == null)
                this.WindowsType = typeof(LangWindow);
            else throw new ArgumentOutOfRangeException("Windows Type");

            this.InputLayoutMove = new PointF();
            this.OutputLayoutMove = new PointF();
        }

        protected abstract override void RegisterInputParams(GH_Component.GH_InputParamManager pManager);

        protected abstract override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager);

        protected abstract override void SolveInstance(IGH_DataAccess DA);

        public abstract void CreateWindow();

        #region Attribute
        public virtual void ChangeParamsLayout()
        {
            foreach (var input in this.Params.Input)
            {
                input.Attributes.Bounds = new RectangleF(new PointF(input.Attributes.Bounds.X + this.InputLayoutMove.X, input.Attributes.Bounds.Y + this.InputLayoutMove.Y), input.Attributes.Bounds.Size);
                ChangeRenderTags((GH_LinkedParamAttributes)input.Attributes, this.InputLayoutMove);
            }
            foreach (var output in this.Params.Output)
            {
                output.Attributes.Bounds = new RectangleF(new PointF(output.Attributes.Bounds.X + this.OutputLayoutMove.X, output.Attributes.Bounds.Y + this.OutputLayoutMove.Y), output.Attributes.Bounds.Size);
                ChangeRenderTags((GH_LinkedParamAttributes)output.Attributes, this.OutputLayoutMove);

            }
        }

        public static void ChangeRenderTags(GH_LinkedParamAttributes gH_LinkedParamAttributes, PointF rectMove)
        {
            if (gH_LinkedParamAttributes == null) return;
            FieldInfo field = typeof(GH_LinkedParamAttributes).GetRuntimeFields().Where(m => m.Name.Contains("m_renderTags")).First();
            if (field == null)
            {
                System.Windows.Forms.MessageBox.Show("Can't find the Tags!");
                return;
            }
            GH_StateTagList tagList = field.GetValue(gH_LinkedParamAttributes) as GH_StateTagList;
            if (tagList == null) return;
            foreach (var tag in tagList)
            {
                tag.Stage = new Rectangle(new System.Drawing.Point(tag.Stage.X + (int)rectMove.X, tag.Stage.Y + (int)rectMove.Y), tag.Stage.Size);
            }
        }

        public virtual void BeforeRender(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel) { }

        public virtual void AfterRender(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel) { }
        #endregion

        #region GetSet
        public void SetValuePub(string name, bool value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Bool Value Changed");
            SetValue(name, value);
        }
        public void SetValuePub(string name, int value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Int Value Changed");
            SetValue(name, value);
        }
        public void SetValuePub(string name, double value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Double Value Changed");
            SetValue(name, value);
        }
        public void SetValuePub(string name, Color value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Color Value Changed");
            SetValue(name, value);
        }
        public void SetValuePub(string name, string value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("String Value Changed");
            SetValue(name, value);
        }

        public bool GetValuePub(string name, bool value)
        {
            if (string.IsNullOrEmpty(name)) return default(bool);
            return GetValue(name, value);
        }
        public int GetValuePub(string name, int value)
        {
            if (string.IsNullOrEmpty(name)) return default(int);
            return GetValue(name, value);
        }
        public double GetValuePub(string name, double value)
        {
            if (string.IsNullOrEmpty(name)) return default(double);
            return GetValue(name, value);
        }
        public Color GetValuePub(string name, Color value)
        {
            if (string.IsNullOrEmpty(name)) return default(Color);
            return GetValue(name, value);
        }
        public string GetValuePub(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) return default(string);
            return GetValue(name, value);
        }
        #endregion


    }
}