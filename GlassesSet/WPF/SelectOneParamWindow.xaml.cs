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

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for SelectOneParamWindow.xaml
    /// </summary>
    public partial class SelectOneParamWindow : LangWindow
    {
        private ParamGlassesComponent _paramOwner { get; }
        private bool _isInput { get; }
        private ParamGlassesProxy _selectedProxy;
        public SelectOneParamWindow(ParamGlassesComponent owner, bool isInput)
            :base()
        {
            this._isInput = isInput;
            this._paramOwner = owner;
            InitializeComponent();

            SetShowProxy(_paramOwner.AllProxy);
            DrawDataTree(_paramOwner.AllProxy);
        }

        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Visible;
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
            Owner.IsEnabled = true;
            ((GooProxyWindow)Owner).MessageSnackBar.IsActive = false;
            base.OnClosed(e);
        }

        private void FinishedClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void IndexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void Datas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(e.AddedItems[0].ToString());
            if (e.AddedItems[0] != null)
                _selectedProxy = (ParamGlassesProxy)e.AddedItems[0];
            OKButton.IsEnabled = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            IGH_DocumentObject obj = _selectedProxy.CreateObejct();
            GH_Canvas canvas = new GH_Canvas();

            ParamControlHelper.AddAObjectToCanvas(obj, new System.Drawing.PointF(), false, canvas);
            GH_Viewport vp = new GH_Viewport();
            vp.Focus(obj.Attributes);
            System.Drawing.Bitmap bitmap = canvas.GenerateHiResImageTile(vp, System.Drawing.Color.Transparent);


            ShowcaseImage.Source = CanvasRenderEngine.BitmapToBitmapImage( bitmap);
            canvas.Dispose();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
