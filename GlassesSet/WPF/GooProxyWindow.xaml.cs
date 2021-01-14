using ArchiTed_Grasshopper;
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

namespace InfoGlasses.WPF
{
    /// <summary>
    /// Interaction logic for GooProxyWindow.xaml
    /// </summary>
    public partial class GooProxyWindow : LangWindow
    {
        private GooTypeProxy Proxy { get; } 
        private new ParamGlassesComponent Owner { get; }
        public GooProxyWindow(ParamGlassesComponent owner, GooTypeProxy proxy)
            :base()
        {
            this.Proxy = proxy;
            this.Owner = owner;
            InitializeComponent();

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
            RefreshButton.ToolTip = LanguagableComponent.GetTransLation(new string[] { "Click to refresh the component.", "单击以刷新运算器。" });
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

            //DataGridPropertyChange();
            //DrawDataTree(GetRightStateProxy(Owner.AllProxy));
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
            //Owner.normalExceptionGuid = this._normalExceptions;
            //Owner.pluginExceptionGuid = this._pluginExceptions;
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

        private void DialogHost_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {

        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {

        }
    }
}
