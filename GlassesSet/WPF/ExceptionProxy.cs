using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Grasshopper.Kernel;
using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WPF;

namespace InfoGlasses.WPF
{
    public class ExceptionProxy: ISearchItem
    {
        public string FullName { get; }
        public string Category { get; }
        public string Subcategory { get; }
        public string Description { get; }
        public BitmapImage Icon { get; }
        public GH_Exposure Exposure { get; }
        public string Location { get; set; }
        public Guid Guid { get; }

        private InfoGlassesComponent _owner;
        public bool IsPlugin = true;

        private bool _isExceptNormal;
        public bool IsExceptNormal
        {
            get { return _isExceptNormal; }
            set
            {
                _isExceptNormal = value;
                if (_isExceptNormal)
                {
                    if (!_owner.normalExceptionGuid.Contains(this.Guid))
                        _owner.normalExceptionGuid.Add(this.Guid);
                }
                else
                {


                    if (_owner.normalExceptionGuid.Contains(this.Guid))
                        _owner.normalExceptionGuid.Remove(this.Guid);
                }
            }
        }

        private bool _isExceptPlugin;

        public bool IsExceptPlugin
        {
            get { return _isExceptPlugin; }
            set
            {
                _isExceptPlugin = value;
                if (_isExceptPlugin)
                {

                    if (!_owner.pluginExceptionGuid.Contains(this.Guid))
                        _owner.pluginExceptionGuid.Add(this.Guid);
                }
                else
                {

                    if (_owner.pluginExceptionGuid.Contains(this.Guid))
                        _owner.pluginExceptionGuid.Remove(this.Guid);
                }
            }
        }

        public string FindDesc => FullName;

        public ExceptionProxy(IGH_ObjectProxy proxy, InfoGlassesComponent owner)
        {
            this.FullName = proxy.Desc.Name;
            this.Category = proxy.Desc.HasCategory ? proxy.Desc.Category : "";
            this.Subcategory = proxy.Desc.HasSubCategory ? proxy.Desc.SubCategory : "";
            this.Description = proxy.Desc.Description;
            this.Icon = CanvasRenderEngine.BitmapToBitmapImage(proxy.Icon);
            this.Guid = proxy.Guid;
            this.Exposure = proxy.Exposure;
            this.Location = proxy.Location;

            foreach (var item in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if(item.Id == proxy.LibraryGuid)
                {
                    this.IsPlugin = !item.IsCoreLibrary;
                    break;
                }
            }
            this._owner = owner;

            this.IsExceptNormal = _owner.normalExceptionGuid.Contains(this.Guid);
            this.IsExceptPlugin = _owner.pluginExceptionGuid.Contains(this.Guid);
        }
    }
}
