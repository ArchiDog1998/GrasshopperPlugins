/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ArchiTed_Grasshopper
{
    public static class WinFormPlus
    {



        #region ValueBox
        public static void AddTextItem(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,
            string Default, string valueName, int width = 100)
        {
            bool flag = true;
            GH_DocumentObject.Menu_AppendTextItem(menu, itemName, BoxItemKeyDown, textchanged, true, width, true);
            void textchanged(GH_MenuTextBox sender, string newText)
            {
                component.SetValuePub(valueName, newText, flag);
                flag = false;
                component.ExpireSolution(true);
            }


        }



        public static void AddNumberBoxItem(ToolStripDropDown menu,LanguagableComponent component,  string itemName, string itemTip, Bitmap itemIcon, bool enable, 
            int Default, int Min, int Max, string valueName, int width = 150)
        {
            if (Max <= Min)
                throw new ArgumentOutOfRangeException("Max less than Min!");

            bool flag = true;
            string English = "Value must be in " + Min.ToString() + " - " + Max.ToString() + ". Default is " + Default.ToString() + ".";
            string Chinese = "输入值域为 " + Min.ToString() + " - " + Max.ToString() + ", 默认为 " + Default.ToString() + "。";
            string tip = itemTip + LanguagableComponent.GetTransLation(new string[] { English, Chinese });
            ToolStripMenuItem item = CreateOneItem(itemName, tip, itemIcon, enable);

            var slider = new GH_DigitScroller
            {
                MinimumValue = Min,
                MaximumValue = Max,
                DecimalPlaces = 0,
                Value = Default,
                Size = new Size(width, 24)
            };
            slider.ValueChanged += Slider_ValueChanged;

            void Slider_ValueChanged(object sender, GH_DigitScrollerEventArgs e)
            {
                int result = (int)e.Value;
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
                component.SetValuePub(valueName, result, flag);
                flag = false;
                component.ExpireSolution(true);
            }

            WinFormPlus.AddLabelItem(item.DropDown, itemName, tip);
            GH_DocumentObject.Menu_AppendCustomItem(item.DropDown, slider);

            GH_DocumentObject.Menu_AppendItem(item.DropDown, LanguagableComponent.GetTransLation(new string[] { "Reset Integer", "重置整数" }), resetClick, Properties.Resources.ResetLogo,
                true, false).ToolTipText = LanguagableComponent.GetTransLation(new string[] { "Click to reset integer.", "点击以重置整数。" });
            void resetClick(object sender, EventArgs e)
            {
                component.SetValuePub(valueName, Default);
                component.ExpireSolution(true);
            }

            menu.Items.Add(item);
        }



        public static void AddNumberBoxItem(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,
            double Default, double Min, double Max, string valueName, int width = 150)
        {
            if (Max <= Min)
                throw new ArgumentOutOfRangeException("Max less than Min!");

            bool flag = true;
            string English = "Value must be in " + Min.ToString() + " - " + Max.ToString() + ". Default is " + Default.ToString() + ".";
            string Chinese = "输入值域为 " + Min.ToString() + " - " + Max.ToString() + ", 默认为 " + Default.ToString() + "。";
            string tip = itemTip + LanguagableComponent.GetTransLation(new string[] { English, Chinese });
            ToolStripMenuItem item = CreateOneItem(itemName, tip, itemIcon, enable);

            var slider = new GH_DigitScroller
            {
                MinimumValue = (decimal)Min,
                MaximumValue = (decimal)Max,
                DecimalPlaces = 3,
                Value = (decimal)Default,
                Size = new Size(width, 24)
            };
            slider.ValueChanged += Slider_ValueChanged;

            void Slider_ValueChanged(object sender, GH_DigitScrollerEventArgs e)
            {
                double result = (double)e.Value;
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
                component.SetValuePub(valueName, result, flag);
                flag = false;
                component.ExpireSolution(true);
            }

            WinFormPlus.AddLabelItem(item.DropDown, itemName, tip);
            GH_DocumentObject.Menu_AppendCustomItem(item.DropDown, slider);

            GH_DocumentObject.Menu_AppendItem(item.DropDown, LanguagableComponent.GetTransLation(new string[] { "Reset Number", "重置数值" }), resetClick, Properties.Resources.ResetLogo,
                true, false).ToolTipText = LanguagableComponent.GetTransLation(new string[] { "Click to reset Number.", "点击以重置数值。" });
            void resetClick(object sender, EventArgs e)
            {
                component.SetValuePub(valueName, Default);
                component.ExpireSolution(true);
            }

            menu.Items.Add(item);
        }

        private static void BoxItemKeyDown(GH_MenuTextBox sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) sender.CloseEntireMenuStructure();
        }
        #endregion

        #region ColorBox
        public struct ItemSet<T>
        {
            public string itemName { get; set; }
            public string itemTip { get; set; }
            public Bitmap itemIcon { get; set; }
            public bool enable { get; set; }
            public T Default { get; set; }
            public string valueName { get; set; }

            public ItemSet(string itemName, string itemTip, Bitmap itemIcon, bool enable,T Default, string valueName)
            {
                this.itemName = itemName;
                this.itemTip = itemTip;
                this.itemIcon = itemIcon;
                this.enable = enable;
                this.Default = Default;
                this.valueName = valueName;
            }
        }

        public static void AddColorBoxItems(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,ItemSet<Color>[] sets)
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

            foreach (var set in sets)
            {
                AddColorBoxItem(item.DropDown, component, set);
            }

            GH_DocumentObject.Menu_AppendItem(item.DropDown, LanguagableComponent.GetTransLation(new string[] { "Reset Color", "重置颜色" }), resetClick, Properties.Resources.ResetLogo,
                           true, false).ToolTipText = LanguagableComponent.GetTransLation(new string[] { "Click to reset colors.", "点击以重置颜色。" });
            void resetClick(object sender, EventArgs e)
            {
                foreach (var set in sets)
                {
                    component.SetValuePub(set.valueName, set.Default);
                }
                component.ExpireSolution(true);
            }
            menu.Items.Add(item);
        }

        public static void AddColorBoxItem(ToolStripDropDown menu, LanguagableComponent component, ItemSet<Color> set)
        {
            AddColorBoxItem(menu, component, set.itemName, set.itemTip, set.itemIcon, set.enable, set.Default, set.valueName);
        }

        public static void AddColorBoxItem(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,
            Color @default, string valueName)
        {
            bool flag = true;
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);
            GH_DocumentObject.Menu_AppendColourPicker(item.DropDown, component.GetValuePub(valueName, @default), textcolorChange);
            void textcolorChange(GH_ColourPicker sender, GH_ColourPickerEventArgs e)
            {
                component.SetValuePub(valueName, e.Colour, flag);
                component.ExpireSolution(true);
                flag = false;
            }
            GH_DocumentObject.Menu_AppendItem(item.DropDown, LanguagableComponent.GetTransLation(new string[] { "Reset Color", "重置颜色" }), resetClick, Properties.Resources.ResetLogo,
               true, false).ToolTipText = LanguagableComponent.GetTransLation(new string[] { "Click to reset colors.", "点击以重置颜色。" });
            void resetClick(object sender, EventArgs e)
            {
                component.SetValuePub(valueName, @default);
                component.ExpireSolution(true);
            }
            menu.Items.Add(item);
        }
        #endregion

        #region ComboBox

        public static void AddComboBoxItemsSingle(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip,string valueName, Bitmap itemIcon, bool enable, ItemSet<bool>[] sets)
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

            for (int i = 0; i < sets.Length; i++)
            {
                AddComboBoxItemSingle(item.DropDown, component, sets[i], valueName, i);
            }

            menu.Items.Add(item);
        }

        public static void AddComboBoxItemSingle(ToolStripDropDown menu, LanguagableComponent component, ItemSet<bool> set, string valueName, int index)
        {

            GH_DocumentObject.Menu_AppendItem(menu, set.itemName, click, set.itemIcon, set.enable, component.GetValuePub(valueName, 0) == index).ToolTipText = set.itemTip;

            void click(object sender, EventArgs e)
            {
                component.SetValuePub(valueName, index);
            }
        }



        public static void AddComboBoxItemsMulty(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,ItemSet<bool>[] sets)
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

            foreach (var set in sets)
            {
                AddComboBoxItemMulty(item.DropDown, component, set);
            }

            menu.Items.Add(item);
        }

        public static void AddComboBoxItemMulty(ToolStripDropDown menu, LanguagableComponent component, ItemSet<bool> set)
        {

            GH_DocumentObject.Menu_AppendItem(menu, set.itemName, click, set.itemIcon, set.enable, component.GetValuePub(set.valueName, set.Default)).ToolTipText = set.itemTip;

            void click(object sender, EventArgs e)
            {
                component.SetValuePub(set.valueName, true);
            }
        }


        #endregion

        /// <summary>
        /// Set a Loop Box item in Winform Menu
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="component"></param>
        /// <param name="itemName">item's name for set infront of the name.</param>
        /// <param name="enable"></param>
        /// <param name="NameList"></param>
        /// <param name="IconList"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="valueName"></param>
        public static void AddLoopBoexItem(ToolStripDropDown menu, LanguagableComponent component, string itemName, bool enable,
            string[] NameList, int defaultIndex, string valueName, Bitmap[] IconList = null)
        {
            if (defaultIndex > NameList.Length - 1)
                throw new ArgumentOutOfRangeException("defaultIndex");
            if (IconList != null && IconList.Length != NameList.Length)
                throw new ArgumentOutOfRangeException("IconList");

            int index = component.GetValuePub(valueName, defaultIndex);
            ToolStripMenuItem item = new ToolStripMenuItem(itemName + LanguagableComponent.GetTransLation(new string[] { ": ", "：" }) + NameList[index]);
            item.ToolTipText = LanguagableComponent.GetTransLation(new string[] { "Click to switch to ", "单击以切换到" }) +
                NameList[(index + 1) % NameList.Length];

            if (IconList != null)
                item.Image = IconList[index];
            item.Enabled = enable;

            item.Click += Item_Click;
            void Item_Click(object sender, EventArgs e)
            {
                component.SetValuePub(valueName, (index + 1) % NameList.Length);
                component.ExpireSolution(true);
            }
            menu.Items.Add(item);
        }

        public static void AddCheckBoxItem(ToolStripDropDown menu, string itemName, string itemTip, Image itemIcon,ControllableComponent owner, string valueName, bool @default, bool enable = true)
        {
            void Item_Click(object sender, EventArgs e)
            {
                owner.SetValuePub(valueName, !owner.GetValuePub(valueName, @default));
                owner.ExpireSolution(true);
            }
            AddClickItem(menu, itemName, itemTip, itemIcon, Item_Click, owner.GetValuePub(valueName, @default), enable);
        }


        public static void AddMessageBoxItem(ToolStripMenuItem menu, string itemName, string itemTip, Bitmap itemIcon, Bitmap boxMap)
        {
            void Item_Click(object sender, EventArgs e)
            {
                ImageMessageBox(boxMap, itemName, itemIcon);
            }

            AddClickItem(menu.DropDown, itemName, itemTip, itemIcon, Item_Click, false);
        }

        public static void AddURLItem(ToolStripMenuItem menu, string itemName, string itemTip, ItemIconType type, string url)
        {
            Image itemIcon;
            switch (type)
            {
                case ItemIconType.Youtube:
                    itemIcon = Properties.Resources.YotubeLogo;
                    break;
                case ItemIconType.Bilibili:
                    itemIcon = Properties.Resources.BilibiliLogo;
                    break;
                case ItemIconType.Wechat:
                    itemIcon = Properties.Resources.WechatLogo;
                    break;
                case ItemIconType.GitHub:
                    itemIcon = Properties.Resources.GithubLogo;
                    break;
                default:
                    itemIcon = Properties.Resources.BilibiliLogo;
                    break;
            }
            AddURLItem(menu, itemName, itemTip, itemIcon, url);
        }

        public static void AddURLItem(ToolStripMenuItem menu, string itemName, string itemTip, Image itemIcon, string url)
        {
            void Item_Click(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start(url);
            }

            AddClickItem(menu.DropDown, itemName, itemTip, itemIcon, Item_Click, false);
        }

        public static ToolStripMenuItem CreateOneItem(string itemName, string itemTip, Image itemIcon)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemName);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            return item;
        }
        public static ToolStripMenuItem CreateOneItem(string itemName, string itemTip, Image itemIcon, bool enable)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemName);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            item.Enabled = enable;
            return item;
        }

        public static void AddLabelItem(ToolStripDropDown menu, string labelText, string labelTip = null, Color? color = null, int divisor = 6, int margin = 5, float? fontSize = null)
        {
            Color realColor = color.HasValue ? color.Value : ColorExtension.OnColor;
            ToolStripLabel item = new ToolStripLabel(labelText);
            if(fontSize == null)
            {
                item.Font = GH_FontServer.StandardBold;
            }
            else
            {
                item.Font = new Font(GH_FontServer.StandardBold.FontFamily, fontSize.Value, FontStyle.Bold);
            }
            
            item.ForeColor = realColor;
            if (!string.IsNullOrEmpty(labelTip))
                item.ToolTipText = labelTip;
            item.Margin = new Padding(menu.Size.Width / divisor, margin, menu.Size.Width / divisor, margin);
            menu.Items.Add(item);
        }

        public static void AddClickItem<T>(ToolStripDropDown menu, string itemName, string itemTip, Image itemIcon,T tag, Action<object, EventArgs, T> click, bool @default = false, bool enable = true)
        {
            void Item_Click(object sender, EventArgs e)
            {
                click.Invoke(sender, e, tag);
            }
            AddClickItem(menu, itemName, itemTip, itemIcon, Item_Click, @default, enable);
        }

        public static void AddClickItem(ToolStripDropDown menu, string itemName, string itemTip, Image itemIcon, EventHandler click, bool @default = false, bool enable = true)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemName);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            item.Click += click;
            item.Checked = @default;
            item.Enabled = enable;
            menu.Items.Add(item);
        }

        private static void ImageMessageBox(Bitmap map, string str, Bitmap icon)
        {
            Form form = new Form();
            IntPtr Hicon = icon.GetHicon();
            form.Icon = System.Drawing.Icon.FromHandle(Hicon);
            form.Text = str;
            Size size = new Size(map.Size.Width + 12, map.Size.Height + 15);
            form.Size = size;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.BackgroundImage = map;
            form.TopMost = true;
            form.Show();
        }



        public enum ItemIconType {Youtube, Bilibili, Wechat, GitHub }
    }
}
