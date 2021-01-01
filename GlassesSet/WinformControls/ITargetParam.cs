/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WinformControls
{
    public interface ITargetParam<TGoo, T> : IControlState<T>, INeedWidth, IRespond where TGoo : GH_Goo<T>
    {
        GH_PersistentParam<TGoo> Target { get; }
        GH_ParamAccess Access { get; set; }
        //string Suffix { get; }
    }
}
