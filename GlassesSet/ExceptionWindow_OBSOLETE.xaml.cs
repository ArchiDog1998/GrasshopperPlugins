using ArchiTed_Grasshopper;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InfoGlasses
{



    /// <summary>
    /// ExceptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExceptionWindow_OBSOLETE : Window
    {
        public List<ComTypeInfo> normalExceptionGuidCopy = new List<ComTypeInfo>();
        public List<ComTypeInfo> pluginExceptionGuidCopy = new List<ComTypeInfo>();

        private new InfoGlassesComponent_OBSOLETE Owner;

        public ExceptionWindow_OBSOLETE(InfoGlassesComponent_OBSOLETE owner)
        {
            this.Owner = owner;
            foreach (ComTypeInfo guid in owner.normalExceptionGuid)
            {
                normalExceptionGuidCopy.Add(guid);
            }
            foreach (ComTypeInfo guid1 in owner.pluginExceptionGuid)
            {
                pluginExceptionGuidCopy.Add(guid1);
            }

            InitializeComponent();


            DrawAll(Tree, owner.allObjectTypes);

            LanguageChanged();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Owner.normalExceptionGuid = this.normalExceptionGuidCopy;
            Owner.pluginExceptionGuid = this.pluginExceptionGuidCopy;
            Owner.ExpireSolution(true);
            this.Close();
        }

        

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Owner.UpdateIsShow();
            Grasshopper.Instances.ActiveCanvas.Refresh();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Owner.UpdateIsShow();
            Grasshopper.Instances.ActiveCanvas.Refresh();
            this.Close();
        }

        public void WindowLanguageChanged(object sender, EventArgs e)
        {
            LanguageChanged();
        }

        private void LanguageChanged()
        {
            AllNormalCheckBox.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to prohibit the display of the name, category, and assembly of all components. ",
                    "点击以禁止所有运算器的名称、类别、类库显示。" });
            AllNormalLabel.ToolTip = AllNormalCheckBox.ToolTip;
            AllNormalLabel.Content = LanguagableComponent.GetTransLation(new string[] { "All Normal", "所有常规" });

            AllPluginCheckBox.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to prohibit the display of plug-in highlight of all components. ",
                    "点击以禁止所有运算器的插件高亮显示。" });
            AllPluginLabel.ToolTip = AllPluginCheckBox.ToolTip;
            AllPluginLabel.Content = LanguagableComponent.GetTransLation(new string[] { "All Plugin", "所有插件" });


            foreach (var a in Tree.Items)
            {
                StackPanel stackCate = (StackPanel)(((TreeViewItem)a).Header);
                ((CheckBox)stackCate.Children[0]).ToolTip = LanguagableComponent.GetTransLation(
                                new string[] { "Click to prohibit the display of the name, category, and assembly of the components in this category. ", "点击以禁止此类别中的所有运算器的名称、类别、类库显示。" });
                ((CheckBox)stackCate.Children[1]).ToolTip = LanguagableComponent.GetTransLation(
                    new string[] { "Click to prohibit the display of plug-in highlight of the components in this category. ", "点击以禁止此类别中的所有运算器的插件高亮显示。" });


                foreach (var b in ((TreeViewItem)a).Items)
                {
                    StackPanel stackSub = (StackPanel)(((TreeViewItem)b).Header);
                    ((CheckBox)stackSub.Children[0]).ToolTip = LanguagableComponent.GetTransLation(
                                    new string[] { "Click to prohibit the display of the name, category, and assembly of the components in this subcategory. ", "点击以禁止此子类别中的所有运算器的名称、类别、类库显示。" });
                    ((CheckBox)stackSub.Children[1]).ToolTip = LanguagableComponent.GetTransLation(
                        new string[] { "Click to prohibit the display of plug-in highlight of the components in this subcategory. ", "点击以禁止此子类别中的所有运算器的插件高亮显示。" });

                    foreach (var c in ((TreeViewItem)b).Items)
                    {
                        if (c is StackPanel)
                        {
                            StackPanel stack = c as StackPanel;
                            ((CheckBox)stack.Children[0]).ToolTip = LanguagableComponent.GetTransLation(
                                new string[] { "Click to prohibit the display of the name, category, and assembly of this component. ", "点击以禁止此运算器的名称、类别、类库显示。" });
                            ((CheckBox)stack.Children[1]).ToolTip = LanguagableComponent.GetTransLation(
                                new string[] { "Click to prohibit the display of plug-in highlight of this component. ", "点击以禁止此运算器的插件高亮显示。" });
                        }
                    }
                }
            }

            AddNormalCheckBox.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to prohibit the display of the name, category, and assembly of the selected component in the document.",
                "点击以禁止在文档中选中的运算器的名称、类别、类库显示。" });
            AddPluginCheckBox.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to prohibit the display of plug-in highlight of the selected component in the document.",
                "点击以禁止在文档中选中的运算器的插件高亮显示。" });

            DefaultButton.Content = LanguagableComponent.GetTransLation(new string[] { "AsDefualt", "设为预设" });
            DefaultButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to make the current selection the default.Note: This will generate a txt file called Infoglasses_Default in the AssemblyDefault folder", 
                "单击以将当前选择设为默认选项。注意：这将在默认程序集插件文件夹下生成一个叫Infoglasses_Default的txt文件。" });

            CancelButton.Content = LanguagableComponent.GetTransLation(new string[] { "Cancel", "取消" });
            UpdateButton.Content = LanguagableComponent.GetTransLation(new string[] { "Apply", "应用" });
            OKButton.Content = LanguagableComponent.GetTransLation(new string[] { "OK", "确定" });
            this.Title = LanguagableComponent.GetTransLation(new string[] { "Exception Window", "除去项窗口" });

            AddButton.Content = LanguagableComponent.GetTransLation(new string[] { "Add", "添加" });
            AddButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to add the selected components in the view to the Exceptions Window.", "点击以将视图中选中的运算器添加到除去项窗口中。" });
            RemoveButton.Content = LanguagableComponent.GetTransLation(new string[] { "Removt", "减少" });
            RemoveButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to remove the selected components in the view to the Exceptions Window.", "点击以将视图中选中的运算器从除去项窗口中移除。" });
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                LanguagableComponent.LanguageChanged -= WindowLanguageChanged;
            }
            catch
            {

            }
           
            base.OnClosed(e);
        }


        #region
        public void DrawAll(System.Windows.Controls.TreeView treeView, List<ComTypeInfo> infos)
        {
            AllNormalCheckBox.Click += AllNormalCheckBox_Click;
            void AllNormalCheckBox_Click(object sender, RoutedEventArgs e)
            {
                SetAll(Tree, 0, AllNormalCheckBox.IsChecked.Value);
            }

            AllPluginCheckBox.Click += AllPluginCheckBox_Click;
            void AllPluginCheckBox_Click(object sender, RoutedEventArgs e)
            {
                SetAll(Tree, 1, AllPluginCheckBox.IsChecked.Value);
            }

            

            var result = infos.GroupBy((x) => { return x.Category; }).ToList();
            result.Sort((y, z) => { return y.Key.CompareTo(z.Key); });
            foreach (var items in result)
            {
                StackPanel stack = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };

                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Width = 16;
                image.Height = 16;
                try
                {
                    Bitmap bitmap = new Bitmap(Grasshopper.Instances.ComponentServer.GetCategoryIcon(items.Key), 16, 16);
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;
                }
                catch
                {

                }
                
                

                System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                label.Content = items.Key;
                label.Padding = new Thickness(6, 0, 6, 0);
                label.VerticalAlignment = VerticalAlignment.Center;

                

                TreeViewItem Category = new TreeViewItem();
                DrawOneCate(Category, items.ToList());
                Category.Selected += SelectedExpand;

                System.Windows.Controls.CheckBox normalCheckbox = new System.Windows.Controls.CheckBox();
                normalCheckbox.VerticalAlignment = VerticalAlignment.Center;
                normalCheckbox.Checked += NormalCheckbox_Checked;
                normalCheckbox.Unchecked += NormalCheckbox_Unchecked;
                normalCheckbox.Click += NormalCheckbox_Click;

                System.Windows.Controls.CheckBox pluginCheckbox = new System.Windows.Controls.CheckBox();
                pluginCheckbox.VerticalAlignment = VerticalAlignment.Center;
                pluginCheckbox.Checked += PluginCheckbox_Checked;
                pluginCheckbox.Unchecked += PluginCheckbox_Unchecked;
                pluginCheckbox.Click += PluginCheckbox_Click;

                void NormalCheckbox_Checked(object sender, RoutedEventArgs e)
                {
                    SetCate(Category, 0, true);
                }
                void NormalCheckbox_Unchecked(object sender, RoutedEventArgs e)
                {
                    SetCate(Category, 0, false);
                }
                void NormalCheckbox_Click(object sender, RoutedEventArgs e)
                {
                    UpdateAll(Tree, 0);
                }
                void PluginCheckbox_Checked(object sender, RoutedEventArgs e)
                {
                    SetCate(Category, 1, true);
                }
                void PluginCheckbox_Unchecked(object sender, RoutedEventArgs e)
                {
                    SetCate(Category, 1, false);
                }
                void PluginCheckbox_Click(object sender, RoutedEventArgs e)
                {
                    UpdateAll(Tree, 1);
                }

                stack.Children.Add(normalCheckbox);
                stack.Children.Add(pluginCheckbox);
                stack.Children.Add(image);
                stack.Children.Add(label);

                Category.Header = stack;

                UpdateCate(Category, 0, 0);
                UpdateCate(Category, 1, 1);

                treeView.Items.Add(Category);
            }

            UpdateAll(treeView, 0);
            UpdateAll(treeView, 1);
        }

        private void SelectedExpand(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeview = e.OriginalSource as TreeViewItem;
            if (treeview == null || e.Handled)
                return;
            treeview.IsExpanded = !treeview.IsExpanded;

            treeview.IsSelected = false;
            e.Handled = true;
        }

        private void SetAll(TreeView Tree, int i, bool flag)
        {
            foreach (var c in Tree.Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stack = ((TreeViewItem)c).Header as StackPanel;
                    ((CheckBox)stack.Children[i]).IsChecked = flag;
                }
            }
        }

        private void UpdateAll(TreeView Tree, int checkIndex)
        {
            bool check = true;
            foreach (var c in Tree.Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stack = ((TreeViewItem)c).Header as StackPanel;
                    if(((CheckBox)stack.Children[checkIndex]).IsChecked !=true)
                    {
                        check = false;
                        break;
                    }
                }
            }
            bool uncheck = true;
            foreach (var c in Tree.Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stack = ((TreeViewItem)c).Header as StackPanel;
                    if (((CheckBox)stack.Children[checkIndex]).IsChecked != false)
                    {
                        uncheck = false;
                        break;
                    }
                }
            }
            if(checkIndex == 0)
            {
                if (check && AllNormalCheckBox.IsChecked != true)
                {
                    AllNormalCheckBox.IsChecked = true;
                }
                else if (uncheck && AllNormalCheckBox.IsChecked != false)
                {
                    AllNormalCheckBox.IsChecked = false;
                }
                else if (!check && !uncheck && AllNormalCheckBox.IsChecked != null)
                {
                    AllNormalCheckBox.IsChecked = null;
                }
            }
            else if(checkIndex == 1)
            {
                if (check && AllPluginCheckBox.IsChecked != true)
                {
                    AllPluginCheckBox.IsChecked = true;
                }
                else if (uncheck && AllPluginCheckBox.IsChecked != false)
                {
                    AllPluginCheckBox.IsChecked = false;
                }
                else if (!check && !uncheck && AllPluginCheckBox.IsChecked != null)
                {
                    AllPluginCheckBox.IsChecked = null;
                }
            }

            
        }

        private void DrawOneCate(TreeViewItem Cate, List<ComTypeInfo> infos)
        {

            var result = infos.GroupBy((x) => { return x.Subcategory; }).ToList();
            result.Sort((y, z) => { return y.Key.CompareTo(z.Key); });
            foreach (var items in result)
            {
                TreeViewItem SubCate = new TreeViewItem();
                SubCate.Selected += SelectedExpand;
                DrawOneSubcate(SubCate, items.ToList(), items.Key);
                Cate.Items.Add(SubCate);
            }
        }

        private void SetCate(TreeViewItem Cate, int i, bool flag)
        {
            foreach (var c in Cate.Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stack = ((TreeViewItem)c).Header as StackPanel;
                    ((CheckBox)stack.Children[i]).IsChecked = flag;
                }
            }
        }
        private void UpdateCate(TreeViewItem Cate, int checkIndex, int cateIndex)
        {
            bool check = true;
            foreach (var c in ((TreeViewItem)Cate).Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stackrelay = ((TreeViewItem)c).Header as StackPanel;
                    if (((CheckBox)stackrelay.Children[checkIndex]).IsChecked != true)
                    {
                        check = false;
                        break;
                    }
                }
            }
            
            bool uncheck = true;
            foreach (var c in ((TreeViewItem)Cate).Items)
            {
                if (c is TreeViewItem)
                {
                    StackPanel stackrelay = ((TreeViewItem)c).Header as StackPanel;
                    if (((CheckBox)stackrelay.Children[checkIndex]).IsChecked == true || ((CheckBox)stackrelay.Children[checkIndex]).IsChecked == null)
                    {
                        uncheck = false;
                        break;
                    }
                }
            }

            if (check && ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked != true)
            {
                ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked = true;
                try
                {
                    UpdateAll(Tree, cateIndex);
                }
                catch
                {

                }
            }
            else if (uncheck && ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked != false)
            {
                ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked = false;
                try
                {
                    UpdateAll(Tree, cateIndex);
                }
                catch
                {

                }
            }
            else if (!check && !uncheck && ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked != null)
            {
                ((CheckBox)(((StackPanel)(Cate.Header)).Children[cateIndex])).IsChecked = null;
                try
                {
                    UpdateAll(Tree, cateIndex);
                }
                catch
                {

                }
            }
        }
         
        private void DrawOneSubcate(TreeViewItem Subcate, List<ComTypeInfo> infos, string name)
        {
            var result = infos.GroupBy((x) => { return x.Exposure; }).ToList();
            result.Sort((y, z) => { return y.Key.CompareTo(z.Key); });
            DrawOneSection(Subcate, result[0].ToList());

            System.Windows.Controls.CheckBox normalCheckbox = new System.Windows.Controls.CheckBox();
            normalCheckbox.VerticalAlignment = VerticalAlignment.Center;
            normalCheckbox.Checked += NormalCheckbox_Checked;
            normalCheckbox.Unchecked += NormalCheckbox_Unchecked;
            normalCheckbox.Click += NormalCheckbox_Click;

            System.Windows.Controls.CheckBox pluginCheckbox = new System.Windows.Controls.CheckBox();
            pluginCheckbox.VerticalAlignment = VerticalAlignment.Center;
            pluginCheckbox.Checked += PluginCheckbox_Checked;
            pluginCheckbox.Unchecked += PluginCheckbox_Unchecked;
            pluginCheckbox.Click += PluginCheckbox_Click;

            for (int i = 1; i < result.Count; i++)
            {
                Subcate.Items.Add(new Separator() { Width = 250, Height = 10});
                DrawOneSection(Subcate, result[i].ToList());
            }

            void NormalCheckbox_Checked(object sender, RoutedEventArgs e)
            {
                SetSubcate(Subcate, 0, true);
            }
            void NormalCheckbox_Unchecked(object sender, RoutedEventArgs e)
            {
                SetSubcate(Subcate, 0, false);
            }
            void NormalCheckbox_Click(object sender, RoutedEventArgs e)
            {
                TreeViewItem Cate = ((TreeViewItem)(((StackPanel)(normalCheckbox.Parent)).Parent)).Parent as TreeViewItem;
                UpdateCate(Cate, 0, 0);
            }
            void PluginCheckbox_Checked(object sender, RoutedEventArgs e)
            {

                SetSubcate(Subcate, 1, true);
            }
            void PluginCheckbox_Unchecked(object sender, RoutedEventArgs e)
            {

                SetSubcate(Subcate, 1, false);
            }
            void PluginCheckbox_Click(object sender, RoutedEventArgs e)
            {
                TreeViewItem Cate = ((TreeViewItem)(((StackPanel)(normalCheckbox.Parent)).Parent)).Parent as TreeViewItem;
                UpdateCate(Cate, 1, 1);
            }

            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = name;
            label.Padding = new Thickness(12, 0, 12, 0);
            label.VerticalAlignment = VerticalAlignment.Center;

            StackPanel stackHeader = new StackPanel();
            stackHeader.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stackHeader.Margin = new Thickness(1);
            stackHeader.Children.Add(normalCheckbox);
            stackHeader.Children.Add(pluginCheckbox);
            stackHeader.Children.Add(label);

            Subcate.Header = stackHeader;

            UpdateSubcate(Subcate, 0, 0);
            UpdateSubcate(Subcate, 1, 1);
        }

        private void SetSubcate(TreeViewItem Subcate, int i, bool flag)
        {
            foreach (var c in ((TreeViewItem)Subcate).Items)
            {
                if (c is StackPanel)
                {
                    StackPanel stack = c as StackPanel;
                    ((CheckBox)stack.Children[i]).IsChecked = flag;
                }
            }
        }

        private void UpdateSubcate(TreeViewItem Subcate, int checkIndex, int subIndex)
        {
            bool check = true;
            foreach (var c in ((TreeViewItem)Subcate).Items)
            {
                if (c is StackPanel)
                {
                    StackPanel stackrelay = c as StackPanel;
                    if (((CheckBox)stackrelay.Children[checkIndex]).IsChecked != true)
                    {
                        check = false;
                        break;
                    }
                }
            }
            
            bool uncheck = true;
            foreach (var c in ((TreeViewItem)Subcate).Items)
            {
                if (c is StackPanel)
                {
                    StackPanel stackrelay = c as StackPanel;
                    if (((CheckBox)stackrelay.Children[checkIndex]).IsChecked != false)
                    {
                        uncheck = false;
                        break;
                    }
                }
            }
            if (check && ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked != true)
            {
                ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked = true;
                try
                {
                    UpdateCate((TreeViewItem)(Subcate.Parent), checkIndex, subIndex);
                }
                catch
                {

                }


            }
            else if (uncheck && ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked != false)
            {
                ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked = false;
                try
                {
                    UpdateCate((TreeViewItem)(Subcate.Parent), checkIndex, subIndex);
                }
                catch
                {

                }
            }
            else if ((!check) && (!uncheck) && ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked != null)
            {
                ((CheckBox)(((StackPanel)(Subcate.Header)).Children[subIndex])).IsChecked = null;
                try
                {
                    UpdateCate((TreeViewItem)(Subcate.Parent), checkIndex, subIndex);
                }
                catch
                {

                }
            }
        }


        private void DrawOneSection(TreeViewItem Subcate, List<ComTypeInfo> infos)
        {
            infos.Sort((x, y) => { return x.Name.CompareTo(y.Name); });
            foreach (var item in infos)
            {
                Subcate.Items.Add(DrawOneObject(item));

            }

        }

        private StackPanel DrawOneObject(ComTypeInfo info)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stack.Margin = new Thickness(1);


            #region
            System.Windows.Controls.CheckBox normalCheckbox = new System.Windows.Controls.CheckBox();
            normalCheckbox.VerticalAlignment = VerticalAlignment.Center;
            normalCheckbox.Padding = new Thickness(3);
            normalCheckbox.IsChecked = Owner.normalExceptionGuid.Contains(info);
            normalCheckbox.Checked += NormalCheckbox_Checked;
            normalCheckbox.Unchecked += NormalCheckbox_Unchecked;
            normalCheckbox.Click += NormalCheckbox_Click;
            void NormalCheckbox_Checked(object sender, RoutedEventArgs e)
            {
                if (!Owner.normalExceptionGuid.Contains(info))
                    Owner.normalExceptionGuid.Add(info);
            }
            void NormalCheckbox_Unchecked(object sender, RoutedEventArgs e)
            {
                if(Owner.normalExceptionGuid.Contains(info))
                    Owner.normalExceptionGuid.Remove(info);
            }
            void NormalCheckbox_Click(object sender, RoutedEventArgs e)
            {
                UpdateCheckBox(normalCheckbox, 0);
            }
            #endregion

            #region
            System.Windows.Controls.CheckBox pluginCheckbox = new System.Windows.Controls.CheckBox();
            pluginCheckbox.VerticalAlignment = VerticalAlignment.Center;
            pluginCheckbox.Padding = new Thickness(3, 3, 8, 3);
            pluginCheckbox.IsChecked = Owner.pluginExceptionGuid.Contains(info);
            pluginCheckbox.Checked += PluginCheckbox_Checked;
            pluginCheckbox.Unchecked += PluginCheckbox_Unchecked;
            pluginCheckbox.Click += PluginCheckbox_Click;
            void PluginCheckbox_Checked(object sender, RoutedEventArgs e)
            {
                if (!Owner.pluginExceptionGuid.Contains(info))
                    Owner.pluginExceptionGuid.Add(info);
            }
            void PluginCheckbox_Unchecked(object sender, RoutedEventArgs e)
            {
                if (Owner.pluginExceptionGuid.Contains(info))
                    Owner.pluginExceptionGuid.Remove(info);
            }
            void PluginCheckbox_Click(object sender, RoutedEventArgs e)
            {
                UpdateCheckBox(pluginCheckbox, 1);
            }

            #endregion

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = info.Icon;
            image.ToolTip = info.Tip;

            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = info.Name;
            label.Padding = new Thickness(12, 0, 12, 0);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.ToolTip = info.Tip;

            stack.Children.Add(normalCheckbox);
            stack.Children.Add(pluginCheckbox);
            stack.Children.Add(image);
            stack.Children.Add(label);


            return stack;
        }

        private void UpdateCheckBox(CheckBox box, int i)
        {
            TreeViewItem Subcate = ((StackPanel)(box.Parent)).Parent as TreeViewItem;
            UpdateSubcate(Subcate, i, i);
        }

        #endregion

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (var a in Tree.Items)
            {
                foreach (var b in ((TreeViewItem)a).Items)
                {
                    foreach (var c in ((TreeViewItem)b).Items)
                    {
                        if (c is StackPanel)
                        {
                            StackPanel stack = c as StackPanel;
                            foreach (IGH_DocumentObject obj in Owner.OnPingDocument().SelectedObjects())
                            {
                                string tip = ((string)((Label)stack.Children[3]).ToolTip);
                                if (tip.Contains(obj.Description) && tip.Contains(obj.Name))
                                {
                                    if (AddNormalCheckBox.IsChecked.Value && ((CheckBox)stack.Children[0]).IsChecked != true)
                                    {
                                        ((CheckBox)stack.Children[0]).IsChecked = true;
                                        UpdateCheckBox(((CheckBox)stack.Children[0]), 0);
                                    }

                                    if (AddPluginCheckBox.IsChecked.Value && ((CheckBox)stack.Children[1]).IsChecked != true)
                                    {
                                        ((CheckBox)stack.Children[1]).IsChecked = true;
                                        UpdateCheckBox(((CheckBox)stack.Children[1]), 1);
                                    }

                                }
                            } 
                        }
                    }
                }
            }
            Owner.UpdateIsShow();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var a in Tree.Items)
            {
                foreach (var b in ((TreeViewItem)a).Items)
                {
                    foreach (var c in ((TreeViewItem)b).Items)
                    {
                        if (c is StackPanel)
                        {
                            StackPanel stack = c as StackPanel;
                            foreach (IGH_DocumentObject obj in Owner.OnPingDocument().SelectedObjects())
                            {
                                string tip = ((string)((Label)stack.Children[3]).ToolTip);
                                if (tip.Contains(obj.Description) && tip.Contains(obj.Name))
                                {
                                    if (AddNormalCheckBox.IsChecked.Value && ((CheckBox)stack.Children[0]).IsChecked != false)
                                    {
                                        ((CheckBox)stack.Children[0]).IsChecked = false;
                                        UpdateCheckBox(((CheckBox)stack.Children[0]), 0);
                                    }

                                    if (AddPluginCheckBox.IsChecked.Value && ((CheckBox)stack.Children[1]).IsChecked != false)
                                    {
                                        ((CheckBox)stack.Children[1]).IsChecked = false;
                                        UpdateCheckBox(((CheckBox)stack.Children[1]), 1);
                                    }

                                }

                            }
                        }
                    }
                }
            }
            Owner.UpdateIsShow();
        }

        private void DefualtButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.Writetxt();
        }
    }




}
