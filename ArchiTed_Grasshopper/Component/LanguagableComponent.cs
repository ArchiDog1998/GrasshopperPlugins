/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using GH_IO.Serialization;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper
{
    [Obsolete]
    public abstract class LanguagableComponent : ControllableComponent
    {
        private bool _showOption;

        public LanguagableComponent(string name, string nickname, string description, string category, string subcategory, bool showOption = true, Type windowsType = null)
            : base(name, nickname, description, category, subcategory, windowsType)
        {
            this._showOption = showOption;
        }


        #region Language

        public static event EventHandler LanguageChanged;

        private static int m_language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "zh-CN" ? 1 : 0;

        protected static int language
        {
            get { return m_language; }
            set { m_language = value; }
        }

        public static string GetTransLation(string[] strs)
        {
            try
            {
                string result = strs[language];
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
        public static readonly List<string> ComponentLanguages = new List<string> { "English", "中文" };

        protected abstract void ResponseToLanguageChanged(object sender, EventArgs e);
        #endregion

        protected abstract override void RegisterInputParams(GH_InputParamManager pManager);

        protected abstract override void RegisterOutputParams(GH_OutputParamManager pManager);

        protected abstract override void SolveInstance(IGH_DataAccess DA);

        #region MenuList
        protected sealed override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            AppendAdditionComponentMenuItems(menu);

            GH_DocumentObject.Menu_AppendSeparator(menu);

            if (_showOption)
            {

                ToolStripMenuItem donateItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Donate To Us!", "向我们赞赏吧！" }),
                    GetTransLation(new string[] { "Select one way to donate to us!", "选择一个渠道赞赏我们！" }), Properties.Resources.DonateIcon);
                AppendDonateMenuItems(donateItem);
                menu.Items.Add(donateItem);

                ToolStripMenuItem helpItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "How to use it?", "如何使用它呢？" }),
                    GetTransLation(new string[] { "Select one Video or URL to know how to use it.", "选择一个视频或者网页了解如何使用它。" }), Properties.Resources.HelpLogo);
                AppendHelpMenuItems(helpItem);
                helpItem.Enabled = helpItem.DropDownItems.Count != 0;
                if (!helpItem.Enabled)
                {
                    helpItem.Text += GetTransLation(new string[] { " (Coming soon...)", "（即将开启……）" });
                }
                menu.Items.Add(helpItem);

                ToolStripMenuItem contactItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Contact us!", "联系我们！" }),
                    GetTransLation(new string[] { "Select one contact way to contact us!", "选择一种联系方式联系我们！" }), Properties.Resources.ContactIcon);
                AppendContactMenuItems(contactItem);
                menu.Items.Add(contactItem);
            }


            string languageTip = GetTransLation(new string[] { "Select one Language.Note: It will Recompute the component with language options!", "请选择一个语言。注意：这将让有语言选项的电池重新计算！" });



            ToolStripMenuItem languageItem = WinFormPlus.CreateOneItem(GetTransLation(new string[] { "Language", "语言(Language)" }), languageTip, Properties.Resources.LanguageIcon);
            AddLanguageItems(languageItem);
            menu.Items.Add(languageItem);


        }

        private void AddLanguageItems(ToolStripMenuItem menu)
        {
            foreach (string name in ComponentLanguages)
            {
                AddALanguageItem(menu, name);
            }
        }

        private void AddALanguageItem(ToolStripMenuItem menu, string name)
        {
            if (ComponentLanguages[language] == name)
                SetValue(name, true);

            GH_DocumentObject.Menu_AppendItem(menu.DropDown, name, click, true, GetValue(name, false));

            void click(object sender, EventArgs e)
            {
                SetValue(name, true);
                RecordUndoEvent(name);
                foreach (string lang in ComponentLanguages)
                {
                    SetValue(lang, false);
                }
                language = ComponentLanguages.IndexOf(name);
                LanguageChanged(this, new EventArgs());
            }
        }

        protected virtual void AppendAdditionComponentMenuItems(ToolStripDropDown menu) { }
        protected virtual void AppendDonateMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddMessageBoxItem(menu, GetTransLation(new string[] { "Alipay", "支付宝" }),
                GetTransLation(new string[] { "Click to donate to us with Alipay!", "点击以使用支付宝赞赏我们！" }), Properties.Resources.AlipayLogo, Properties.Resources.DonateAlipayQRcode);
            WinFormPlus.AddMessageBoxItem(menu, GetTransLation(new string[] { "WechatPay", "微信" }),
                GetTransLation(new string[] { "Click to donate to us with WechatPay!", "点击以使用微信赞赏我们！" }), Properties.Resources.WechatLogo, Properties.Resources.DonateWechatQRcode);

        }
        protected virtual void AppendContactMenuItems(ToolStripMenuItem menu)
        {
            WinFormPlus.AddMessageBoxItem(menu, GetTransLation(new string[] { "InfoGlasses QQ Group", "InfoGlasses QQ交流群" }),
              GetTransLation(new string[] { "Click to contact us in InfoGlasses QQ Group!", "点击以加入InfoGlasses QQ交流群。" }), Properties.Resources.QQLogo, Properties.Resources.InfoGlasses_QQGroup_QRcode);

            WinFormPlus.AddMessageBoxItem(menu, GetTransLation(new string[] { "Bright Zone of Rhino QQ Group", "犀牛之光 QQ交流群" }),
              GetTransLation(new string[] { "Click to contact us in Bright Zone of Rhino QQ Group!", "点击以加入犀牛之光 QQ交流群。" }), Properties.Resources.QQLogo, Properties.Resources.BrightZoneOfRhino_QQGroup_QRcode);

            GH_DocumentObject.Menu_AppendSeparator(menu.DropDown);

            WinFormPlus.AddMessageBoxItem(menu, GetTransLation(new string[] { "Parameterization QQ Group", "参数化交流 QQ群" }),
                GetTransLation(new string[] { "Click to contact us in Parameterization QQ Group!", "点击以加入参数化交流 QQ群。" }), Properties.Resources.QQLogo, Properties.Resources.Parameterization_QQGroup_QRcode);
        }
        protected virtual void AppendHelpMenuItems(ToolStripMenuItem menu) 
        {
            GH_DocumentObject.Menu_AppendSeparator(menu.DropDown);
            WinFormPlus.AddURLItem(menu, GetTransLation(new string[] { "See Source Code", "查看源代码" }), GetTransLation(new string[] { "Click to the GitHub to see source code.", "点击以到GitHub查看源代码。" }),
                WinFormPlus.ItemIconType.GitHub, "https://github.com/ArchiDog1998/GrasshopperPlugins");
        }
        #endregion

        #region ChangeAttribute
        protected static void ChangeComponentAtt(GH_Component Owner, string[] componentAtts, string[][] inputAtts, string[][] outputAtts)
        {
            if (inputAtts.Length != Owner.Params.Input.Count || outputAtts.Length != Owner.Params.Output.Count || componentAtts.Length != 3)
                throw new ArgumentOutOfRangeException();

            ChangeComponentAtt(Owner, componentAtts[0], componentAtts[1], componentAtts[2]);

            for (int i = 0; i < Owner.Params.Input.Count; i++)
            {
                ChangeParamAtt(Owner.Params.Input[i], inputAtts[i][0], inputAtts[i][1], inputAtts[i][2]);
            }

            for (int j = 0; j < Owner.Params.Output.Count; j++)
            {
                ChangeParamAtt(Owner.Params.Output[j], outputAtts[j][0], outputAtts[j][1], outputAtts[j][2]);
            }

        }
        private static void ChangeComponentAtt(GH_Component Owner, string fullName, string nickName, string description)
        {
            Owner.Name = fullName;
            Owner.NickName = nickName;
            Owner.Description = description;
        }
        private static void ChangeParamAtt(IGH_Param Owner, string fullName, string nickName, string description)
        {
            Owner.Name = fullName;
            Owner.NickName = nickName;
            Owner.Description = description;
        }
        #endregion

        #region IO
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("Language", language);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader)
        {
            reader.TryGetInt32("Language", ref m_language);
            return base.Read(reader);
        }
        #endregion
    }
}
