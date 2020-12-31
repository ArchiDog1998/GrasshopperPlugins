/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using ArchiTed_Grasshopper.WinformControls;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.GUI.Canvas;

namespace ArchiTed_Grasshopper
{
    public abstract class ControllableComponent : GH_Component
    {
        public List<IRenderable> RenderObjsUnderComponent { get; protected set; }

        public List<IRenderable> RenderObjs { get; protected set; }

        public IRespond[] Controls { get; protected set; }

        public Type WindowsType { get; }

        public Func<RectangleF, RectangleF> ChangeInputLayout { get; protected set; }

        public Func<RectangleF, RectangleF> ChangeOutputLayout { get; protected set; }

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

            ChangeInputLayout = (x) => x;
            ChangeOutputLayout = (x) => x;

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
                input.Attributes.Bounds = this.ChangeInputLayout(input.Attributes.Bounds);
            }
            foreach (var output in this.Params.Output)
            {
                output.Attributes.Bounds = this.ChangeOutputLayout(output.Attributes.Bounds);
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
            return GetValue(name, value);
        }
        public int GetValuePub(string name, int value)
        {
            return GetValue(name, value);
        }
        public double GetValuePub(string name, double value)
        {
            return GetValue(name, value);
        }
        public Color GetValuePub(string name, Color value)
        {
            return GetValue(name, value);
        }
        public string GetValuePub(string name, string value)
        {
            return GetValue(name, value);
        }

        #endregion


    }
}