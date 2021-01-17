/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace InfoGlasses.WPF
{
    public class ObjectProxy : ISearchItem
    {
        public string FullName { get; }
        public string Category { get; }
        public string Subcategory { get; }
        public string Description { get; }
        public BitmapImage Icon { get; }
        public GH_Exposure Exposure { get; }
        public string Location { get; }
        public Guid Guid { get; }
        public bool IsPlugin = true;
        public string FindDesc => FullName + Guid;

        public ObjectProxy(IGH_ObjectProxy proxy)
        {
            this.FullName = proxy.Desc.Name;
            this.Category = string.IsNullOrEmpty(proxy.Desc.Category) ? "No Category" : proxy.Desc.Category;
            this.Subcategory = string.IsNullOrEmpty(proxy.Desc.SubCategory) ? "No SubCategory" : proxy.Desc.SubCategory;
            this.Description = proxy.Desc.Description;
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(proxy.Icon);
            this.Guid = proxy.Guid;
            this.Exposure = proxy.Exposure;
            this.Location = proxy.Location;
            foreach (var item in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if (item.Id == proxy.LibraryGuid)
                {
                    this.IsPlugin = !item.IsCoreLibrary;
                    break;
                }
            }
        }

        public ObjectProxy(IGH_DocumentObject obj)
        {
            this.FullName = obj.Name;
            this.Category = obj.HasCategory ? obj.Category : "";
            this.Subcategory = obj.HasSubCategory ? obj.SubCategory : "";
            this.Description = obj.Description;
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(obj.Icon_24x24);
            this.Guid = obj.ComponentGuid;
            this.Exposure = obj.Exposure;
            foreach (var item in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if (item.Assembly == obj.GetType().Assembly)
                {
                    this.IsPlugin = !item.IsCoreLibrary;
                    this.Location = item.Location;
                    break;
                }
            }
        }
    }
}
