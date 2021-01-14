using ArchiTed_Grasshopper.WPF;
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
using InfoGlasses;
using MaterialDesignThemes.Wpf;

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : LangWindow
    {

        private List<Guid> _normalExceptions = new List<Guid>();
        private List<Guid> _pluginExceptions = new List<Guid>();
        private new InfoGlassesComponent Owner { get; }
        public ExceptionWindow(InfoGlassesComponent owner)
            : base()
        {
            this.DataContext = this;

            _normalExceptions = new List<Guid>();
            foreach (Guid item in owner.normalExceptionGuid)
            {
                _normalExceptions.Add(item);
            }
            _pluginExceptions = new List<Guid>();
            foreach (Guid item1 in owner.pluginExceptionGuid)
            {
                _pluginExceptions.Add(item1);
            }
            this.Owner = owner;

            this.InitializeComponent();

            SetShowProxy(owner.AllProxy);
            DrawDataTree(owner.AllProxy);

            this.Datas.LoadingRow += Datas_LoadingRow;

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



        #region language

        protected override void LanguageChanged()
        {
            #region Top Tree Translate
            WindowTitle.Text = LanguagableComponent.GetTransLation(new string[] { "Exception Window", "除去项窗口" });
            WindowSwitchControl.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to switch between normal showcase and plugin showcase.", "点击以在常规展示和插件展示中切换。" });
            NormalMode.Content = LanguagableComponent.GetTransLation(new string[] { "Normal", "常规项" });
            PluginMode.Content = LanguagableComponent.GetTransLation(new string[] { "Plugin", "插件项" });
            FilterButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to add the components selected on the canvas to list.", "点击以将在画布中选中的运算器添加到列表当中" });

            HintAssist.SetHint(SearchBox, new Label() { Content = LanguagableComponent.GetTransLation(new string[] { "Search", "搜索" }),
                HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            #endregion

            #region Major Box
            DataGridPropertyChange();
            IconColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Icon", "图标" });
            FullNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "FullName", "全名" });
            CategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Category", "类别" });
            SubcategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Subcategory", "子类别" });
            ExposureColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Exposure", "分栏" });
            GuidColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Guid", "全局唯一标识符（Guid）" });

            FirstExpenderName.Text = LanguagableComponent.GetTransLation(new string[] { "Filter", "过滤器" });
            SecondExpenderName.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Column Visibility", "列可视性" });
            #endregion

            #region B0ttom Four Translate
            FileOption.ToolTip = LanguagableComponent.GetTransLation(new string[] { "File Option", "文件选项" });
            AsDefaultButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to save as template.", "点击以保存一个模板。" });
            ImportButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to import a file.", "点击以导入一个文件。" });
            ExportButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to export a file.", "点击以导出一个文件。" });

            CancelButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to cancel the changes and close this window.", "单击以取消修改，并关闭此窗口。" });
            RefreshButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to refresh the component.", "单击以刷新运算器。" });
            OKButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to comfirm the change and close the window.", "单击以确认修改并关闭窗口。" });
            #endregion

        }
        private void DataGridPropertyChange()
        {
            if (DataGridCheckBox != null)
            {
                bool IsNormal = WindowSwitchControl.SelectedIndex == 0;
                ((Label)((StackPanel)DataGridCheckBox.Header).Children[1]).Content = IsNormal ? LanguagableComponent.GetTransLation(new string[] { "Normal", "常规项" }) : LanguagableComponent.GetTransLation(new string[] { "Plugin", "插件项" });
                DataGridCheckBox.Binding = new Binding(IsNormal ? "IsExceptNormal" : "IsExceptPlugin");
            }
        }
        #endregion

        #region Top Respond
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = null;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            var q = Search(SearchBox.Text, Owner.AllProxy.ToArray());
            if (q.Count > 0)
            {
                List<ExceptionProxy> proxies = new List<ExceptionProxy>();
                foreach (var item in q)
                {
                    proxies.Add(item as ExceptionProxy);
                }
                SetShowProxy(proxies);
            }

        }
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                FindButton_Click(null, null);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            List<ExceptionProxy> showProxies = new List<ExceptionProxy>();
            foreach (var docObj in Grasshopper.Instances.ActiveCanvas.Document.SelectedObjects())
            {
                foreach (var proxy in Owner.AllProxy)
                {
                    if (docObj.ComponentGuid == proxy.Guid)
                    {
                        if (!showProxies.Contains(proxy))
                            showProxies.Add(proxy);
                        break;
                    }
                }
            }
            SetShowProxy(showProxies);
        }
        private void WindowSwitchControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGridPropertyChange();
            DrawDataTree(GetRightStateProxy(Owner.AllProxy));
        }
        #endregion

        #region Middle Respond
        private void SetShowProxy(List<ExceptionProxy> proxies, string Category = null, string Subcategory = null)
        {
            if (Category != null)
                proxies = (List<ExceptionProxy>)(proxies.Where((x) => x.Category == Category).ToList());
            if (Subcategory != null)
                proxies = (List<ExceptionProxy>)(proxies.Where((x) => x.Subcategory == Subcategory).ToList());

            this.Datas.ItemsSource = GetRightStateProxy(proxies);
        }

        private List<ExceptionProxy> GetRightStateProxy(List<ExceptionProxy> proxies)
        {
            if (WindowSwitchControl.SelectedIndex != 0)
                return (List<ExceptionProxy>)(proxies.Where((x) => x.IsPlugin).ToList());
            else
                return proxies;

        }

        private void DrawDataTree(List<ExceptionProxy> proxies)
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
            SetShowProxy(Owner.AllProxy, category, subcate);

        }



        private void CategoryClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetShowProxy(Owner.AllProxy, ((Label)((StackPanel)sender).Children[1]).Content as string);
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

        /// <summary>
        /// ModeChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(Datas.Items[0].ToString());
            for (int i = 0; i < Datas.Items.Count; i++)
            {
                DataGridRow ObjRow = (DataGridRow)(Datas.ItemContainerGenerator.ContainerFromIndex(i));
                if (ObjRow != null)
                {
                    FrameworkElement objElement = Datas.Columns[0].GetCellContent(ObjRow);
                    if (objElement != null)
                    {
                        CheckBox checkBox = objElement as CheckBox;
                        checkBox.IsChecked = AllButton.IsChecked.Value;
                    }
                    //else
                    //{
                    //    System.Windows.Forms.MessageBox.Show(LanguagableComponent.GetTransLation(new string[] { "Failed to set all button", "设定所有按钮失败" }));
                    //}
                }
                //else
                //{
                //    System.Windows.Forms.MessageBox.Show(LanguagableComponent.GetTransLation(new string[] { "Failed to set all button", "设定所有按钮失败" }));
                //}
            }
        }

        private void Datas_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            int count = Datas.Items.Count;
            for (int i = 0; i < Datas.Items.Count; i++)
            {
                DataGridRow ObjRow = (DataGridRow)(Datas.ItemContainerGenerator.ContainerFromIndex(i));
                if (ObjRow != null)
                {
                    FrameworkElement objElement = Datas.Columns[0].GetCellContent(ObjRow);
                    if (objElement != null)
                    {
                        CheckBox checkBox = objElement as CheckBox;
                        checkBox.Click -= checkAllButton_Click;
                        checkBox.Click += checkAllButton_Click;
                    }

                }
            }

            List<bool> allBool = new List<bool>();
            foreach (var item in Datas.ItemsSource)
            {
                ExceptionProxy proxy = item as ExceptionProxy;
                allBool.Add(WindowSwitchControl.SelectedIndex == 0 ? proxy.IsExceptNormal : proxy.IsExceptPlugin);
            }
            CheckAllButton(allBool);
        }


        private void checkAllButton_Click(object sender, RoutedEventArgs e)
        {

            List<bool> allBool = new List<bool>();
            for (int i = 0; i < Datas.Items.Count; i++)
            {
                DataGridRow ObjRow = (DataGridRow)(Datas.ItemContainerGenerator.ContainerFromIndex(i));
                if (ObjRow != null)
                {
                    FrameworkElement objElement = Datas.Columns[0].GetCellContent(ObjRow);
                    if (objElement != null)
                    {
                        CheckBox checkBox = objElement as CheckBox;
                        allBool.Add(checkBox.IsChecked.Value);
                    }
                }
            }
            //foreach (var item in Owner.AllProxy)
            //{
            //    allBool.Add(WindowSwitchControl.SelectedIndex == 0 ? item.IsExceptNormal : item.IsExceptPlugin);
            //}
            CheckAllButton(allBool);
        }

        private void CheckAllButton(List<bool> allBool)
        {
            bool isChecked = true;
            bool isUnChecked = true;
            foreach (var item in allBool)
            {
                if (!isChecked && !isUnChecked)
                {
                    AllButton.IsChecked = null;
                    return;
                }

                switch (item)
                {
                    case false:
                        isChecked = false;
                        break;
                    case true:
                        isUnChecked = false;
                        break;
                }
            }

            if (isChecked)
            {
                AllButton.IsChecked = true;
                return;
            }

            else if (isUnChecked)
            {
                AllButton.IsChecked = false;
                return;
            }


            System.Windows.Forms.MessageBox.Show("Something wrong with allbutton check!");
        }
        #endregion
        #endregion

        #region Bottom four respond
        private void AsDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Content = Owner.Writetxt();
            MessageSnackBar.IsActive = true;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = LanguagableComponent.GetTransLation( new string[] { "Select a template", "选择一个模板"});


            openFileDialog.Filter = "*.txt|*.txt";


            openFileDialog.FileName = string.Empty;


            openFileDialog.Multiselect = false;


            openFileDialog.RestoreDirectory = true;


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                string result = Owner.Readtxt(path);
                Owner.UpdateAllProxy();
                SetShowProxy(Owner.AllProxy);

                MessageBox.Content = result;
                MessageSnackBar.IsActive = true;
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
                MessageBox.Content = Owner.Writetxt(path);
                MessageSnackBar.IsActive = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.normalExceptionGuid = this._normalExceptions;
            Owner.pluginExceptionGuid = this._pluginExceptions;
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

        private void WindowTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            LanguagableComponent.LanguageChanged -= WindowLanguageChanged;
            base.OnClosed(e);
        }

        #endregion


    }

}
