/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformMenu
{
    internal class FixCategoryMenuItem: ToolStripMenuItem
    {
        internal enum FixCategoryProps
        {
            IsFixCategoryIcon,
            FixCategoryFolder,
        }
        internal static SaveableSettings<FixCategoryProps> Settings { get; } = new SaveableSettings<FixCategoryProps>(new SettingsPreset<FixCategoryProps>[]
        {
            new SettingsPreset<FixCategoryProps>(FixCategoryProps.IsFixCategoryIcon, true, (value)=>
            {
                switch (value)
                    {
                        case true:
                            foreach (string cateName in CanChangeCategoryIcon.Keys)
                            {
                                Grasshopper.Instances.ComponentServer.AddCategoryIcon(cateName, CanChangeCategoryIcon[cateName]);
                            }
                            break;
                        case false:
                            foreach (string cateName in CanChangeCategoryIcon.Keys)
                            {
                                AlreadyHave.Remove(cateName);
                            }
                            break;
                    }
                GH_ComponentServer.UpdateRibbonUI();
            }),
            new SettingsPreset<FixCategoryProps>(FixCategoryProps.FixCategoryFolder, Grasshopper.Folders.UserObjectFolders[0]),
        }, Grasshopper.Instances.Settings);

        internal FixCategoryMenuItem():base(Grasshopper.Instances.ComponentServer.GetCategoryIcon("Params"))
        {
            this.SetItemLangChange(new string[] { "Fix Catogory Icon", "修复类别图标" }, new string[] { "Fix as most category icon as possible.", "修复尽可能多的类别图标。" });
            this.BindingAndCheckProperty(Settings, FixCategoryProps.IsFixCategoryIcon);

            ToolStripMenuItem folderItem = WinFormPlus.CreateClickItem(new string[] { "Change Icons' Folder", "修改图标所在文件" },
                new string[] { "Click to change the folder.\n", "单击以修改路径。\n" }, null, (x, y) =>
                {
                    //Find Dictionary Dialog.
                    IO_Helper.OpenDirectionaryDialog((folder) => Settings.SetProperty(FixCategoryProps.FixCategoryFolder, folder));

                    //Change ToolTips.
                    ((ToolStripMenuItem)x).ToolTipText = ((ToolStripMenuItem)x).ToolTipText.Split('\n')[0] + "\n \n"
                        + (string)Settings.GetProperty(FixCategoryProps.FixCategoryFolder);

                }, enable: (bool)Settings.GetProperty(FixCategoryProps.IsFixCategoryIcon));
            this.DropDown.Items.Add(folderItem);
        }
        #region FixCategoryIcon

        private static SortedList<string, Bitmap> _alreadyhave = null;

        private static SortedList<string, Bitmap> AlreadyHave
        {
            get
            {
                if (_alreadyhave == null)
                {
                    //find the m_guids private field.
                    IEnumerable<FieldInfo> infos = typeof(GH_ComponentServer).GetRuntimeFields().Where((info) => info.Name.Contains("_categoryIcons"));

                    // check whether to find the info.
                    if (infos.Count() == 0) throw new Exception(nameof(GH_ComponentServer) + "'s _categoryIcons can not be found!");

                    //Get the already dictionary
                    _alreadyhave = infos.ElementAt(0).GetValue(Grasshopper.Instances.ComponentServer) as SortedList<string, Bitmap>;
                }
                return _alreadyhave;
            }
        }


        private static Dictionary<string, Bitmap> _canChangeCategoryIcon = null;

        /// <summary>
        /// hold the categoryicon that can be changed!
        /// </summary>
        private static Dictionary<string, Bitmap> CanChangeCategoryIcon
        {
            get
            {
                if (_canChangeCategoryIcon == null)
                {
                    _canChangeCategoryIcon = new Dictionary<string, Bitmap>();

                    foreach (IGH_ObjectProxy proxy in Grasshopper.Instances.ComponentServer.ObjectProxies)
                    {
                        //skip two condition.
                        if (AlreadyHave.ContainsKey(proxy.Desc.Category)) continue;
                        if (_canChangeCategoryIcon.ContainsKey(proxy.Desc.Category)) continue;
                        if (proxy.Kind != GH_ObjectType.CompiledObject) continue;

                        //Other function
                        string folderPath = (string)Settings.GetProperty(FixCategoryProps.FixCategoryFolder);
                        if (Directory.Exists(folderPath))
                        {
                            bool isSucceed = false;
                            foreach (string itemFilePath in Directory.GetFiles(folderPath))
                            {
                                if (!itemFilePath.Contains(proxy.Desc.Category)) continue;
                                try
                                {
                                    Bitmap bitmap = new Bitmap(itemFilePath);
                                    _canChangeCategoryIcon.Add(proxy.Desc.Category, bitmap);
                                    isSucceed = true;
                                    break;
                                }
                                catch { continue; }
                            }
                            if (isSucceed) continue;
                        }

                        //Find the library.
                        GH_AssemblyInfo info = Grasshopper.Instances.ComponentServer.FindAssembly(proxy.LibraryGuid);
                        if (info == null) continue;
                        if (info.Icon == null) continue;

                        //add it to dictionary.
                        _canChangeCategoryIcon.Add(proxy.Desc.Category, info.Icon);
                    }
                }
                return _canChangeCategoryIcon;
            }
        }



        #endregion
    }
}
