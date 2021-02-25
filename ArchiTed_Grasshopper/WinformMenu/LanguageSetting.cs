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

    public delegate void LanguageChangedHandler(Func<string[], string> getTrans);

    public static class LanguageSetting
    {

        public enum LanguageProperties { LanguageIndex, }

        public static SaveableSettings<LanguageProperties> Settings { get; } = new SaveableSettings<LanguageProperties>(
            new SettingsPreset<LanguageProperties>[] 
            { 
                new SettingsPreset<LanguageProperties>(LanguageProperties.LanguageIndex,
                    System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "zh-CN" ? 1 : 0, LanguageChange),
            }, Grasshopper.Instances.Settings);

        #region LanguageSettings
        public static List<string> Languages { get; } = new List<string> { "English", "中文" };

        private static event Action LanguageChange;

        public static string Language => Languages[LanguageIndex];
        private static int LanguageIndex 
        { 
            get => (int)Settings.GetProperty(LanguageProperties.LanguageIndex);
            set => Settings.SetProperty(LanguageProperties.LanguageIndex, value); 
        }

        /// <summary>
        /// Translation from differenc culture to one.
        /// </summary>
        /// <param name="strs">different language.</param>
        /// <returns></returns>
        private static string GetTransLation(params string[] strs)
        {
            if (strs.Length == 0) 
                throw new ArgumentOutOfRangeException(nameof(strs), nameof(strs) + "must more than 1!");
            try
            {
                string result = strs[LanguageIndex];
                if (string.IsNullOrEmpty(result))
                    return strs[0];
                else
                    return result;
            }
            catch (ArgumentOutOfRangeException)
            {
                return strs[0];
            }
        }

        /// <summary>
        /// Add the function to language change event.
        /// </summary>
        /// <param name="languageFunc"></param>
        public static Action AddToLangChangeEvt(this LanguageChangedHandler languageFunc)
        {
            //Change it first.
            languageFunc.Invoke(GetTransLation);

            //Define RealLanguageChange Event.
            void realLangFunc()
            {
                try
                {
                    languageFunc.Invoke(GetTransLation);
                }
                catch (NullReferenceException)
                {
                    LanguageChange -= realLangFunc;
                }
            };
            LanguageChange += realLangFunc;

            //Return the realLangFunc
            return realLangFunc;
        }
        #endregion

        #region LanguageItem
        public static ToolStripMenuItem LanguageMenuItem { get; } = GetLanguageIcon();
        private static ToolStripMenuItem GetLanguageIcon()
        {
            List<string[]> languages = new List<string[]>();
            Languages.ForEach((str) => languages.Add(new string[] { str }));

            return WinFormPlus.CreateComboBoxItemSingle(new string[] { "Language", "语言(Language)" },
                new string[] { "Select one Language.Note: It will Recompute the component with language options!", "请选择一个语言。注意：这将让有语言选项的电池重新计算！" },
                Properties.Resources.LanguageIcon, Settings, LanguageProperties.LanguageIndex, languages.ToArray());
           
        }

        #endregion
    }
}
