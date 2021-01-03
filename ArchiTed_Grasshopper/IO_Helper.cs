/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiTed_Grasshopper
{
    public static class IO_Helper
    {
        public static string GetNamedPath(IGH_ActiveObject obj, string name, string suffix = ".txt")
        {
            string result = GetNamedLocationWithObject(obj, name, suffix);
            if(result != null)
            {
                return result;
            }
            var strs = FindObjectInAssemblyFolder(name);
            if (strs.Count() > 0)
            {
                return strs.First();
            }
            obj.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, LanguagableComponent.GetTransLation(new string[]
            {
                $"Finding {name} Failed", $"{name} 查找失败！"
            }));
            return null;
        }

        public static IEnumerable<string> FindObjectInAssemblyFolder(string name, string suffix = ".txt", SearchOption option = SearchOption.AllDirectories)
        {
            return Directory.EnumerateFiles(Grasshopper.Folders.DefaultAssemblyFolder, "*" + name + suffix, option);
        } 

        public static string GetNamedLocationWithObject(IGH_DocumentObject obj, string name, string suffix = ".txt")
        {
            string loc = GetGHObjectLocation(obj);
            if(loc == null)
            {
                return null;
            }
            return Path.GetDirectoryName(loc) + "\\" + name + suffix;
        }

        public static string GetGHObjectLocation(IGH_DocumentObject obj)
        {
            string location = obj.GetType().Assembly.Location;
            foreach (var assem in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if(assem.Assembly == obj.GetType().Assembly)
                {
                    location = assem.Location;
                }
            }
            if (Directory.Exists(location))
                return location;
            else return null;
        }

    }
}
