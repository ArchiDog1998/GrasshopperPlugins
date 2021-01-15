/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WinformControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace InfoGlasses
{
    public struct AddProxyParams
    {
        public Guid Guid { get; }
        public ushort Index { get; }
        public Iconable Icon { get;}
        public BitmapImage ShowIcon => CanvasRenderEngine.BitmapToBitmapImage(this.Icon.GetIcon(true, true));
        public string Name { get; }
        public string Category { get;}
        public string Subcategory { get; }
        public string Exposure { get; }
        public AddProxyParams(Guid guid, ushort Index, int picturesize = 24)
        {
            this.Guid = guid;
            this.Index = Index;
            this.Icon = null;
            this.Name = "";
            this.Category = "";
            this.Subcategory = "";
            this.Exposure = "";

            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if(proxy.Guid == this.Guid)
                {
                    this.Icon = new Iconable(new Bitmap(proxy.Icon, new Size(picturesize, picturesize)));
                    this.Name = proxy.Desc.Name;
                    this.Category = proxy.Desc.HasCategory ? proxy.Desc.Category : "";
                    this.Subcategory = proxy.Desc.HasSubCategory ? proxy.Desc.SubCategory : "";
                    this.Exposure = proxy.Exposure.ToString();
                    break;
                }
            }
        }
    }
}
