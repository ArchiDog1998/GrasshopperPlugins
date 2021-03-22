/*  Copyright 2020 RadiRhino-秋水. All Rights Reserved.

    Distributed under MIT license.

    See file LICENSE for detail or copy at http://opensource.org/licenses/MIT
*/

using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.Kernel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArchiTed_Grasshopper
{
    public static class WinFormPlus
    {
        #region ValueBox
        [Obsolete]
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
        [Obsolete]
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

        public static ToolStripMenuItem CreateNumberBox<T>(string[] itemName, string[] itemTip, Bitmap itemIcon,
            SaveableSettings<T> server, T valueName, double Max, double Min)where T : Enum
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon);

            dynamic @default = server.GetProperty(valueName);
            bool isInt = @default is int;
            int decimalPlace = 3;
            if (isInt)
                decimalPlace = 0;

            decimal forDefault = (decimal)@default;

            GH_DigitScroller slider = new GH_DigitScroller
            {
                MinimumValue = (decimal)Min,
                MaximumValue = (decimal)Max,
                DecimalPlaces = decimalPlace,
                Value = forDefault,
                Size = new Size(150, 24),
            };
            slider.ValueChanged += Slider_ValueChanged;

            void Slider_ValueChanged(object sender, GH_DigitScrollerEventArgs e)
            {
                double result = (double)e.Value;
                result = result >= Min ? result : Min;
                result = result <= Max ? result : Max;
                if (isInt)
                    server.SetProperty(valueName, (int)result);
                else
                    server.SetProperty(valueName, result);
            }

            GH_DocumentObject.Menu_AppendCustomItem(item.DropDown, slider);

            //Add a Reset Item.
            item.DropDownItems.Add(CreateClickItem(new string[] { "Reset Value", "重置数值" }, new string[] { "Click to reset value.", "点击以重置数值。" },
                 Properties.Resources.ResetLogo, (x, y) =>
                 {
                     dynamic value = server.ResetProperty(valueName);
                     slider.Value = (decimal)value;
                 }));

            server.DefaultValueChanged(valueName);
            return item;
        }

        [Obsolete]
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
        public struct ItemSet<T> where T : Enum
        {
            public string[] Text { get; }
            public string[] Tip { get; }
            public Bitmap Icon { get; }
            public bool Enable { get; }
            public T ValueName { get; }

            public ItemSet(string[] itemText, string[] itemTip, Bitmap itemIcon, bool enable, T valueName)
            {
                this.Text = itemText;
                this.Tip = itemTip;
                this.Icon = itemIcon;
                this.Enable = enable;
                this.ValueName = valueName;
            }
        }
        #region Rubbish
        [Obsolete]
        public struct ItemSet_Obsolete<T>
        {
            public string itemText { get; set; }
            public string itemTip { get; set; }
            public Bitmap itemIcon { get; set; }
            public bool enable { get; set; }
            public T Default { get; set; }
            public string valueName { get; set; }

            public ItemSet_Obsolete(string itemText, string itemTip, Bitmap itemIcon, bool enable,T Default, string valueName)
            {
                this.itemText = itemText;
                this.itemTip = itemTip;
                this.itemIcon = itemIcon;
                this.enable = enable;
                this.Default = Default;
                this.valueName = valueName;
            }
        }
        [Obsolete]
        public static void AddColorBoxItems_Obsolete(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,ItemSet_Obsolete<Color>[] sets)
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

            foreach (var set in sets)
            {
                AddColorBoxItem_Obsolete(item.DropDown, component, set);
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
        [Obsolete]
        public static void AddColorBoxItem_Obsolete(ToolStripDropDown menu, LanguagableComponent component, ItemSet_Obsolete<Color> set)
        {
            AddColorBoxItem_Obsolete(menu, component, set.itemText, set.itemTip, set.itemIcon, set.enable, set.Default, set.valueName);
        }
        [Obsolete]
        public static void AddColorBoxItem_Obsolete(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,
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
        public static ToolStripMenuItem CreateColorBoxItems<T>(SaveableSettings<T> setting, string[] itemName, string[] itemTip, Bitmap itemIcon, bool enable, ItemSet<T>[] sets) where T : Enum
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, @checked: false, enable: enable);

            //Add all ColourPickers and save actions.
            for (int i = 0; i < sets.Length; i++)
            {
                item.DropDownItems.Add(CreateColorBoxItem(setting, sets[i]));
            }

            item.DropDownItems.Add(CreateClickItem(new string[] { "Reset Color", "重置颜色" }, new string[] { "Click to reset colors.", "点击以重置颜色。" },
                Properties.Resources.ResetLogo, (x, y) =>
                {
                    //Reset every Colours.
                    sets.ToList().ForEach((set) => setting.ResetProperty(set.ValueName));
                }));

            return item;
        }

        public static ToolStripMenuItem CreateColorBoxItem<T>(SaveableSettings<T> setting, ItemSet<T> set) where T : Enum
        {
            return CreateColorBoxItem(setting, set.Text, set.Tip, set.Icon, set.Enable, set.ValueName);
        }

        public static ToolStripMenuItem CreateColorBoxItem<T>(SaveableSettings<T> setting, string[] itemName, string[] itemTip, Bitmap itemIcon, bool enable, T valueName) where T : Enum
        {
            //Create a new item.
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, @checked: false, enable: enable);

            //Add a ColourPicker
            GH_ColourPicker colourPicker = GH_DocumentObject.Menu_AppendColourPicker(item.DropDown, (Color)setting.GetProperty(valueName), 
                (x, e) => { setting.SetProperty(valueName, e.Colour); });

            //Add a Reset Item.
            item.DropDownItems.Add(CreateClickItem(new string[] { "Reset Color", "重置颜色" }, new string[] { "Click to reset colors.", "点击以重置颜色。" },
                 Properties.Resources.ResetLogo, (x, y) => 
                 { 
                     colourPicker.Colour =(Color)setting.ResetProperty(valueName);
                 }));

            //Return and do valueChanged.
            setting.DefaultValueChanged(valueName);
            return item;
        }

        #endregion

            //#region ComboBox

        public static ToolStripMenuItem CreateComboBoxItemSingle<T>(string[] itemName, string[] itemTip, Image icon,
            SaveableSettings<T> server, T valueName, string[][] nameList, string[][] tipList = null,
            Image[] IconList = null, bool enable = true) where T : Enum
        {
            //Reset Array.
            if (IconList == null) IconList = new Image[nameList.Length];
            if (tipList == null)
            {
                List<string[]> tiplist = new List<string[]>();
                nameList.ToList().ForEach((x) => tiplist.Add(new string[1]));
                tipList = tiplist.ToArray();
            }

            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, icon, false, enable);
            
            //Add each sub items.
            for (int i = 0; i < nameList.Length; i++)
            {
                ToolStripMenuItem temp = CreateClickItem(nameList[i], tipList[i], IconList[i], (x, y)=>
                {
                    //Uncheck previous checked item.
                    try
                    {
                        ((ToolStripMenuItem)item.DropDownItems[(int)server.GetProperty(valueName)]).Checked = false;
                    }
                    catch(IndexOutOfRangeException)
                    {
                        throw new Exception(valueName.ToString() + " Must be a index integer!");
                    }

                    //Set Value.
                    server.SetProperty(valueName, ((ToolStripMenuItem)x).Tag);

                    //set this one to checked.
                    ((ToolStripMenuItem)x).Checked = true;

                    //Set default value.
                }, (int)server.GetProperty(valueName) == i);
                temp.Tag = i;
                item.DropDown.Items.Add(temp);
            }

            server.DefaultValueChanged(valueName);
            return item;
        }
        //[Obsolete]
        //public static void AddComboBoxItemsSingle(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip,string valueName, Bitmap itemIcon, bool enable, ItemSet_Obsolete<bool>[] sets)
        //{
        //    ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

        //    for (int i = 0; i < sets.Length; i++)
        //    {
        //        AddComboBoxItemSingle(item.DropDown, component, sets[i], valueName, i);
        //    }

        //    menu.Items.Add(item);
        //}
        //[Obsolete]
        //public static void AddComboBoxItemSingle(ToolStripDropDown menu, LanguagableComponent component, ItemSet_Obsolete<bool> set, string valueName, int index)
        //{

        //    GH_DocumentObject.Menu_AppendItem(menu, set.itemText, click, set.itemIcon, set.enable, component.GetValuePub(valueName, 0) == index).ToolTipText = set.itemTip;

        //    void click(object sender, EventArgs e)
        //    {
        //        component.SetValuePub(valueName, index);
        //    }
        //}


        //[Obsolete]
        //public static void AddComboBoxItemsMulty(ToolStripDropDown menu, LanguagableComponent component, string itemName, string itemTip, Bitmap itemIcon, bool enable,ItemSet_Obsolete<bool>[] sets)
        //{
        //    ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, enable);

        //    foreach (var set in sets)
        //    {
        //        AddComboBoxItemMulty(item.DropDown, component, set);
        //    }

        //    menu.Items.Add(item);
        //}

        //[Obsolete]
        //public static void AddComboBoxItemMulty(ToolStripDropDown menu, LanguagableComponent component, ItemSet_Obsolete<bool> set)
        //{

        //    GH_DocumentObject.Menu_AppendItem(menu, set.itemText, click, set.itemIcon, set.enable, component.GetValuePub(set.valueName, set.Default)).ToolTipText = set.itemTip;

        //    void click(object sender, EventArgs e)
        //    {
        //        component.SetValuePub(set.valueName, true);
        //    }
        //}


        //#endregion

        #region ClickBox
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
        [Obsolete]
        public static void AddLoopBoxItem(ToolStripDropDown menu, LanguagableComponent component, string itemName, bool enable,
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

        public static ToolStripMenuItem CreateLoopBoxItem<T>(string[] itemName, string[] itemTip,
            SaveableSettings<T> server, T valueName, string[][] nameList,
            Image[] IconList = null, bool enable = true) where T:Enum
        {
            //Check input.
            if ((int)server.GetProperty(valueName) > nameList.Length - 1)
                throw new ArgumentOutOfRangeException(valueName.ToString());
            if (IconList != null && IconList.Length != nameList.Length)
                throw new ArgumentOutOfRangeException(nameof(IconList));

            ToolStripMenuItem item = new ToolStripMenuItem() { Enabled = enable , Checked = false};

            //Change icon and language.
            Action IndexAndlanguageChange = LanguageSetting.AddToLangChangeEvt((getTrans) =>
            {
                int index = (int)server.GetProperty(valueName);
                if (IconList != null)
                    item.Image = IconList[index];

                string text = (itemName == null || itemName.Length == 0) ? "" : getTrans(itemName) + getTrans(new string[] { ": ", "：" });
                item.Text = text + getTrans(nameList[index]);
                string tip = (itemTip == null || itemTip.Length == 0) ? "" : getTrans(itemTip) + "\n";
                item.ToolTipText = tip + getTrans(new string[] { "Click to switch to ", "单击以切换到" }) + getTrans(nameList[(index + 1) % nameList.Length]);
            });

            //Reset Default
            IndexAndlanguageChange.Invoke();

            //Add click event.
            item.Click += (sender, e) =>
            {
                //Change value
                server.SetProperty(valueName, ((int)server.GetProperty(valueName) + 1) % nameList.Length);

                //Change icon and language.
                IndexAndlanguageChange.Invoke();
            };

            server.DefaultValueChanged(valueName);
            return item;
        }

        public static ToolStripMenuItem CreateCheckItem<T>(string[] itemName, string[] itemTip, Image itemIcon, 
            SaveableSettings<T> server, T valueName,  bool @default = false, bool enable = true) where T : Enum
        {

            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, @default, enable);

            item.BindingAndCheckProperty(server, valueName);

            server.DefaultValueChanged(valueName);
            return item;
        }

        /// <summary>
        /// Check Property for bool value container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="server"></param>
        /// <param name="valueName"></param>
        /// <param name="clickedAction"></param>
        public static void BindingAndCheckProperty<T>(this ToolStripMenuItem item, SaveableSettings<T> server, T valueName, Action<bool> clickedAction = null) where T : Enum
        {
            //ResetValue.
            item.Checked = (bool)server.GetProperty(valueName);

            //Check whether action should do.
            void _clickedAction()
            {
                if (clickedAction != null)
                    clickedAction.Invoke((bool)server.GetProperty(valueName));
                else
                    server.DefaultValueChanged(valueName);
            }
            _clickedAction();

            //Add Click Event.
            void Item_Click(object sender, EventArgs e)
            {
                //Invert checked.
                server.SetProperty(valueName, !(bool)server.GetProperty(valueName));
                item.Checked = (bool)server.GetProperty(valueName);

                //Check the subItems' Enable.
                foreach (var subItem in item.DropDownItems)
                {
                    if (subItem is ToolStripItem)
                    {
                        ((ToolStripItem)subItem).Enabled = item.Checked;
                    }
                }

                _clickedAction();
            }
            item.Click += Item_Click;
        }

        [Obsolete]
        public static void AddCheckBoxItem(ToolStripDropDown menu, string itemName, string itemTip, Image itemIcon,ControllableComponent owner, string valueName, bool @default, bool enable = true)
        {
            void Item_Click(object sender, EventArgs e)
            {
                owner.SetValuePub(valueName, !owner.GetValuePub(valueName, @default));
                owner.ExpireSolution(true);
            }
            AddClickItem(menu, itemName, itemTip, itemIcon, Item_Click, owner.GetValuePub(valueName, @default), enable);
        }

        [Obsolete]
        public static void AddMessageBoxItem(ToolStripMenuItem menu, string itemName, string itemTip, Bitmap itemIcon, Bitmap boxMap)
        {
            void Item_Click(object sender, EventArgs e)
            {
                ImageMessageBox(boxMap, itemName, itemIcon);
            }

            AddClickItem(menu.DropDown, itemName, itemTip, itemIcon, Item_Click, false);
        }
        public static ToolStripMenuItem CreateMessageBoxItem(string[] itemName, string[] itemTip, Bitmap itemIcon, Bitmap boxMap)
        {
            void Item_Click(object sender, EventArgs e)
            {
                ImageMessageBox(boxMap, itemName, itemIcon);
            }
            return CreateClickItem(itemName, itemTip, itemIcon, Item_Click);
        }
        [Obsolete]
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

        public static ToolStripMenuItem CreateFontSelector<T>(string[] itemName, string[] itemTip, SaveableSettings<T> server, T valueName)where T:Enum
        {

            void SelectFontHandler(object sender, EventArgs e)
            {
                //Create default form.
                Font beforeFont = (Font)server.GetProperty(valueName);
                Form form = GH_FontPicker.CreateFontPickerWindow(beforeFont);
                form.CreateControl();

                //Find Objects.
                GH_FontPicker picker = form.Controls.OfType<GH_FontPicker>().FirstOrDefault();
                if (picker == null)
                    return;
                Panel panel = form.Controls.OfType<Panel>().FirstOrDefault();
                if (panel == null)
                    return;

                //Add Function.
                Type pickerType = typeof(GH_FontPicker);
                GH_DigitScroller sizeScroller = pickerType.GetField("_SizeScroller", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(picker) as GH_DigitScroller;
                CheckBox boldChecker = pickerType.GetField("_BoldCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(picker) as CheckBox;
                CheckBox italicChecker = pickerType.GetField("_ItalicCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(picker) as CheckBox;
                GH_FontList fontScroller = pickerType.GetField("_FontList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(picker) as GH_FontList;
                sizeScroller.ValueChanged += PreviewFontChangedHandler;
                boldChecker.CheckedChanged += PreviewFontChangedHandler;
                italicChecker.CheckedChanged += PreviewFontChangedHandler;
                fontScroller.MouseClick += PreviewFontChangedHandler;

                //Add a default button.
                Button defaultButton = new Button()
                {
                    Text = "Default",
                    Width = Grasshopper.Global_Proc.UiAdjust(80),
                    Dock = DockStyle.Right,
                    DialogResult = DialogResult.Yes,
                };
                panel.Controls.Add(defaultButton);

                //Open form and check the language.
                var editor = Grasshopper.Instances.DocumentEditor;
                GH_WindowsFormUtil.CenterFormOnWindow(form, editor, true);
                var result = form.ShowDialog(editor);
                if (result == DialogResult.OK)
                {
                    var font = form.Tag as Font;
                    if (font != null)
                        server.SetProperty(valueName, font);
                }
                else if (result == DialogResult.Yes)
                {
                    server.ResetProperty(valueName);
                }
                else if(result == DialogResult.Cancel)
                {
                    server.SetProperty(valueName, beforeFont);
                }
                Grasshopper.Instances.ActiveCanvas?.Refresh();
                sizeScroller.ValueChanged -= PreviewFontChangedHandler;
                boldChecker.CheckedChanged -= PreviewFontChangedHandler;
                italicChecker.CheckedChanged -= PreviewFontChangedHandler;
                fontScroller.MouseClick -= PreviewFontChangedHandler;

                //Preview handler.
                void PreviewFontChangedHandler(object s, EventArgs args)
                {
                    Font previewFont = picker.SelectedFont;
                    if (previewFont != null)
                    {
                        Font currentFont = (Font)server.GetProperty(valueName);
                        server.SetProperty(valueName, previewFont);
                        Grasshopper.Instances.ActiveCanvas?.Refresh();
                        server.SetProperty(valueName, currentFont);
                    }
                }
            }

            ToolStripMenuItem item = CreateClickItem(itemName, itemTip, Properties.Resources.TextIcon, SelectFontHandler);
            return item;
        }

        public static ToolStripMenuItem CreateURLItem(string[] itemName, string[] itemTip, ItemIconType type, string url)
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
            return CreateURLItem(itemName, itemTip, itemIcon, url);
        }
        public static ToolStripMenuItem CreateURLItem(string[] itemName, string[] itemTip, Image itemIcon, string url)
        {
            void Item_Click(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start(url);
            }
            return CreateClickItem(itemName, itemTip, itemIcon, Item_Click);
        }
        [Obsolete]
        public static void AddURLItem(ToolStripMenuItem menu, string itemName, string itemTip, Image itemIcon, string url)
        {
            void Item_Click(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start(url);
            }

            AddClickItem(menu.DropDown, itemName, itemTip, itemIcon, Item_Click, false);
        }
        [Obsolete]
        public static ToolStripMenuItem CreateOneItem(string itemName, string itemTip, Image itemIcon)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemName);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            return item;
        }

        [Obsolete]
        public static ToolStripMenuItem CreateOneItem(string itemName, string itemTip, Image itemIcon, bool enable)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemName);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            item.Enabled = enable;
            return item;
        }

        [Obsolete]
        public static ToolStripMenuItem CreateClickItem<T>(string[] itemName, string[] itemTip, Image itemIcon, T tag, Action<object, EventArgs, T> click, bool @default = false, bool enable = true)
        {
            void Item_Click(object sender, EventArgs e)
            {
                click.Invoke(sender, e, tag);
            }
            return CreateClickItem(itemName, itemTip, itemIcon, Item_Click, @default, enable);
        }

        public static ToolStripMenuItem CreateClickItem(string[] itemName, string[] itemTip, Image itemIcon, EventHandler click, bool @checked = false, bool enable = true)
        {
            ToolStripMenuItem item = CreateOneItem(itemName, itemTip, itemIcon, @checked, enable);
            item.Click += click;
            return item;
        }
        #endregion

        [Obsolete]
        public static void AddLabelItem(ToolStripDropDown menu, string labelText, string labelTip = null, Color? color = null, int divisor = 6, int margin = 5, float? fontSize = null)
        {
            Color realColor = color.HasValue ? color.Value : ColorExtension.OnColor;
            ToolStripLabel item = new ToolStripLabel(labelText);
            if (fontSize == null)
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

        public static ToolStripLabel CreateLabelItem(string[] labelText, string[] labelTip = null, Color? color = null, float? fontSize = null)
        {
            ToolStripLabel item = new ToolStripLabel();

            //Set font.
            if (fontSize == null)
            {
                item.Font = GH_FontServer.StandardBold;
            }
            else
            {
                item.Font = new Font(GH_FontServer.StandardBold.FontFamily, fontSize.Value, FontStyle.Bold);
            }

            //Change color.
            Color onColor = color.HasValue ? color.Value : ColorExtension.OnColor;
            item.EnabledChanged += Item_EnabledChanged;
            void Item_EnabledChanged(object sender, EventArgs e)
            {
                item.ForeColor = item.Enabled ? onColor : ColorExtension.OffColor;
            }
            Item_EnabledChanged(item, new EventArgs());

            //Set language.
            item.SetItemLangChange(labelText, labelTip);

            return item;
        }

        public static ToolStripMenuItem CreateOneItem(string[] itemName, string[] itemTip, Image itemIcon, bool @checked = false, bool enable = true)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            if (itemIcon != null)
                item.Image = itemIcon;
            item.Checked = @checked;
            item.Enabled = enable;

            //Set LanguageChanged.
            item.SetItemLangChange(itemName, itemTip);

            return item;
        }

        public static void SetItemLangChange(this ToolStripItem item, string[] itemName, string[] itemTip)
        {
            if (itemTip == null || itemTip.Length == 0)
                itemTip = new string[] { "" };

            //LanguageChange.
            LanguageSetting.AddToLangChangeEvt((getTrans) =>
            {
                item.Text = getTrans(itemName);
                item.ToolTipText = getTrans(itemTip);
            });
        }


        [Obsolete]
        public static void AddClickItem(ToolStripDropDown menu, string itemText, string itemTip, Image itemIcon, EventHandler click, bool @default = false, bool enable = true)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(itemText);
            item.ToolTipText = itemTip;
            if (itemIcon != null)
                item.Image = itemIcon;
            item.Click += click;
            item.Checked = @default;
            item.Enabled = enable;
            menu.Items.Add(item);
        }

        public static void ImageMessageBox(Bitmap map, string[] textName, Bitmap icon)
        {
            Form form = new Form();
            IntPtr Hicon = icon.GetHicon();
            form.Icon = Icon.FromHandle(Hicon);
            Size size = new Size(map.Size.Width + 12, map.Size.Height + 15);
            form.Size = size;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.BackgroundImage = map;
            form.TopMost = true;
            form.Show();

            LanguageSetting.AddToLangChangeEvt((getTrans) =>
            {
                form.Text = getTrans(textName);
            });
        }

        [Obsolete]
        private static void ImageMessageBox(Bitmap map, string str, Bitmap icon)
        {
            Form form = new Form();
            IntPtr Hicon = icon.GetHicon();
            form.Icon = Icon.FromHandle(Hicon);
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
