/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper
{
    public class LanguageSetting
    {
        #region LanguageSettings
        public static List<string> Languages { get; } = new List<string> { "English", "中文" };

        public static event Action LanguageChange;

        public static string Language => Languages[LanguageIndex];

        public static int LanguageIndex
        {
            get => Grasshopper.Instances.Settings.GetValue(nameof(LanguageIndex),
                System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "zh-CN" ? 1 : 0);
            set
            {
                if (LanguageIndex == value) return;
                if (value >= Languages.Count) throw new ArgumentOutOfRangeException(nameof(LanguageIndex));
                Grasshopper.Instances.Settings.SetValue(nameof(LanguageIndex), value);
                LanguageChange.Invoke();

                //Change self
                LanguageMenuItem.ToolTipText = LanguageSetting.GetTransLation(new string[] { "Select one Language.Note: It will Recompute the component with language options!", "请选择一个语言。注意：这将让有语言选项的电池重新计算！" });
                LanguageMenuItem.Text = LanguageSetting.GetTransLation(new string[] { "Language", "语言(Language)" });
            }
        }

        public static string GetTransLation(params string[] strs)
        {
            try
            {
                string result = strs[LanguageIndex];
                if (result != "")
                    return result;
                else
                    return strs[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                return strs[0];
            }
        }

        #endregion

        #region LanguageItem
        public static ToolStripMenuItem LanguageMenuItem { get; } = GetLanguageIcon();
        private static ToolStripMenuItem GetLanguageIcon()
        {
            string languageTip = LanguageSetting.GetTransLation(new string[] { "Select one Language.Note: It will Recompute the component with language options!", "请选择一个语言。注意：这将让有语言选项的电池重新计算！" });

            ToolStripMenuItem languageItem = WinFormPlus.CreateOneItem(LanguageSetting.GetTransLation(new string[] { "Language", "语言(Language)" }),
                languageTip, Properties.Resources.LanguageIcon);
            ResetLanguageItems(languageItem);

            return languageItem;
        }

        private static void ResetLanguageItems(ToolStripMenuItem menu)
        {
            menu.DropDown.Items.Clear();
            for (int i = 0; i < LanguageSetting.Languages.Count; i++)
            {
                AddALanguageItem(menu, i);
            }
        }

        private static void AddALanguageItem(ToolStripMenuItem menu, int index)
        {
            GH_DocumentObject.Menu_AppendItem(menu.DropDown, LanguageSetting.Languages[index],
                click, true, index == LanguageSetting.LanguageIndex);

            void click(object sender, EventArgs e)
            {
                LanguageSetting.LanguageIndex = index;
                ResetLanguageItems(menu);
            }
        }
        #endregion
    }
}
