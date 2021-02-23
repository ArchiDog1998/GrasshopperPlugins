/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using ArchiTed_Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfoGlasses.WinformMenu
{
    public enum ShowcaseToolsSettingsProperty
    {
        IsFixCategoryIcon,
    }
    public class ShowcaseToolsMenu : ToolStripMenuItem, ISettings<ShowcaseToolsSettingsProperty>
    {
        public SaveableSettings<ShowcaseToolsSettingsProperty> Settings { get; } = new SaveableSettings<ShowcaseToolsSettingsProperty>(new Dictionary<ShowcaseToolsSettingsProperty, object>()
        {
            { ShowcaseToolsSettingsProperty.IsFixCategoryIcon, true },
        });

        public ShowcaseToolsMenu()
            : base("ShowcaseTools", Properties.Resources.ShowcaseTools)
        {
            this.DropDown.Items.Add(GetFixCategoryIcon());
        }

        #region FixCategoryIcon
        private SortedList<string, Bitmap> _alreadyhave = null;

        public SortedList<string, Bitmap> AlreadyHave
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
        public Dictionary<string, Bitmap> CanChangeCategoryIcon
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
            ToolStripMenuItem item = WinFormPlus.CreateOneItem("Fix Catogory Icon", "Fix as most category icon as possible.", Grasshopper.Instances.ComponentServer.GetCategoryIcon("Params"));

            MakeRespondItem(item, ShowcaseToolsSettingsProperty.IsFixCategoryIcon, () =>
            {
                foreach (string cateName in CanChangeCategoryIcon.Keys)
                {
                    Grasshopper.Instances.ComponentServer.AddCategoryIcon(cateName, CanChangeCategoryIcon[cateName]);
                }
                GH_ComponentServer.UpdateRibbonUI();
            }, () =>
            {
                foreach (string cateName in CanChangeCategoryIcon.Keys)
                {
                    AlreadyHave.Remove(cateName);
                }
                GH_ComponentServer.UpdateRibbonUI();
            });

            return item;
        }

        private void MakeRespondItem(ToolStripMenuItem item, ShowcaseToolsSettingsProperty name, Action checkAction, Action uncheckAction)
        {
            #region Define event.
            void Item_Click(object sender, EventArgs e)
            {
                Settings.SetProperty(name, Settings.GetProperty(name));
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
