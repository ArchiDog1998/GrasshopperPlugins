/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiTed_Grasshopper.WinformControls
{
    public struct TipsSet
    {
        public int RelayTime { get;}
        public int ShowTime { get;}

        public TipsSet(int relay, int show)
        {
            this.RelayTime = relay;
            this.ShowTime = show;
        }
    }
}