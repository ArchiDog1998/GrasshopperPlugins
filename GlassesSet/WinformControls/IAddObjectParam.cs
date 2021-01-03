/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformControls
{
    public interface IAddObjectParam<TGoo>: IParamControlBase<TGoo> where TGoo : class, IGH_Goo
    {
        AddProxyParams[] MyProxies { get; }
        RectangleF IconButtonBound { get; }

        new ParamGlassesComponent Owner { get; }
    }
}
