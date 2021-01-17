/*  Copyright 2021 Administrator. All Rights Reserved.

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
    public class NullInfo : GH_AssemblyInfo
    {
        public override string AssemblyName => "Other Assembly";
        public override Bitmap AssemblyIcon => Properties.Resources.ExceptionIcon;
        public override Guid Id => Guid.Empty;
    }
}
