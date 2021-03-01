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
using InfoGlasses.WinformMenu;
using static InfoGlasses.WinformMenu.InfoGlassesMenuItem;

namespace InfoGlasses.WPF
{
    public class ExceptionProxyPlus: ObjectProxy
    {

        private InfoGlassesMenuItem _owner;

        private bool _isExceptNormal;
        public bool IsExceptNormal
        {
            get { return _isExceptNormal; }
            set
            {
                _isExceptNormal = value;
                if (_isExceptNormal)
                {
                    if (!((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList().Contains(this.Guid))
                    {
                        var relayList = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList();
                        relayList.Add(this.Guid);
                        Settings.SetProperty(InfoGlassesProps.NormalExceptionGuid, relayList);
                    }
                }
                else
                {
                    if (((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList().Contains(this.Guid))
                    {
                        var relayList = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList();
                        relayList.Remove(this.Guid);
                        Settings.SetProperty(InfoGlassesProps.NormalExceptionGuid, relayList);
                    }
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
                    if (!((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList().Contains(this.Guid))
                    {
                        var relayList = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList();
                        relayList.Add(this.Guid);
                        Settings.SetProperty(InfoGlassesProps.PluginExceptionGuid, relayList);
                    }
                }
                else
                {
                    if (((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList().Contains(this.Guid))
                    {
                        var relayList = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList();
                        relayList.Remove(this.Guid);
                        Settings.SetProperty(InfoGlassesProps.PluginExceptionGuid, relayList);
                    }
                }
            }
        }

        public ExceptionProxyPlus(IGH_ObjectProxy proxy, InfoGlassesMenuItem owner)
            :base(proxy)
        {
            this.AddOwner(owner);
        }

        public ExceptionProxyPlus(IGH_DocumentObject obj, InfoGlassesMenuItem owner)
            :base(obj)
        {
            this.AddOwner(owner);
        }

        private void AddOwner(InfoGlassesMenuItem owner)
        {
            this._owner = owner;

            this.IsExceptNormal = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.NormalExceptionGuid)).ToList().Contains(this.Guid);
            this.IsExceptPlugin = ((IEnumerable<Guid>)Settings.GetProperty(InfoGlassesProps.PluginExceptionGuid)).ToList().Contains(this.Guid);
        }
    }
}
