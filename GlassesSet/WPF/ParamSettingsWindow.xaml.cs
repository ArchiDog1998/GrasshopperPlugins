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
using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WPF;
using Grasshopper.Kernel;

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for WireColorsWindow.xaml
    /// </summary>
    public partial class ParamSettingsWindow : LangWindow
    {
        private Dictionary<string, AddProxyParams[]> _createProxyDictInput = new Dictionary<string, AddProxyParams[]>();

        private Dictionary<string, AddProxyParams[]> _createProxyDictOutput = new Dictionary<string, AddProxyParams[]>();

        private Dictionary<string, System.Drawing.Color> _colorDict = new Dictionary<string, System.Drawing.Color>();

        private new ParamGlassesComponent Owner { get; }

        public ParamSettingsWindow(ParamGlassesComponent owner)
            :base()
        {
            this.DataContext = this;
            this.Owner = owner;

            foreach (var item in owner.CreateProxyDictInput)
            {
                _createProxyDictInput[item.Key] = item.Value;
            }
            foreach (var item in owner.CreateProxyDictOutput)
            {
                _createProxyDictOutput[item.Key] = item.Value;
            }
            foreach (var item in owner.ColorDict)
            {
                _colorDict[item.Key] = item.Value;
            }
            InitializeComponent();

            SetShowProxy(owner.AllParamProxy);
            DrawDataTree(owner.AllParamProxy);
        }

        #region Top Respond
        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = null;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            var q = Search(SearchBox.Text, Owner.AllParamProxy.ToArray());
            if (q.Count > 0)
            {
                List<GooTypeProxy> proxies = new List<GooTypeProxy>();
                foreach (var item in q)
                {
                    proxies.Add(item as GooTypeProxy);
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

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            List<GooTypeProxy> showProxies = new List<GooTypeProxy>();
            foreach (var docObj in Grasshopper.Instances.ActiveCanvas.Document.SelectedObjects())
            {
                if (!(docObj is IGH_Param)) continue;
                Type persistType;
                if(Owner.IsPersistentParam(docObj.GetType(), out persistType))
                {
                    foreach (var proxy in Owner.AllParamProxy)
                    {
                        if (persistType == proxy.DataType)
                        {
                            if (!showProxies.Contains(proxy))
                                showProxies.Add(proxy);
                            break;
                        }
                    }
                }

            }
            SetShowProxy(showProxies);
        }
        private void WindowSwitchControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGridPropertyChange();
            //DrawDataTree(GetRightStateProxy(Owner.AllProxy));
        }
        #endregion

        #region Middle Respond
        private void SetShowProxy(List<GooTypeProxy> proxies, string AssemName = null)
        {
            if (AssemName != null)
                proxies = (List<GooTypeProxy>)(proxies.Where((x) => x.AssemName == AssemName).ToList());

            this.Datas.ItemsSource = proxies;
        }

        private void DrawDataTree(List<GooTypeProxy> proxies)
        {
            if (LeftCateTree == null)
                return;

            var result = proxies.GroupBy((x) => x.Assembly).ToList();
            result.Sort((x, y) => x.Key.Name.CompareTo(y.Key.Name));

            foreach (var categoryItem in result)
            {
                TreeViewItem viewItem = new TreeViewItem();

                #region Dim the CategoryName
                StackPanel panel = new StackPanel();
                panel.MouseDown += AssemblyClick;
                panel.Orientation = Orientation.Horizontal;
                try
                {
                    panel.Children.Add(new Image()
                    {
                        Source = CanvasRenderEngine.BitmapToBitmapImage(
                            new System.Drawing.Bitmap(categoryItem.Key.Icon, 20, 20)),
                    });
                }
                catch { }

                panel.Children.Add(new Label()
                {
                    Content = categoryItem.Key.Name,
                    FontSize = 15,
                    Margin = new Thickness(5, 0, 0, 0),
                });
                viewItem.Header = panel;
                #endregion

                LeftCateTree.Items.Add(viewItem);
            }
        }

        private void AssemblyClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetShowProxy(Owner.AllParamProxy, ((Label)((StackPanel)sender).Children[1]).Content as string);
            }
        }

        private void CategoryCheckbox_Click(object sender, RoutedEventArgs e)
        {
            TypeFullNameColumn.Visibility = GetVisibility((CheckBox)sender);
        }

        private void SubcateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            AssemNameColumn.Visibility = GetVisibility((CheckBox)sender);
        }

        private void GuidCheckBox_Click(object sender, RoutedEventArgs e)
        {
            AssemIdColumn.Visibility = GetVisibility((CheckBox)sender);
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

        /// <summary>
        /// Message OK Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }


        #region Allbutton Events

        private void Datas_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            //int count = Datas.Items.Count;
            for (int i = 0; i < Datas.Items.Count; i++)
            {
                DataGridRow ObjRow = (DataGridRow)(Datas.ItemContainerGenerator.ContainerFromIndex(i));
                if (ObjRow != null)
                {
                    FrameworkElement objElement = Datas.Columns[0].GetCellContent(ObjRow);
                    if (objElement != null)
                    {
                        CheckBox checkBox = objElement as CheckBox;
                        checkBox.Click -= selcetion_Click;
                        checkBox.Click += selcetion_Click;
                        //checkBox.Click += checkAllButton_Click;
                    }

                }
            }

            List<bool> allBool = new List<bool>();
            foreach (var item in Datas.ItemsSource)
            {
                ExceptionProxy proxy = item as ExceptionProxy;
            }
            //CheckAllButton(allBool);
        }

        private void selcetion_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Datas.Items.Count; i++)
            {
                DataGridRow ObjRow = (DataGridRow)(Datas.ItemContainerGenerator.ContainerFromIndex(i));
                if (ObjRow != null)
                {
                    if (!ObjRow.IsSelected) continue;

                    FrameworkElement objElement = Datas.Columns[0].GetCellContent(ObjRow);
                    if (objElement != null)
                    {
                        CheckBox checkBox = objElement as CheckBox;
                        checkBox.IsChecked = ((CheckBox)sender).IsChecked;
                    }

                }
            }
        }

        #endregion
        #endregion

        #region Bottom four respond
        private void AsDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Content = Owner.Writetxt();
            //MessageSnackBar.IsActive = true;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = LanguagableComponent.GetTransLation(new string[] { "Select a template", "选择一个模板" });


            openFileDialog.Filter = "*.txt|*.txt";


            openFileDialog.FileName = string.Empty;


            openFileDialog.Multiselect = false;


            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                //string result = Owner.Readtxt(path);
                //Owner.UpdateAllProxy();
                //SetShowProxy(Owner.AllProxy);

                //MessageBox.Content = result;
                //MessageSnackBar.IsActive = true;
            }


        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            saveFileDialog.Title = LanguagableComponent.GetTransLation(new string[] { "Set a Paht", "设定一个路径" });
            saveFileDialog.Filter = "*.txt|*.txt";
            saveFileDialog.FileName = "Infoglasses_Default";
            saveFileDialog.SupportMultiDottedExtensions = false;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = saveFileDialog.FileName;
                //MessageBox.Content = Owner.Writetxt(path);
                //MessageSnackBar.IsActive = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.CreateProxyDictInput = this._createProxyDictInput;
            Owner.CreateProxyDictOutput = this._createProxyDictOutput;
            Owner.ColorDict = this._colorDict;
            Owner.ExpireSolution(true);
            this.Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.ExpireSolution(true);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.ExpireSolution(true);
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            LanguagableComponent.LanguageChanged -= WindowLanguageChanged;
            base.OnClosed(e);
        }

        #endregion

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {

        }
    }
}
