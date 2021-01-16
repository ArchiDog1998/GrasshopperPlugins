using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WPF;
using System;
using System.Reflection;
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
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using InfoGlasses.WinformControls;
using MaterialDesignThemes.Wpf;

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for SelectOneParamWindow.xaml
    /// </summary>
    public partial class SelectOneParamWindow : LangWindow
    {
        private GooTypeProxy _proxy { get; }
        private byte _paramIndex { get; set; }
        private ParamGlassesComponent _paramOwner { get; }
        private bool _isInput { get; }
        private ParamGlassesProxy _selectedProxy;
        public SelectOneParamWindow(ParamGlassesComponent owner, bool isInput, GooTypeProxy proxy)
            :base()
        {
            this._proxy = proxy;
            this._isInput = isInput;
            this._paramOwner = owner;
            InitializeComponent();

            SetShowProxy(_paramOwner.AllProxy);
            DrawDataTree(_paramOwner.AllProxy);
        }

        #region Language
        protected override void LanguageChanged()
        {
            #region Top Tree Translate
            WindowTitle.Text = LanguagableComponent.GetTransLation(new string[] { "Select one component", "选择一个运算器" });
            HintAssist.SetHint(SearchBox, new Label()
            {
                Content = LanguagableComponent.GetTransLation(new string[] { "Search", "搜索" }),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            CancelButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to cancel the changes and close this window.", "单击以取消修改，并关闭此窗口。" });
            OKButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to comfirm the change and close the window.", "单击以确认修改并关闭窗口。" });

            #endregion

            #region Major Box
            IconColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Icon", "图标" });
            FullNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "FullName", "全名" });
            CategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Category", "类别" });
            SubcategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Subcategory", "子类别" });
            ExposureColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Exposure", "分栏" });
            GuidColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Guid", "全局唯一标识符（Guid）" });

            FirstExpenderName.Text = LanguagableComponent.GetTransLation(new string[] { "Filter", "过滤器" });
            #endregion
        }
        #endregion

        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            IndexTextBox.Text = null;
            ActiveBorder.Visibility = Visibility.Visible;
            this.ComponentName.Text = _selectedProxy.FullName;
            System.Drawing.Bitmap bitmap = CanvasRenderEngine.GetObjectBitmap(_selectedProxy.CreateObejct(), this._isInput, out _);
            ShowcaseImage.Source = CanvasRenderEngine.BitmapToBitmapImage(bitmap);

            this.DialogCancelButton.Content = LanguagableComponent.GetTransLation(new string[] { "CANCEL", "取消" });
            this.DialogFinishButton.Content = LanguagableComponent.GetTransLation(new string[] { "ACCEPT", "接受" });
            HintAssist.SetHint(this.IndexTextBox, _isInput ? LanguagableComponent.GetTransLation(new string[] { "Output Index", "输出端索引" }) :
                LanguagableComponent.GetTransLation(new string[] { "Input Index", "输入端索引" }));
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Hidden;
        }

        #region Dialog
        private void SerachCancelButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = null;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            var q = Search(SearchBox.Text, _paramOwner.AllProxy.ToArray());
            if (q.Count > 0)
            {
                List<ParamGlassesProxy> proxies = new List<ParamGlassesProxy>();
                foreach (var item in q)
                {
                    proxies.Add(item as ParamGlassesProxy);
                }
                SetShowProxy(proxies);
            }

        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FindButton_Click(null, null);
            }
        }

        private void SetShowProxy(List<ParamGlassesProxy> proxies, string Category = null, string Subcategory = null)
        {
            if (Category != null)
                proxies = (proxies.Where((x) => x.Category == Category).ToList());
            if (Subcategory != null)
                proxies = (proxies.Where((x) => x.Subcategory == Subcategory).ToList());

            this.Datas.ItemsSource = proxies;
        }

        private void DrawDataTree(List<ParamGlassesProxy> proxies)
        {
            if (LeftCateTree == null)
                return;

            var result = proxies.GroupBy((x) => x.Category).ToList();
            result.Sort((x, y) => x.Key.CompareTo(y.Key));

            foreach (var categoryItem in result)
            {
                TreeViewItem viewItem = new TreeViewItem();

                #region Dim the CategoryName
                StackPanel panel = new StackPanel();
                panel.MouseDown += CategoryClick;
                panel.Orientation = Orientation.Horizontal;
                try
                {
                    panel.Children.Add(new Image()
                    {
                        Source = CanvasRenderEngine.BitmapToBitmapImage(
                            new System.Drawing.Bitmap(Grasshopper.Instances.ComponentServer.GetCategoryIcon(categoryItem.Key), 20, 20)),
                    });
                }
                catch { }

                panel.Children.Add(new Label()
                {
                    Content = categoryItem.Key,
                    FontSize = 15,
                    Margin = new Thickness(5, 0, 0, 0),
                });
                viewItem.Header = panel;
                #endregion

                var relayResult = categoryItem.ToList().GroupBy((x) => x.Subcategory).ToList();
                relayResult.Sort((x, y) => x.Key.CompareTo(y.Key));

                foreach (var SubcategoryItem in relayResult)
                {
                    Label label = new Label()
                    {
                        Content = SubcategoryItem.Key,
                        Margin = new Thickness(5, 0, 0, 0),
                        Foreground = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    };


                    TreeViewItem treeView = new TreeViewItem()
                    {
                        Header = label,
                    };

                    treeView.Selected += SubcateClick;

                    viewItem.Items.Add(treeView);
                }

                LeftCateTree.Items.Add(viewItem);
            }
        }

        private void SubcateClick(object sender, RoutedEventArgs e)
        {
            string subcate = ((Label)((TreeViewItem)sender).Header).Content as string;
            string category = ((Label)((StackPanel)((TreeViewItem)((TreeViewItem)sender).Parent).Header).Children[1]).Content as string;
            SetShowProxy(_paramOwner.AllProxy, category, subcate);

        }



        private void CategoryClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetShowProxy(_paramOwner.AllProxy, ((Label)((StackPanel)sender).Children[1]).Content as string);
            }
        }

        private void CategoryCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CategoryColumn.Visibility = GetVisibility((CheckBox)sender);
        }

        private void SubcateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            SubcategoryColumn.Visibility = GetVisibility((CheckBox)sender);
        }

        private void ExposureCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ExposureColumn.Visibility = GetVisibility((CheckBox)sender);
        }

        private void GuidCheckBox_Click(object sender, RoutedEventArgs e)
        {
            GuidColumn.Visibility = GetVisibility((CheckBox)sender);
        }


        private System.Windows.Visibility GetVisibility(CheckBox box)
        {
            System.Windows.Visibility visi;

            switch (box.IsChecked)
            {
                case true:
                    visi = Visibility.Visible;
                    break;
                case false:
                    visi = Visibility.Hidden;
                    break;
                default:
                    visi = Visibility.Collapsed;
                    break;
            }
            return visi;
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            LanguagableComponent.LanguageChanged -= WindowLanguageChanged;

            Owner.IsEnabled = true;
            Owner.Activate();
            ActiveBorder.Visibility = Visibility.Hidden;
            ((GooProxyWindow)Owner).MessageSnackBar.IsActive = false;
            base.OnClosed(e);
        }

        private void FinishedClick(object sender, RoutedEventArgs e)
        {
            switch (this._isInput)
            {
                case true:
                    {
                        AddProxyParams[] oldArray = ((GooProxyWindow)Owner).InputProxy;
                        AddProxyParams[] newArray = new AddProxyParams[oldArray.Length + 1];
                        newArray[0] = new AddProxyParams(this._selectedProxy.Guid, this._paramIndex);
                        for (int i = 0; i < oldArray.Length; i++)
                        {
                            newArray[i + 1] = oldArray[i];
                        }
                        ((GooProxyWindow)Owner).InputProxy = newArray;
                    }
                    break;
                case false:
                    {
                        AddProxyParams[] oldArray = ((GooProxyWindow)Owner).OutputProxy;
                        AddProxyParams[] newArray = new AddProxyParams[oldArray.Length + 1];
                        newArray[0] = new AddProxyParams(this._selectedProxy.Guid, this._paramIndex);
                        for (int i = 0; i < oldArray.Length; i++)
                        {
                            newArray[i + 1] = oldArray[i];
                        }
                        ((GooProxyWindow)Owner).OutputProxy = newArray;
                    }
                    break;
            }
            ((GooProxyWindow)Owner).WindowSwitchControl_SelectionChanged(null, null);
            this.Close();
        }

        private void IndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DialogFinishButton.IsEnabled = true;
            SelectedMessage.Text = SelectedParam.Text = null; 
            System.Drawing.Bitmap bitmap;

            SelectedMessage.Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DimGray));

            byte index;
            if(byte.TryParse(IndexTextBox.Text, out index))
            {
                IGH_Param dataType;
                bitmap = CanvasRenderEngine.GetObjectBitmap(_selectedProxy.CreateObejct(), this._isInput, out dataType, index);
                if(bitmap != null)
                {
                    ShowcaseImage.Source = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
                    SelectedParam.Text = $"{dataType.Name} [{dataType.Type.Name}]";
                    if(this._proxy.TypeFullName != dataType.Type.FullName)
                    {
                        SelectedMessage.Text += LanguagableComponent.GetTransLation(new string[] { "Data Type is not the Same!", "数据类型并不一致！" });
                        SelectedMessage.Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DarkOrange));

                    }
                }
                else
                {
                    bitmap = CanvasRenderEngine.GetObjectBitmap(_selectedProxy.CreateObejct(), this._isInput, out _);
                    SelectedMessage.Text = LanguagableComponent.GetTransLation(new string[] { "Input data is out of range!", "输入数据超出索引阈值！" });
                    SelectedMessage.Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DarkRed));
                    DialogFinishButton.IsEnabled = false;
                }
            }
            else
            {
                bitmap = CanvasRenderEngine.GetObjectBitmap(_selectedProxy.CreateObejct(), this._isInput, out _);
                SelectedMessage.Text = LanguagableComponent.GetTransLation(new string[] { "Please input a number between 0 - 255!", "请输入一个0 - 255的数值！" });
                SelectedMessage.Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DarkRed));
                DialogFinishButton.IsEnabled = false;
            }
            ShowcaseImage.Source = CanvasRenderEngine.BitmapToBitmapImage(bitmap);
            this._paramIndex = index;
        }


        private void Datas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(e.AddedItems[0].ToString());
            if (e.AddedItems.Count == 0) return;
            if (e.AddedItems[0] != null)
                _selectedProxy = (ParamGlassesProxy)e.AddedItems[0];
            OKButton.IsEnabled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
