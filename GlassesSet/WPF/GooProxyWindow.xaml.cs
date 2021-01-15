using ArchiTed_Grasshopper;
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
        private AddProxyParams[] InputProxy 
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
                if(value != null && value.Length != 0)
                {
                    _paramOwner.CreateProxyDictInput[_proxy.TypeFullName] = value;
                }
            } 
        }
        private AddProxyParams[] OutputProxy
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
                if (value != null && value.Length != 0)
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
            ColorMode.Content = LanguagableComponent.GetTransLation(new string[] { "Wire Color", "连线颜色" });
            InputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Input Control", "输入控制项" });
            OutputMode.Content = LanguagableComponent.GetTransLation(new string[] { "Output Control", "输出控制项" });
            #endregion

            #region Major Box
            //DataGridPropertyChange();
            //IconColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Icon", "图标" });
            //FullNameColumn.Header = LanguagableComponent.GetTransLation(new string[] { "FullName", "全名" });
            //CategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Category", "类别" });
            //SubcategoryColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Subcategory", "子类别" });
            //ExposureColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Exposure", "分栏" });
            //GuidColumn.Header = LanguagableComponent.GetTransLation(new string[] { "Guid", "全局唯一标识符（Guid）" });

            //FirstExpenderName.Text = LanguagableComponent.GetTransLation(new string[] { "Filter", "过滤器" });
            //SecondExpenderName.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Column Visibility", "列可视性" });
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
        private void WindowSwitchControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MajorGrid == null || ButtonPanel == null) return;
                
            MajorGrid.Children.Clear();
            ButtonPanel.Children.Clear();
            switch (SelectionBox.SelectedIndex)
            {
                case 0:
                    ColorPicker color = new ColorPicker() { Color = this.WireColor };
                    MajorGrid.Children.Add(color);

                    Button resetButton = new Button() 
                    {
                        Margin = new Thickness(4, 0, 4, 0),
                        Content = new PackIcon()
                        {
                            Kind = PackIconKind.Refresh,
                            Height = 24,
                            Width = 24,
                            Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DarkRed))
                        },
                        ToolTip = LanguagableComponent.GetTransLation(new string[] { "Reset", "重置"}),
                    };
                    resetButton.SetValue(Button.StyleProperty, this.Resources["MaterialDesignFloatingActionMiniAccentButton"]);
                    resetButton.Click += (x, y) => { this.SetWireColor(ColorExtension.ConvertToMediaColor(this._wireColor)); this._proxy.Owner.ExpireSolution(true); };

                    Button freshButton = new Button() 
                    {
                        Margin = new Thickness(4, 0, 4, 0),
                        Content = new PackIcon()
                        {
                            Kind = PackIconKind.Refresh,
                            Height = 24,
                            Width = 24,
                            Foreground = new SolidColorBrush(ColorExtension.ConvertToMediaColor(System.Drawing.Color.DimGray))
                        },
                        ToolTip = LanguagableComponent.GetTransLation(new string[] { "Refresh", "刷新" }),
                    };
                    freshButton.SetValue(Button.StyleProperty, this.Resources["MaterialDesignFloatingActionMiniAccentButton"]);

                    //Set the respond for color changed.
                    freshButton.Click += (x, y) => { this.SetWireColor(color); this._proxy.Owner.ExpireSolution(true); };
                    SelectionBox.SelectionChanged += (x, y) => { this.SetWireColor(color); };
                    OKButton.Click += (x, y) => { this.SetWireColor(color); };

                    ButtonPanel.Children.Add(resetButton);
                    ButtonPanel.Children.Add(freshButton);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("selectedindex", "index is out of range.");
            }
            //DataGridPropertyChange();
            //DrawDataTree(GetRightStateProxy(Owner.AllProxy));
        }

        private void SetWireColor(ColorPicker colorPicker)
        {
            SetWireColor(colorPicker.Color);
        }

        private void SetWireColor(Color color)
        {
            this._proxy.Owner.RecordUndoEvent("Define the wire color");
            this.WireColor = color;
        }
        #endregion

        #region Middle Respond
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

        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }
    }
}
