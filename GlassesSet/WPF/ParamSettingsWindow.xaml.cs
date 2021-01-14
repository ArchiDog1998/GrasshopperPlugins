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
using MaterialDesignThemes.Wpf;

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

        private IOType _ioType;

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

            LanguageChanged();
            LanguagableComponent.LanguageChanged += WindowLanguageChanged;

            this.Deactivated += (x, y) =>
            {
                ActiveBorder.Visibility = Visibility.Visible;
                //this.Opacity = 0.6;
            };
            this.Activated += (x, y) =>
            {
                ActiveBorder.Visibility = Visibility.Hidden;
                //this.Opacity = 1;
            };
        }

        #region Language
        protected override void LanguageChanged()
        {
            #region Top Tree Translate
            WindowTitle.Text = LanguagableComponent.GetTransLation(new string[] { "Param Settings Window", "参数设定窗口" });
            FilterButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to add the components selected on the canvas to list.", "点击以将在画布中选中的运算器添加到列表当中" });

            HintAssist.SetHint(SearchBox, new Label()
            {
                Content = LanguagableComponent.GetTransLation(new string[] { "Search", "搜索" }),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            #endregion

            #region Major Box
            EditColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Edit", "编辑" });
            TypeNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Type Name", "类型名称" });
            TypeFullNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Type FullName", "类型全名" });
            AssemNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Assembly Name", "类库名称" });
            AssemIdColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Assembly Guid", "类库全局唯一标识符（Guid）" });

            FirstExpenderName.Text = LanguagableComponent.GetTransLation(new string[] { "Filter", "过滤器" });
            SecondExpenderName.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Column Visibility", "列可视性" });
            #endregion

            #region B0ttom Four Translate
            FileOption.ToolTip = LanguagableComponent.GetTransLation(new string[] { "File Option", "文件选项" });
            AsDefaultButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to save as template.", "点击以保存一个模板。" });
            ImportButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to import a file.", "点击以导入一个文件。" });
            ExportButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to export a file.", "点击以导出一个文件。" });

            CancelButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to cancel the changes and close this window.", "单击以取消修改，并关闭此窗口。" });
            OKButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to comfirm the change and close the window.", "单击以确认修改并关闭窗口。" });
            #endregion

        }
        #endregion

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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            GooTypeProxy aimProxy = (GooTypeProxy)((Button)sender).Tag;
            System.Windows.Forms.MessageBox.Show(aimProxy.TypeName);
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
            this._ioType = IOType.Default;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            this._ioType = IOType.Import;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            this._ioType = IOType.Export;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.CreateProxyDictInput = this._createProxyDictInput;
            Owner.CreateProxyDictOutput = this._createProxyDictOutput;
            Owner.ColorDict = this._colorDict;
            Owner.ExpireSolution(true);
            this.Close();
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



        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Visible;
            HintAssist.SetHint(DialogSelect, new Label()
            {
                Content = LanguagableComponent.GetTransLation(new string[] { "Select Mode", "选择模式" }),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            ColorMode.Content = LanguagableComponent.GetTransLation(new string[] { "Wire Color", "连线颜色" });
            InputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Input Control", "输入控制项" });
            OutputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Output Control", "输出控制项" });
            DialogAccept.Content = LanguagableComponent.GetTransLation(new string[] { "ACCEPT", "接受" });
        }

        private void SaveIO_Click(object sender, RoutedEventArgs e)
        {
            switch (this._ioType)
            {
                case IOType.Default:
                    switch(DialogSelect.SelectedIndex)
                    {
                        case 0:
                            MessageBox.Content = Owner.WriteColorTxt();
                            MessageSnackBar.IsActive = true;
                            break;
                        case 1:
                            MessageBox.Content = Owner.WriteInputTxt();
                            MessageSnackBar.IsActive = true;
                            break;
                        case 2:
                            MessageBox.Content = Owner.WriteOutputTxt();
                            MessageSnackBar.IsActive = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("SelectedIndex", "SelectedIndex is invalid!");
                    }
                    break;
                case IOType.Import:
                    switch (DialogSelect.SelectedIndex)
                    {
                        case 0:
                            IO_Helper.ImportOpenFileDialog((path) =>
                            {
                                string result = Owner.ReadColorTxt(path);
                                //Owner.UpdateAllProxy();
                                SetShowProxy(Owner.AllParamProxy);

                                MessageBox.Content = result;
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        case 1:
                            IO_Helper.ImportOpenFileDialog((path) =>
                            {
                                string result = Owner.ReadInputTxt(path);
                                //Owner.UpdateAllProxy();
                                //SetShowProxy(Owner.AllParamProxy);

                                MessageBox.Content = result;
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        case 2:
                            IO_Helper.ImportOpenFileDialog((path) =>
                            {
                                string result = Owner.ReadOutputTxt(path);
                                //Owner.UpdateAllProxy();
                                //SetShowProxy(Owner.AllParamProxy);

                                MessageBox.Content = result;
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("SelectedIndex", "SelectedIndex is invalid!");
                    }
                    break;
                case IOType.Export:
                    switch (DialogSelect.SelectedIndex)
                    {
                        case 0:
                            IO_Helper.ExportSaveFileDialog("WireColors_Default", (path) =>
                            {
                                MessageBox.Content = Owner.WriteColorTxt(path);
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        case 1:
                            IO_Helper.ExportSaveFileDialog("InputControl_Default", (path) =>
                            {
                                MessageBox.Content = Owner.WriteInputTxt(path);
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        case 2:
                            IO_Helper.ExportSaveFileDialog("OutputControl_Default", (path) =>
                            {
                                MessageBox.Content = Owner.WriteOutputTxt(path);
                                MessageSnackBar.IsActive = true;
                            });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("SelectedIndex", "SelectedIndex is invalid!");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("_ioType", "ioType is invalid!");
            }
        }

        private void DialogHost_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Hidden;
        }

        private void DialogSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DialogAccept.IsEnabled = true;
        }
    }
}
