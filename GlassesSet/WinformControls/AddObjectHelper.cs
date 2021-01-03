﻿/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WinformControls
{
    public static class AddObjectHelper
    {
        #region AddObjectIcon
        public static float IconSize => 12;
        public static float IconSpacing => 6;
        public static RectangleF GetIconBound(RectangleF bound, float multy = 1)
        {
            return new RectangleF(bound.X - AddObjectHelper.IconSize - AddObjectHelper.IconSpacing * multy, bound.Y + bound.Height / 2 - AddObjectHelper.IconSize / 2,
                AddObjectHelper.IconSize, AddObjectHelper.IconSize);
        }
        #endregion


        #region Create Object
        public static void CreateNewObject(AddProxyParams[] proxies, IGH_Param target, int index = 0)
        {
            if (proxies.Length < index) return;

            IGH_DocumentObject obj = Grasshopper.Instances.ComponentServer.EmitObject(proxies[index].Guid);
            if (obj == null)
            {
                return;
            }

            CreateNewObject(obj, target, proxies[index].OutIndex);
        }

        public static void CreateNewObject(IGH_DocumentObject obj, IGH_Param target, int outIndex = 0, float leftMove = 100, string init = null)
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


        public static void AddAObjectToCanvas(IGH_DocumentObject obj, PointF pivot, bool update, string init = null)
        {
            var functions = typeof(GH_Canvas).GetRuntimeMethods().Where(m => m.Name.Contains("InstantiateNewObject") && !m.IsPublic).ToArray();
            if (functions.Length > 0)
            {
                functions[0].Invoke(Grasshopper.Instances.ActiveCanvas, new object[] { obj, init, pivot, update });
            }
        }
        #endregion
    }
}
