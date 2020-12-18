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
        }

        protected abstract override void RegisterInputParams(GH_Component.GH_InputParamManager pManager);

        protected abstract override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager);

        protected abstract override void SolveInstance(IGH_DataAccess DA);

        public abstract void CreateWindow();

        #region Attribute
        public virtual void ChangeParamsLayout() { }

        public virtual void BeforeRender(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel) { }

        public virtual void AfterRender(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel) { }
        #endregion

        #region GetSet
        internal void SetValuePub(string name, bool value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Bool Value Changed");
            SetValue(name, value);
        }
        internal void SetValuePub(string name, int value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Int Value Changed");
            SetValue(name, value);
        }
        internal void SetValuePub(string name, double value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Double Value Changed");
            SetValue(name, value);
        }
        internal void SetValuePub(string name, Color value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("Color Value Changed");
            SetValue(name, value);
        }
        internal void SetValuePub(string name, string value, bool recordUndo = true)
        {
            if (recordUndo)
                RecordUndoEvent("String Value Changed");
            SetValue(name, value);
        }

        internal bool GetValuePub(string name, bool value)
        {
            return GetValue(name, value);
        }
        internal int GetValuePub(string name, int value)
        {
            return GetValue(name, value);
        }
        internal double GetValuePub(string name, double value)
        {
            return GetValue(name, value);
        }
        internal Color GetValuePub(string name, Color value)
        {
            return GetValue(name, value);
        }
        internal string GetValuePub(string name, string value)
        {
            return GetValue(name, value);
        }

        #endregion


    }
}