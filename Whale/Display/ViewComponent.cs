/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Whale.Display
{
    public abstract class ViewComponent : GH_Component
    {
        public ViewComponent(string name, string nickname, string description, string category, string subcategory)
            : base(name, nickname, description, category, subcategory)
        {

        }

        private static Type _viewType = null;

        private static Type FindViewParam()
        {
            //CurveComponents.Make2DViewParameter
            //7069208c-c471-4b82-bae6-e938f16dacb0

            foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (proxy.Guid != new Guid("3fc08088-d75d-43bc-83cc-7a654f156cb7")) continue;
                return ((GH_Component)proxy.CreateInstance()).Params.Output[0].GetType();
            }
            return null;
        }

        public static IGH_Param CreateViewParam()
        {
            _viewType = _viewType ?? FindViewParam();
            if (_viewType == null) throw new ArgumentNullException();

            return (IGH_Param)Activator.CreateInstance(_viewType, "View", "V", "View", "Display", "Dimensions", GH_ParamAccess.item);
        }
    }
}