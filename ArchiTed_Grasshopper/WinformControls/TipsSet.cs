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