/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Orthoptera.Language
{
    public class GH_ObjectDescriptionTable
    {
        private CultureInfo Info;
        private static string Path => Grasshopper.Folders.AppDataFolder + "Language\\";

        public List<GH_ObjectDescriptionList> DescSet { get; } = new List<GH_ObjectDescriptionList>();

        public GH_ObjectDescriptionTable(CultureInfo info)
        {
            //Directory.g
        }

        #region WriteXml to translate.
        public static void WriteXml(CultureInfo info)
        {
            string pathWithCulture = Path + info.Name + '\\';
            Directory.CreateDirectory(pathWithCulture);

            List<IGH_ObjectProxy> compiledProxies = new List<IGH_ObjectProxy>();
            List<IGH_ObjectProxy> userobjectProxies = new List<IGH_ObjectProxy>();
            foreach (var proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
            {
                if (proxy.Kind == GH_ObjectType.CompiledObject)
                {
                    compiledProxies.Add(proxy);
                }
                else if (proxy.Kind == GH_ObjectType.UserObject)
                {
                    userobjectProxies.Add(proxy);
                }
            }

            List<GH_ObjectDescriptionList> descSet = new List<GH_ObjectDescriptionList>();

            //Group the CompiledProxies with their libraries.
            foreach (var group in compiledProxies.GroupBy((proxy) => {
                foreach (GH_AssemblyInfo lib in Grasshopper.Instances.ComponentServer.Libraries)
                {
                    if (lib.Assembly == proxy.Type.Assembly)
                    {
                        return lib.Name;
                    }
                }
                return "NullLibrary";
            }))
            {
                new GH_ObjectDescriptionList(group.ToList(), group.Key).WriteXml(pathWithCulture + info.Name + "_");
            }

            //Group the UserProxies with their categories.
            foreach (var group in userobjectProxies.GroupBy((proxy) => proxy.Desc.Category))
            {
                new GH_ObjectDescriptionList(group.ToList(), group.Key).WriteXml(pathWithCulture + info.Name + "_");
            }
        }
        #endregion
    }
}
