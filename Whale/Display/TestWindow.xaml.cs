using Rhino.Display;
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

namespace Whale.Display
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
            //IntPtr flpHandle = flowLayoutPanel1.Handle;//仅限主界面代码段


            //RhinoView rhinoView = Rhino.RhinoDoc.ActiveDoc.Views.Add("Whale", DefinedViewportProjection.Perspective, new System.Drawing.Rectangle(500, 500, 500, 500), false);
            //rhinoView.Size = new System.Drawing.Size(500, 500);
            ////rhinoView.ActiveViewport.DisplayMode = DisplayModeDescription.GetDisplayMode(DisplayModeDescription.RenderedId);
            //IntPtr parent = Whale.Animation.GetControl.GetDC(rhinoView.Handle);


            var a = new RhinoWindows.Forms.Controls.ViewportControl();
            a.Viewport.DisplayMode = DisplayModeDescription.GetDisplayMode(DisplayModeDescription.ShadedId);
            Host.Child = a;
            //GridOne.Children.Add()
            ////var c = a.Viewport;
            //System.Windows.Forms.Control control = RhinoWindows.Forms.Controls.ViewportControl.FromHandle(rhinoView.Handle);
            //System.Windows.Forms.Control flp = System.Windows.Forms.Control.FromHandle(flpHandle);
            ////考虑动态创建可能存在跨线程访问，添加判断
            //if (flp.InvokeRequired)
            //{
            //    flp.Invoke(new Action(
            //    () => { flp.Controls.Add(a); }
            //    ));
            //}
            //else
            //{
            //    flp.Controls.Add(a);
            //}
        }
    }
}
