﻿/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

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
    public class ParamProxy: Proxy
    {
        public Color ShowColor { get; set; }
        public string TypeFullName { get; }

        public ParamProxy(IGH_Param param, Color color)
            :base(param)
        {
            this.ShowColor = color;
            this.TypeFullName = param.Type.FullName;
        }
    }
}