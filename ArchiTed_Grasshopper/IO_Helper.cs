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
using System.Windows.Forms;

namespace ArchiTed_Grasshopper
{
    public static class IO_Helper
    {
        #region IO
        public static void WriteString(string path, Func<string> getString)
        {
            WriteString(path, getString.Invoke());
        }


        public static void WriteString(string path, string value)
        {
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(value);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ReadFileInLine(string path, Action<string, int> operationToOneLine, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(path)) return;
            encoding = encoding ?? Encoding.Default;
            try
            {
                StreamReader sr = new StreamReader(path, encoding);

                string[] strs = sr.ReadToEnd().Split('\n');
                for (int i = 0; i < strs.Length; i++)
                {
                    if (string.IsNullOrEmpty(strs[i]))
                        continue;
                    operationToOneLine.Invoke(strs[i], i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Path
        public static string GetNamedPath(IGH_ActiveObject obj, string name, string suffix = ".txt", bool create = false)
        {
            string result = GetNamedLocationWithObject(obj, name, suffix, create);
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
                $"Finding {name} Failed!", $"{name} 查找失败！"
            }));
            return null;
        }

        public static IEnumerable<string> FindObjectInAssemblyFolder(string name, string suffix = ".txt", SearchOption option = SearchOption.AllDirectories)
        {
            return Directory.EnumerateFiles(Grasshopper.Folders.DefaultAssemblyFolder, "*" + name + suffix, option);
        } 

        public static string GetNamedLocationWithObject(IGH_DocumentObject obj, string name, string suffix = ".txt", bool create = false)
        {
            string loc = GetGHObjectLocation(obj, create);
            if(loc == null)
            {
                return null;
            }
            return Path.GetDirectoryName(loc) + "\\" + name + suffix;
        }

        public static string GetGHObjectLocation(IGH_DocumentObject obj, bool create = false)
        {
            string location = obj.GetType().Assembly.Location;
            foreach (var assem in Grasshopper.Instances.ComponentServer.Libraries)
            {
                if(assem.Assembly == obj.GetType().Assembly)
                {
                    location = assem.Location;
                }
            }
            if (create || Directory.Exists(location))
                return location;
            else return null;
        }
        #endregion
    }
}
