﻿using ArchiTed_Grasshopper;
using ArchiTed_Grasshopper.WPF;
using MaterialDesignThemes.Wpf;
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

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for GooProxyWindow.xaml
    /// </summary>
    public partial class GooProxyWindow : LangWindow
    {
        private GooTypeProxy _proxy { get; }
        private System.Drawing.Color _wireColor;
        private AddProxyParams[] _inputProxy;
        private AddProxyParams[] _outputProxy;

        private Color WireColor
        {
            get
            {
                return ColorExtension.ConvertToMediaColor( _proxy.ShowColor);
            }
            set
            {
                _proxy.ShowColor =System.Drawing.Color.FromArgb(150, value.R, value.G, value.B);
                //System.Windows.Forms.MessageBox.Show(_proxy.ShowColor.ToString());
            }
        }
        internal AddProxyParams[] InputProxy 
        { get 
            {
                try
                {
                    return _paramOwner.CreateProxyDictInput[_proxy.TypeFullName];
                }
                catch
                {
                    return new AddProxyParams[] { };
                }
            } 
            set 
            {
                if(value != null)
                {
                    _paramOwner.CreateProxyDictInput[_proxy.TypeFullName] = value;
                }
            } 
        }
        internal AddProxyParams[] OutputProxy
        {
            get
            {
                try
                {
                    return _paramOwner.CreateProxyDictOutput[_proxy.TypeFullName];
                }
                catch
                {
                    return new AddProxyParams[] { };
                }
            }
            set
            {
                if (value != null)
                {
                    _paramOwner.CreateProxyDictOutput[_proxy.TypeFullName] = value;
                }
            }
        }


        private ParamGlassesComponent _paramOwner { get; }
        public GooProxyWindow(ParamGlassesComponent owner, GooTypeProxy proxy)
            :base()
        {

            this._proxy = proxy;
            this._paramOwner = owner;
            this._wireColor = proxy.ShowColor;
            try
            {
                this._inputProxy = owner.CreateProxyDictInput[proxy.TypeFullName];
            }
            catch { }
            try
            {
                this._outputProxy = owner.CreateProxyDictOutput[proxy.TypeFullName];
            }
            catch { }

            InitializeComponent();

            WindowSwitchControl_SelectionChanged(null, null);
            WindowTitle.Text = proxy.TypeName;

            LanguageChanged();
            LanguagableComponent.LanguageChanged += WindowLanguageChanged;

            AddActiveEvents();

        }
        #region language

        protected override void LanguageChanged()
        {
            #region Top Tree Translate
            ColorMode.Content = LanguagableComponent.GetTransLation(new string[] { "Wire Color", "连线颜色" });
            InputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Input Control", "输入控制项" });
            OutputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Output Control", "输出控制项" });
            #endregion

            #region Major Box
            WindowSwitchControl_SelectionChanged(null, null);
            #endregion

            #region B0ttom Four Translate

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

        #region Set the major box
        internal void WindowSwitchControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MajorGrid == null || ButtonPanel == null) return;
                
            MajorGrid.Children.Clear();
            ButtonPanel.Children.Clear();
            switch (SelectionBox.SelectedIndex)
            {
                case 0:
                    #region Add major
                    ColorPicker color = new ColorPicker() { Color = this.WireColor };
                    MajorGrid.Children.Add(color);
                    #endregion

                    #region Add Button
                    ButtonPanel.Children.Add(CreateARespondIconButton(PackIconKind.Refresh, System.Drawing.Color.DarkRed, 
                        (x, y) => { this.SetWireColor(ColorExtension.ConvertToMediaColor(this._wireColor)); this._proxy.Owner.ExpireSolution(true); },
                        LanguagableComponent.GetTransLation(new string[] { "Reset", "重置" })));

                    ButtonPanel.Children.Add(CreateARespondIconButton(PackIconKind.Refresh, System.Drawing.Color.DimGray,
                        (x, y) => { this.SetWireColor(color); this._proxy.Owner.ExpireSolution(true); },
                        LanguagableComponent.GetTransLation(new string[] { "Refresh", "刷新" })));

                    SelectionBox.SelectionChanged += (x, y) => { this.SetWireColor(color); };
                    OKButton.Click += (x, y) => { this.SetWireColor(color); };
                    #endregion
                    break;
                case 1:
                    #region Add major
                    MajorGrid.Children.Add(CreateDataGrid(true));
                    #endregion

                    #region Add Button
                    IOControlButton(true);
                    #endregion
                    break;
                case 2:
                    #region Add major
                    MajorGrid.Children.Add(CreateDataGrid(false));
                    #endregion

                    #region Add Button
                    IOControlButton(false);
                    #endregion
                    break;
                default:
                    throw new ArgumentOutOfRangeException("selectedindex", "index is out of range.");
            }
        }

        private void IOControlButton(bool isInput)
        {

            ButtonPanel.Children.Add(CreateARespondIconButton(PackIconKind.Plus, System.Drawing.Color.LightSeaGreen,
                (x, y) => { new SelectOneParamWindow(_paramOwner, isInput, _proxy) { Owner = this}.Show();
                    ActiveBorder.Visibility = Visibility.Visible;
                    this.IsEnabled = false;
                    MessageBox.Content = LanguagableComponent.GetTransLation(new string[] { "Please finish the child window first.", "请先完成子窗口。" });
                    MessageSnackBar.IsActive = true;
                },
                LanguagableComponent.GetTransLation(new string[] { "Add", "添加" })));

            ButtonPanel.Children.Add(CreateARespondIconButton(PackIconKind.Minus, System.Drawing.Color.DarkRed,
                (x, y) =>
                {
                    if (this.MajorGrid.Children.Count == 0)
                        return;
                    if (!(this.MajorGrid.Children[0] is DataGrid))
                        return;
                    DataGrid grid = this.MajorGrid.Children[0] as DataGrid;
                    List<AddProxyParams> restProxies = new List<AddProxyParams>();
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        DataGridRow ObjRow = (DataGridRow)(grid.ItemContainerGenerator.ContainerFromIndex(i));
                        if (ObjRow != null)
                        {
                            if (!ObjRow.IsSelected)
                            {
                                restProxies.Add(isInput ? InputProxy[i] : OutputProxy[i]);
                            }
                        }
                    }

                    if (isInput)
                    {
                        InputProxy = restProxies.ToArray();
                        grid.ItemsSource = InputProxy;
                    }
                    else
                    {
                        OutputProxy = restProxies.ToArray();
                        grid.ItemsSource = OutputProxy;
                    }

                },
                LanguagableComponent.GetTransLation(new string[] { "Remove", "删除" })));
        }

        private Button CreateARespondIconButton(PackIconKind icon, System.Drawing.Color forecolor, RoutedEventHandler clickRespond = null, string tooltip = null)
        {
            Button button = CreateAIconButton(icon, forecolor, tooltip);
            if (clickRespond != null)
                button.Click += clickRespond;

            return button;
        }

        private Button CreateAIconButton(PackIconKind icon, System.Drawing.Color forecolor, string tooltip = null)
        {
            Button button = new Button()
            {
                Margin = new Thickness(4, 0, 4, 0),
                Content = new PackIcon()
                {
                    Kind = icon,
                    Height = 24,
                    Width = 24,
                    Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(forecolor))
                },
                ToolTip = tooltip,
            };
            button.SetValue(Button.StyleProperty, this.Resources["MaterialDesignFloatingActionMiniAccentButton"]);
            return button;
        }

        private DataGrid CreateDataGrid(bool isInput)
        {
            DataGrid dataGrid = new DataGrid()
            {
                SelectionUnit = DataGridSelectionUnit.FullRow,
                CanUserSortColumns = true,
                SelectionMode = DataGridSelectionMode.Extended,
                AutoGenerateColumns = false,
            };
            DataGridAssist.SetCellPadding(dataGrid, new Thickness(4, 2, 2, 2));

            #region Add a iconColumn
            DataGridTemplateColumn iconColumn = new DataGridTemplateColumn()
            {
                Header = LanguagableComponent.GetTransLation(new string[] { "Icon", "图标" }),
                MinWidth = 24,
                CanUserSort = false,
            };
            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(Image));
            spFactory.SetBinding(Image.SourceProperty, new Binding("ShowIcon"));
            spFactory.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            spFactory.SetValue(Image.MaxHeightProperty, 24.0);
            spFactory.SetValue(Image.MaxWidthProperty, 24.0);
            iconColumn.CellTemplate = new DataTemplate()
            {
                DataType = typeof(AddProxyParams),
                VisualTree = spFactory,
            };
            dataGrid.Columns.Add(iconColumn);
            #endregion

            #region Add other column
            dataGrid.Columns.Add(GetATextColumn("Name", LanguagableComponent.GetTransLation(new string[] { "Name", "名称" })));
            string indexHeader = isInput ? LanguagableComponent.GetTransLation(new string[] { "Input Index", "输入端序号" }) :
                LanguagableComponent.GetTransLation(new string[] { "Output Index", "输出端序号" });
            dataGrid.Columns.Add(GetATextColumn("Index", indexHeader));
            dataGrid.Columns.Add(GetATextColumn("Category", LanguagableComponent.GetTransLation(new string[] { "Category", "类别" })));
            dataGrid.Columns.Add(GetATextColumn("Subcategory", LanguagableComponent.GetTransLation(new string[] { "Subcategory", "子类别" })));
            dataGrid.Columns.Add(GetATextColumn("Exposure", LanguagableComponent.GetTransLation(new string[] { "Exposure", "分栏" })));
            dataGrid.Columns.Add(GetATextColumn("Guid", LanguagableComponent.GetTransLation(new string[] { "Guid", "全局唯一标识符（Guid）" })));
            #endregion

            dataGrid.ItemsSource = isInput ? InputProxy : OutputProxy;
            return dataGrid;
        }

        private System.Windows.Controls.DataGridTextColumn GetATextColumn(string path, string header)
        {
            System.Windows.Controls.DataGridTextColumn column = new System.Windows.Controls.DataGridTextColumn()
            {
                Binding = new Binding(path),
                Header = header ,
                IsReadOnly = true,
            };
            column.SetValue(System.Windows.Controls.DataGridTextColumn.ElementStyleProperty, this.Resources["MaterialDesignDataGridTextColumnStyle"]);
            column.SetValue(System.Windows.Controls.DataGridTextColumn.EditingElementStyleProperty, this.Resources["MaterialDesignDataGridTextColumnEditingStyle"]);
            return column;
        }
        #endregion

        #endregion

        #region Middle Respond
        private void SetWireColor(ColorPicker colorPicker)
        {
            SetWireColor(colorPicker.Color);
        }

        private void SetWireColor(Color color)
        {
            if (this.WireColor == color) return;

            this._proxy.Owner.RecordUndoEvent("Define the wire color");
            this.WireColor = color;
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            MessageSnackBar.IsActive = false;
        }
        #endregion

        #region Bottom four respond

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this._proxy.ShowColor = this._wireColor;
            if (this._inputProxy != null)
                _paramOwner.CreateProxyDictInput[this._proxy.TypeFullName] = this._inputProxy;
            if (this._outputProxy != null)
                _paramOwner.CreateProxyDictOutput[this._proxy.TypeFullName] = this._outputProxy;
            _paramOwner.ExpireSolution(true);
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ((ParamSettingsWindow)this.Owner).UpdateProxy();
            _paramOwner.ExpireSolution(true);
            Owner.Activate();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            LanguagableComponent.LanguageChanged -= WindowLanguageChanged;
            base.OnClosed(e);
        }

        #endregion

        #region Others
        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Visible;
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            ActiveBorder.Visibility = Visibility.Hidden;
        }

        public void AddActiveEvents()
        {
            this.Deactivated += GooProxyWindow_Deactivated;
            this.Activated += GooProxyWindow_Activated;
        }

        private void GooProxyWindow_Activated(object sender, EventArgs e)
        {
            ActiveBorder.Visibility = Visibility.Hidden;
        }

        private void GooProxyWindow_Deactivated(object sender, EventArgs e)
        {
            ActiveBorder.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
