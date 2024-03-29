﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orthoptera.Wpf
{
    /// <summary>
    /// Interaction logic for OrthopteraBaseWindowControl.xaml
    /// </summary>
    public partial class OrthopteraBaseWindowControl : UserControl
    {



        public Brush BackGroudBrushFather
        {
            get { return (Brush)GetValue(BackGroudBrushProperty); }
            set { SetValue(BackGroudBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackGroudBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackGroudBrushProperty =
            DependencyProperty.Register("BackGroudBrushFather", typeof(Brush), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(180, 240, 240, 240))));



        public Brush BorderBrushFather
        {
            get { return (Brush)GetValue(BorderBrushFatherProperty); }
            set { SetValue(BorderBrushFatherProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrushFather.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushFatherProperty =
            DependencyProperty.Register("BorderBrushFather", typeof(Brush), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(120, 120, 120))));



        public Visibility ForeMaseVisibility
        {
            get { return (Visibility)GetValue(ForeMaseVisibilityProperty); }
            set { SetValue(ForeMaseVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForeMaseVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForeMaseVisibilityProperty =
            DependencyProperty.Register("ForeMaseVisibility", typeof(Visibility), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(Visibility.Collapsed));



        public object TittleLeftContent
        {
            get { return (object)GetValue(TittleLeftProperty); }
            set { SetValue(TittleLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TittleLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TittleLeftProperty =
            DependencyProperty.Register("TittleLeftContent", typeof(object), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(null));

        public object TittleRightContent
        {
            get { return (object)GetValue(TittleRightProperty); }
            set { SetValue(TittleRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TittleLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TittleRightProperty =
            DependencyProperty.Register("TittleRightContent", typeof(object), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(null));




        public string TittleText
        {
            get { return (string)GetValue(TittleTextProperty); }
            set { SetValue(TittleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TittleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TittleTextProperty =
            DependencyProperty.Register("TittleText", typeof(string), typeof(OrthopteraBaseWindowControl), new PropertyMetadata("Orthoptera Window"));


        public object BottomLeftContent
        {
            get { return (object)GetValue(BottomLeftContentProperty); }
            set { SetValue(BottomLeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TittleLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomLeftContentProperty =
            DependencyProperty.Register("BottomLeftContent", typeof(object), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(null));





        public Action RefreshClicked
        {
            get { return (Action)GetValue(RefreshClickedProperty); }
            set { SetValue(RefreshClickedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshClicked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshClickedProperty =
            DependencyProperty.Register("RefreshClicked", typeof(Action), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(null));




        public Window Owner
        {
            get { return (Window)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Owner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(Window), typeof(OrthopteraBaseWindowControl), new PropertyMetadata(null));




        public OrthopteraBaseWindowControl()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                RefreshClicked();
                Owner.Close();
            }

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
                RefreshClicked();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) 
        {
            if (Owner != null)
                Owner.Close();
        }

        private void ColorZone_MouseMove(object sender, MouseEventArgs e) 
        {
            if (e.LeftButton == MouseButtonState.Pressed  && Owner != null)
                Owner.DragMove();
        }
    }
}
