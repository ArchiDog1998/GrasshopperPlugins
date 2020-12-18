using ArchiTed_Grasshopper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InfoGlasses
{
    /// <summary>
    /// WireColors.xaml 的交互逻辑
    /// </summary>
    public partial class WireColors_OBSOLETE : Window
    {
        private new WireGlassesComponent_OBSOLETE Owner;
        private Dictionary<string, System.Drawing.Color> colorSetRelay;
        private event RoutedEventHandler ResetAll;

        public WireColors_OBSOLETE(WireGlassesComponent_OBSOLETE owner)
        {
      
            this.Owner = owner;

            InitializeComponent();

            CreateAll(owner.allParamType);

            LanguageChange();

            colorSetRelay = Owner.ColorDefinationToDict();
        }

        //private Grid CreateOneObject(ParamTypeInfo info)
        //{
        //    //StackPanel stack = new StackPanel();

        //    //return stack;
        //}
        private void CreateAll(List<ParamTypeInfo> infos)
        {
            var result = infos.GroupBy((x) => { return x.AssemblyName; }).ToList();
            result.Sort((y, z) => { return y.Key.CompareTo(z.Key); });
            foreach (var infosets in result)
            {
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;
                Image image = new Image();
                image.Source = infosets.ToList()[0].AssemblyIconSource;

                System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                label.Content = infosets.Key;
                label.Padding = new Thickness(6, 0, 6, 0);
                label.VerticalAlignment = VerticalAlignment.Center;
                stack.Children.Add(image);
                stack.Children.Add(label);

                TreeViewItem item = new TreeViewItem();
                item.Header = stack;
                item.ToolTip = infosets.ToList()[0].AssemblyDesc;
                item.Expanded += Item_Expanded;
                item.Selected += SelectedExpand;

                void SelectedExpand(object sender, RoutedEventArgs e)
                {
                    TreeViewItem treeview = e.OriginalSource as TreeViewItem;
                    if (treeview == null || e.Handled)
                        return;
                    treeview.IsExpanded = !treeview.IsExpanded;

                    treeview.IsSelected = false;
                    e.Handled = true;
                }
                void Item_Expanded(object sender, RoutedEventArgs e)
                {
                    foreach (var obj in MainListView.Items)
                    {
                        if(obj != item)
                        {
                            TreeViewItem treeitem = obj as TreeViewItem;
                            treeitem.IsExpanded = false;
                        }
                    }
                }

                var sets = infosets.ToList();
                sets.Sort((x, y) => { return x.Name.CompareTo(y.Name); });
                foreach (var info in sets)
                {
                    item.Items.Add(CreateObj(info));
                }
                MainListView.Items.Add(item);
            }
        }

        

        private TreeViewItem CreateObj(ParamTypeInfo info)
        {
            TreeViewItem item = new TreeViewItem();
            Border colorbord = new Border();
            item.Header = CreateTitle(info, out colorbord);
            item.ToolTip = info.ProxyDesc;
            item.Items.Add(CreateColorPicker(info, colorbord));
            item.Selected += SelectedExpand;
            item.Expanded += Item_Expanded;

            void SelectedExpand(object sender, RoutedEventArgs e)
            {
                TreeViewItem treeview = e.OriginalSource as TreeViewItem;
                if (treeview == null || e.Handled)
                    return;
                treeview.IsExpanded = !treeview.IsExpanded;

                treeview.IsSelected = false;
                e.Handled = true;
            }
            void Item_Expanded(object sender, RoutedEventArgs e)
            {
                TreeViewItem assem = item.Parent as TreeViewItem;
                foreach (var obj in assem.Items)
                {
                    if(obj != item)
                    {
                        TreeViewItem treeitem = obj as TreeViewItem;
                        treeitem.IsExpanded = false;
                    }
                }
            }

            return item;
        }

        

        private StackPanel CreateTitle(ParamTypeInfo info, out Border colorbord)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            System.Drawing.Color color = Owner.GetColor(info);

            Image image = new Image();
            image.Source = info.IconSource;
            image.VerticalAlignment = VerticalAlignment.Center;

            Border bord = new Border();
            bord.BorderThickness = new Thickness(0);
            bord.CornerRadius = new CornerRadius(3);
            bord.Background = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            bord.Width = 12;
            bord.Height = 12;
            bord.HorizontalAlignment = HorizontalAlignment.Center;
            bord.VerticalAlignment = VerticalAlignment.Center;
            bord.Margin = new Thickness(0, 0, 5, 0);

            Label label = new Label();
            label.Content = info.Name;
            label.VerticalAlignment = VerticalAlignment.Center;

            stack.Children.Add(bord);
            stack.Children.Add(image);
            stack.Children.Add(label);

            colorbord = bord;
            return stack;
        }

        private Border CreateColorPicker(ParamTypeInfo info, Border colorbord)
        {
            #region
            StackPanel stack1 = new StackPanel();
            stack1.Orientation = Orientation.Vertical;
            System.Drawing.Color color = Owner.GetColor(info);
            stack1.Margin = new Thickness(5);


            Border bord = new Border();
            bord.Width = 70;
            bord.Height = 70;
            bord.BorderThickness = new Thickness(0);
            bord.CornerRadius = new CornerRadius(15);
            bord.Background = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            bord.Margin = new Thickness(8);

            #region
            Label buttonName = new Label();
            buttonName.Content = "Reset";
            buttonName.Foreground = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            buttonName.HorizontalAlignment = HorizontalAlignment.Center;
            buttonName.VerticalAlignment = VerticalAlignment.Center;
            buttonName.FontSize = 15;
            buttonName.FontWeight = FontWeight.FromOpenTypeWeight(50);

            //Border buttonBorder = new Border();
            //buttonBorder.CornerRadius = new CornerRadius(10);
            //buttonBorder.BorderThickness = new Thickness(2);
            //buttonBorder.Width = 60;
            //buttonBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            //buttonBorder.Margin = new Thickness(0);
            //buttonBorder.Child = buttonName;
            
            Button button = new Button();
            button.Content = buttonName;
            button.BorderThickness = new Thickness(0);
            button.Foreground = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            button.Margin = new Thickness( 0, 5, 0, 5);
            #endregion

            stack1.Children.Add(bord);
            stack1.Children.Add(button);
            #endregion

            #region
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Vertical;

            byte gray = 127;
            byte high = 200;

            Slider Aslider = new Slider();
            stack.Children.Add( CreateASliderSet("A", color.A, Color.FromRgb(gray, gray, gray), out Aslider));
            Slider Rslider = new Slider();
            stack.Children.Add(CreateASliderSet("R", color.R, Color.FromRgb(high, gray, gray), out Rslider));
            Slider Gslider = new Slider();
            stack.Children.Add(CreateASliderSet("G", color.G, Color.FromRgb(gray, high, gray), out Gslider));
            Slider Bslider = new Slider();
            stack.Children.Add(CreateASliderSet("B", color.B, Color.FromRgb(gray, gray, high), out Bslider));

            
            #endregion
            
            StackPanel wholeStack = new StackPanel();
            wholeStack.Orientation = Orientation.Horizontal;
            wholeStack.Children.Add(stack1);
            wholeStack.Children.Add(stack);
            Border wholeBorder = new Border();
            wholeBorder.Child = wholeStack;
            wholeBorder.BorderThickness = new Thickness(3);
            wholeBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            wholeBorder.CornerRadius = new CornerRadius(30);
            wholeBorder.Padding = new Thickness(5);

            void Button_Click(object sender, RoutedEventArgs e)
            {
                Aslider.Value = Owner.defaultColor.A;
                Rslider.Value = Owner.defaultColor.R;
                Gslider.Value = Owner.defaultColor.G;
                Bslider.Value = Owner.defaultColor.B;
                RespondToColorChanged(bord, Owner.defaultColor, info, wholeBorder, buttonName, colorbord);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }
            void ChangeEvents(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                System.Drawing.Color setColor = System.Drawing.Color.FromArgb((int)Aslider.Value, (int)Rslider.Value, (int)Gslider.Value, (int)Bslider.Value);
                RespondToColorChanged(bord, setColor, info, wholeBorder, buttonName, colorbord);
                Grasshopper.Instances.ActiveCanvas.Refresh();
            }
            void ToReset(object sender, RoutedEventArgs e)
            {
                Aslider.Value = Owner.defaultColor.A;
                Rslider.Value = Owner.defaultColor.R;
                Gslider.Value = Owner.defaultColor.G;
                Bslider.Value = Owner.defaultColor.B;
                RespondToColorChanged(bord, Owner.defaultColor, info, wholeBorder, buttonName, colorbord);
            }
            ResetAll += ToReset;

            button.Click += Button_Click;
            Aslider.ValueChanged += ChangeEvents;
            Rslider.ValueChanged += ChangeEvents;
            Gslider.ValueChanged += ChangeEvents;
            Bslider.ValueChanged += ChangeEvents;

            return wholeBorder;
        }

        private void RespondToColorChanged(Border colorBorder, System.Drawing.Color color, ParamTypeInfo info, Border wholeBorder, Label buttonName, Border colorbord)
        {
            colorBorder.Background = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            Owner.SetColor(info, color);
            wholeBorder.BorderBrush =  new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            colorbord.Background = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            buttonName.Foreground = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            //Grasshopper.Instances.ActiveCanvas.Refresh();
        }

        

        private StackPanel CreateASliderSet(string name,int num,Color color, out Slider slider)
        {

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            Label label = new Label();
            label.Content = name;
            label.Margin = new Thickness(3);
            label.Width = 20;
            label.FontSize = 15;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(color);


            Slider newslider = new Slider();
            newslider.Minimum = 0;
            newslider.Maximum = 255;
            newslider.Value = num;
            newslider.Width = 200;
            newslider.Height = 20;
            newslider.VerticalAlignment = VerticalAlignment.Center;
            newslider.HorizontalAlignment = HorizontalAlignment.Center;
            newslider.Foreground = new SolidColorBrush(color);
            slider = newslider;

            Label label2 = new Label();
            label2.Content = num.ToString();
            label2.Margin = new Thickness(3);
            label2.Width = 40;
            label2.VerticalAlignment = VerticalAlignment.Center;
            label2.HorizontalAlignment = HorizontalAlignment.Center;
            label2.HorizontalContentAlignment = HorizontalAlignment.Center;
            label2.VerticalContentAlignment = VerticalAlignment.Center;


            void Newslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {
                label2.Content = ((int)newslider.Value).ToString();
            }

            newslider.ValueChanged += Newslider_ValueChanged;

            stack.Children.Add(label);
            stack.Children.Add(newslider);
            stack.Children.Add(label2);
            return stack;

        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.WriteTxt(Owner.ColorDefinationToDict());
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetAll(this, new RoutedEventArgs());
            Grasshopper.Instances.ActiveCanvas.Refresh();
            //this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.ResetColor();
            Owner.ReadDictionary(colorSetRelay);
            Grasshopper.Instances.ActiveCanvas.Refresh();
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void WindowLanguageChanged(object sender, EventArgs e)
        {
            LanguageChange();
        }

        private void LanguageChange()
        {
            this.Title = LanguagableComponent.GetTransLation(new string[] { "Wire Color Window", "连线颜色窗口" });

            DefaultButton.Content = LanguagableComponent.GetTransLation(new string[] { "AsDefualt", "设为预设" });
            DefaultButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to make the current selection the default.Note: This will generate a txt file called WireGlasses_Default in the AssemblyDefault folder",
                "单击以将当前选择设为默认选项。注意：这将在默认程序集插件文件夹下生成一个叫WireGlasses_Default的txt文件。" });

            CancelButton.Content = LanguagableComponent.GetTransLation(new string[] { "Cancel", "取消" });
            ResetButton .Content = LanguagableComponent.GetTransLation(new string[] { "Reset", "重置" });
            OKButton.Content = LanguagableComponent.GetTransLation(new string[] { "OK", "确定" });
        }

        
    }
}
