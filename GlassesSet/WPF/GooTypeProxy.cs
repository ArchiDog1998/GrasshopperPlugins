/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper.WPF;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGlasses.WPF
{
    public class GooTypeProxy : ISearchItem
    {
        public ParamGlassesComponent Owner { get; }
        public Color ShowColor => Owner.GetColor(this.TypeFullName);

        private Func<IGH_DocumentObject> _createObject;
        /// <summary>
        /// Null for no defination.
        /// </summary>
        public Func<IGH_DocumentObject> CreateObject { get 
            {
                if(_createObject == null)
                {
                    FindCreateFunction();
                }
                return _createObject;
            } }
        public string TypeFullName { get; }
        public string TypeName{ get; }
        public bool IsPlugin = true;


        public string FindDesc => TypeName + TypeFullName;

        public GooTypeProxy(Type type, ParamGlassesComponent owner)
        {
            this.Owner = owner;
            this.TypeFullName = type.FullName;
            this.TypeName = type.Name;
            foreach (var item in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if (item.Assembly == type.Assembly)
                {
                    this.IsPlugin = !item.IsCoreLibrary;
                    break;
                }
            }
        }

        private void FindCreateFunction()
        {
            foreach (var item in Owner.AllProxy)
            {
                if(item.Guid == Owner.GetCreateProxyGuid(this.TypeFullName).Value)
                {
                    _createObject = item.CreateObejct;
                }
            }
        }
    }
}
