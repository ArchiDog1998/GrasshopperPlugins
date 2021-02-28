/*  Copyright 2021 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.Kernel;

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
                    System.Threading.Thread.CurrentThread.CurrentUICulture.LCID == 2052?1:0, 
                    (value) =>{LanguageChange.Invoke(); }),
            }, Grasshopper.Instances.Settings);

        #region LanguageSettings
        public static List<CultureInfo> Languages { get; } = new List<CultureInfo> { new CultureInfo(1033), new CultureInfo(2052) };

        /// <summary>
        /// Right Language.
        /// </summary>
        public static CultureInfo Language => Languages[(int)Settings.GetProperty(LanguageProperties.LanguageIndex)];


        private static event Action LanguageChange;

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
                string result = strs[(int)Settings.GetProperty(LanguageProperties.LanguageIndex)];
                if (string.IsNullOrEmpty(result))
                    return strs[0];
                else
                    return result;
            }
            catch (IndexOutOfRangeException)
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

        private static ToolStripMenuItem _languageMenuItem;
        public static ToolStripMenuItem LanguageMenuItem 
        {
            get
            {
                if(_languageMenuItem == null)
                {
                    _languageMenuItem = GetLanguageIcon();
                }
                return _languageMenuItem;
            }
        }



        private static ToolStripMenuItem GetLanguageIcon()
        {
            List<string[]> languages = new List<string[]>();
            Languages.ForEach((info) => languages.Add(new string[] { info.NativeName }));

            return WinFormPlus.CreateComboBoxItemSingle(new string[] { "Language", "语言(Language)" },
                new string[] { "Select one Language.Note: It will Recompute the component with language options!", "请选择一个语言。注意：这将让有语言选项的电池重新计算！" },
                Properties.Resources.LanguageIcon, Settings, LanguageProperties.LanguageIndex, languages.ToArray());
        }

        #endregion

        #region ExtraItems
        private static ToolStripMenuItem[] _extraItems;

        public static ToolStripMenuItem[] ExtraItems
        {
            get 
            {
                if (_extraItems == null)
                    _extraItems = GetExtraItems();
                return _extraItems; 
            }
        }


        private static ToolStripMenuItem[] GetExtraItems()
        {
            ToolStripMenuItem donateItem = GetDonateItem();
            ToolStripMenuItem contactItem = GetContactItem();

            return new ToolStripMenuItem[] { donateItem, contactItem, LanguageMenuItem };
        }

        private static ToolStripMenuItem GetDonateItem()
        {
            ToolStripMenuItem donateItem = WinFormPlus.CreateOneItem(new string[] { "Donate To Us!", "向我们赞赏吧！" },
                new string[] { "Select one way to donate to us!", "选择一个渠道赞赏我们！" }, ArchiTed_Grasshopper.Properties.Resources.DonateIcon);
            donateItem.DropDownItems.Add(WinFormPlus.CreateMessageBoxItem(new string[] { "Alipay", "支付宝" },
                new string[] { "Click to donate to us with Alipay!", "点击以使用支付宝赞赏我们！" }, ArchiTed_Grasshopper.Properties.Resources.AlipayLogo, ArchiTed_Grasshopper.Properties.Resources.DonateAlipayQRcode));
            donateItem.DropDownItems.Add(WinFormPlus.CreateMessageBoxItem(new string[] { "WechatPay", "微信" },
                new string[] { "Click to donate to us with WechatPay!", "点击以使用微信赞赏我们！" }, ArchiTed_Grasshopper.Properties.Resources.WechatLogo, ArchiTed_Grasshopper.Properties.Resources.DonateWechatQRcode));
            return donateItem;
        }

        private static ToolStripMenuItem GetContactItem()
        {
            ToolStripMenuItem contactItem = WinFormPlus.CreateOneItem(new string[] { "Contact us!", "联系我们！" },
                new string[] { "Select one contact way to contact us!", "选择一种联系方式联系我们！" }, ArchiTed_Grasshopper.Properties.Resources.ContactIcon);
            contactItem.DropDownItems.Add(WinFormPlus.CreateMessageBoxItem(new string[] { "InfoGlasses QQ Group", "InfoGlasses QQ交流群" },
                new string[] { "Click to contact us in InfoGlasses QQ Group!", "点击以加入InfoGlasses QQ交流群。" }, ArchiTed_Grasshopper.Properties.Resources.QQLogo, ArchiTed_Grasshopper.Properties.Resources.InfoGlasses_QQGroup_QRcode));
            contactItem.DropDownItems.Add(WinFormPlus.CreateMessageBoxItem(new string[] { "Bright Zone of Rhino QQ Group", "犀牛之光 QQ交流群" },
                new string[] { "Click to contact us in Bright Zone of Rhino QQ Group!", "点击以加入犀牛之光 QQ交流群。" }, ArchiTed_Grasshopper.Properties.Resources.QQLogo, ArchiTed_Grasshopper.Properties.Resources.BrightZoneOfRhino_QQGroup_QRcode));
            GH_DocumentObject.Menu_AppendSeparator(contactItem.DropDown);
            contactItem.DropDownItems.Add(WinFormPlus.CreateMessageBoxItem(new string[] { "Parameterization QQ Group", "参数化交流 QQ群" },
                new string[] { "Click to contact us in Parameterization QQ Group!", "点击以加入参数化交流 QQ群。" }, ArchiTed_Grasshopper.Properties.Resources.QQLogo, ArchiTed_Grasshopper.Properties.Resources.Parameterization_QQGroup_QRcode));

            //MessageBox.Show(new Class1().GoogleTranslate("Hello"));
            return contactItem;
        }
        #endregion
    }

}
