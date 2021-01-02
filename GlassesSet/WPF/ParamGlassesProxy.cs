/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WPF
{
    public class ParamGlassesProxy:ObjectProxy
    {
        public Func<IGH_DocumentObject> CreateObejct { get; }

        public ParamGlassesProxy(IGH_ObjectProxy proxy)
            :base (proxy)
        {
            this.CreateObejct = proxy.CreateInstance;
        }
    }
}
