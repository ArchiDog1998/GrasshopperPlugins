/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WPF
{
    public class ParamProxy : Proxy
    {
        public ParamGlassesComponent Owner { get; }
        public Color ShowColor => Owner.GetValuePub(this.TypeFullName, Owner.DefaultColor);
        public string TypeFullName { get; }
        public string TypeName{ get; }

        public ParamProxy(IGH_Param param, ParamGlassesComponent owner)
            :base(param)
        {
            this.Owner = owner;
            this.TypeFullName = param.Type.FullName;
            this.TypeName = param.TypeName;
        }
    }
}
