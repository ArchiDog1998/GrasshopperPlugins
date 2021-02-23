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
    public enum ShowcaseToolsProperties
    {
        IsFixCategoryIcon,
        FixCategoryFolder,
    }
    public class ShowcaseToolsMenu : ToolStripMenuItem, ISettings<ShowcaseToolsProperties>, ILanguageChangeable
    {
        public SaveableSettings<ShowcaseToolsProperties> Settings { get; } = new SaveableSettings<ShowcaseToolsProperties>(new Dictionary<ShowcaseToolsProperties, object>()
        {
            { ShowcaseToolsProperties.IsFixCategoryIcon, true },
            { ShowcaseToolsProperties.FixCategoryFolder, Grasshopper.Folders.UserObjectFolders[0] },
        });

        public void ResponseToLanguageChanged()
        {
            this.Text = LanguageSetting.GetTransLation("ShowcaseTools", "展示工具");

            FixCategoryMenuItem.Text = LanguageSetting.GetTransLation("Fix Catogory Icon", "修复类别图标");
            FixCategoryMenuItem.ToolTipText = LanguageSetting.GetTransLation("Fix as most category icon as possible.", "修复尽可能多的类别图标。");
            
            CateIconFoderNameChange();
        }

        public ToolStripMenuItem FixCategoryMenuItem { get; }
        public ToolStripMenuItem FixCategoryIconFolder { get; }
        public ShowcaseToolsMenu()
            : base("", Properties.Resources.ShowcaseTools)
        {
            //Add to Language Changes.
            LanguageSetting.AddLangObj(this);

            //this.DropDown.MaximumSize = new Size(150, int.MaxValue);

            //Create items.
            this.FixCategoryIconFolder = GetCategoryIconFolder();
            this.FixCategoryMenuItem = GetFixCategoryIcon();

            //Add items.
            this.DropDown.Items.Add(FixCategoryMenuItem);
            this.DropDown.Items.Add(FixCategoryIconFolder);

            GH_DocumentObject.Menu_AppendSeparator(this.DropDown);

            this.DropDown.Items.Add(LanguageSetting.LanguageMenuItem);

            //Change Language.
            ResponseToLanguageChanged();
        }

        #region FixCategoryIcon
        private SortedList<string, Bitmap> _alreadyhave = null;

        private SortedList<string, Bitmap> AlreadyHave
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


        private Dictionary<string, Bitmap> _canChangeCategoryIcon = null;

        /// <summary>
        /// hold the categoryicon that can be changed!
        /// </summary>
        private Dictionary<string, Bitmap> CanChangeCategoryIcon
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
                        string folderPath = (string)Settings.GetProperty(ShowcaseToolsProperties.FixCategoryFolder);
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

        private ToolStripMenuItem GetFixCategoryIcon()
        {
            ToolStripMenuItem item = WinFormPlus.CreateOneItem("","", Grasshopper.Instances.ComponentServer.GetCategoryIcon("Params"));

            MakeRespondItem(item, ShowcaseToolsProperties.IsFixCategoryIcon, () =>
            {
                foreach (string cateName in CanChangeCategoryIcon.Keys)
                {
                    Grasshopper.Instances.ComponentServer.AddCategoryIcon(cateName, CanChangeCategoryIcon[cateName]);
                }
                GH_ComponentServer.UpdateRibbonUI();
                FixCategoryIconFolder.Enabled = true;
            }, () =>
            {
                foreach (string cateName in CanChangeCategoryIcon.Keys)
                {
                    AlreadyHave.Remove(cateName);
                }
                GH_ComponentServer.UpdateRibbonUI();
                FixCategoryIconFolder.Enabled = false;
            });
            return item;
        }

        private ToolStripMenuItem GetCategoryIconFolder()
        {
            string latestFolder = (string)Settings.GetProperty(ShowcaseToolsProperties.FixCategoryFolder);
            ToolStripMenuItem item = new ToolStripMenuItem(latestFolder);
            item.Enabled = (bool)Settings.GetProperty(ShowcaseToolsProperties.IsFixCategoryIcon);
            item.Click += Item_Click;

            void Item_Click(object sender, EventArgs e)
            {
                IO_Helper.OpenDirectionaryDialog((folder) => Settings.SetProperty(ShowcaseToolsProperties.FixCategoryFolder, folder));
                CateIconFoderNameChange();
            }

            return item;
        }

        private void CateIconFoderNameChange()
        {
            FixCategoryIconFolder.Text = LanguageSetting.GetTransLation("Change Icons' Folder", "修改图标所在文件");
            FixCategoryIconFolder.ToolTipText = LanguageSetting.GetTransLation("Click to change the folder.", "单击以修改路径") + 
                "\n \n" + (string)Settings.GetProperty(ShowcaseToolsProperties.FixCategoryFolder);
        }

        private void MakeRespondItem(ToolStripMenuItem item, ShowcaseToolsProperties name, Action checkAction, Action uncheckAction)
        {
            #region Define event.
            void Item_Click(object sender, EventArgs e)
            {
                Settings.SetProperty(name, !(bool)Settings.GetProperty(name));
                item.Checked = (bool)Settings.GetProperty(name);
            }

            void Item_CheckedChanged(object sender, EventArgs e)
            {
                switch (item.Checked)
                {
                    case true:
                        checkAction.Invoke();
                        break;
                    case false:
                        uncheckAction.Invoke();
                        break;
                }
            }
            #endregion

            item.Checked = (bool)Settings.GetProperty(name);
            item.Click += Item_Click;
            item.CheckedChanged += Item_CheckedChanged;
            Item_CheckedChanged(null, new EventArgs());
        }


        #endregion


    }
}
