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

        #region Message
        public static string GetWriteMessage(string path)
        {
            return LanguagableComponent.GetTransLation(new string[] { "Export successfully! \n Location: ", "导出成功！\n 位置：" }) + path;
        }

        public static string GetReadMessage(int successCount, int failCount)
        {
            string all = successCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported successfully!", "个数据导入成功！" });
            if (failCount > 0)
            {
                all += "\n" + failCount.ToString() + LanguagableComponent.GetTransLation(new string[] { " data imported failed!", "个数据导入失败！" });
            }
            return all;
        }

        public static void OpenDirectionaryDialog(Action<string> selectedAction)
        {
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedAction.Invoke(fileDialog.SelectedPath);
            }
        }

        public static void ImportOpenFileDialog(Action<string> openAction)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = LanguagableComponent.GetTransLation(new string[] { "Select a template", "选择一个模板" });


            openFileDialog.Filter = "*.txt|*.txt";


            openFileDialog.FileName = string.Empty;


            openFileDialog.Multiselect = false;


            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openAction.Invoke(openFileDialog.FileName);
            }
        }

        public static void ExportSaveFileDialog(string defaultName, Action<string> saveAction)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            saveFileDialog.Title = LanguagableComponent.GetTransLation(new string[] { "Set a Paht", "设定一个路径" });
            saveFileDialog.Filter = "*.txt|*.txt";
            saveFileDialog.FileName = "defaultName";
            saveFileDialog.SupportMultiDottedExtensions = false;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveAction.Invoke( saveFileDialog.FileName);
            }
        }
        #endregion
    }
}
