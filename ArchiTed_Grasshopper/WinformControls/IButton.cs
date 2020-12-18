using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface IButton : IRespond
    {
        ButtonRenderSet RenderSet { get; }
    }
}
