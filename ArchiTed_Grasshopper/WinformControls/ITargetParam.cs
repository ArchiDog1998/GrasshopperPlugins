using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper.WinformControls
{
    public interface ITargetParam<T> : IControlState<T>, INeedWidth, IRespond
    {
        GH_PersistentParam<GH_Goo<T>> Target { get; }
        GH_ParamAccess Access { get; set; }
        //string Suffix { get; }
    }
}
