/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

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
    public class ExceptionProxy: ObjectProxy
    {

        private InfoGlassesComponent _owner;

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

        public ExceptionProxy(IGH_ObjectProxy proxy, InfoGlassesComponent owner)
            :base(proxy)
        {
            this.AddOwner(owner);
        }

        public ExceptionProxy(IGH_DocumentObject obj, InfoGlassesComponent owner)
            :base(obj)
        {
            this.AddOwner(owner);
        }

        private void AddOwner(InfoGlassesComponent owner)
        {
            this._owner = owner;

            this.IsExceptNormal = _owner.normalExceptionGuid.Contains(this.Guid);
            this.IsExceptPlugin = _owner.pluginExceptionGuid.Contains(this.Guid);
        }
    }
}
