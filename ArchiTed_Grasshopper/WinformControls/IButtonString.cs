using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface IButtonString : IButton
    {
        string Name { get; }

        Color RightColor { get; }
    }

}
