/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WinformControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses
{
    public struct AddProxyParams
    {
        public Guid Guid { get; }
        public int Index { get; }
        public Iconable Icon { get;}
        public string Name { get; }
        public AddProxyParams(Guid guid, int Index, int picturesize = 24)
        {
            this.Guid = guid;
            this.Index = Index;
            this.Icon = null;
            this.Name = "";

            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if(proxy.Guid == this.Guid)
                {
                    this.Icon = new Iconable(new Bitmap(proxy.Icon, new Size(picturesize, picturesize)));
                    this.Name = proxy.Desc.Name;
                    break;
                }
            }
        }
    }
}
