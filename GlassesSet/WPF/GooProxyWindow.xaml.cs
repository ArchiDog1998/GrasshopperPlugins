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
        }
    }
}
