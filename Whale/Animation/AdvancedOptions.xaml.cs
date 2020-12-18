using Rhino.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Whale.Animation
{
    /// <summary>
    /// AdvancedOptions.xaml 的交互逻辑
    /// </summary>
    public partial class AdvancedOptions : Window, INotifyPropertyChanged
    {
        private new Frame Owner;
        //FPS
        public List<int> m_FPSitems
        {
            get => Owner.FPSitems;
            set => SetProperty(ref Owner.FPSitems, value);
        }

        private int FPSindexRelay;
        public int m_FPSindex 
        { 
            get => Owner.FPSindex;
            set => SetProperty(ref Owner.FPSindex, value);
        }

        //Size
        public List<string> m_SizeItems
        {
            get => Owner.SizeItems;
            set => SetProperty(ref Owner.SizeItems, value);
        }

        private int PictWidthRelay;
        public string m_PictWidth
        {
            get => Owner.PictWidth.ToString();
            set {
                int relay;
                if (int.TryParse(value, out relay))
                    SetProperty(ref Owner.PictWidth, relay);
                else if(value.Length > 0)
                    MessageBox.Show("Please input a number in Resolution!");
            }
        }

        private int PictHeightRelay;
        public string m_PictHeight
        {
            get => Owner.PictHeight.ToString();
            set {
                int relay;
                if (int.TryParse(value, out relay))
                    SetProperty(ref Owner.PictHeight, relay);
                else if (value.Length > 0)
                    MessageBox.Show("Please input a number in Resolution!");
            }
        }

        private int PictSizeIndexRelay;
        public int PictSizeIndex
        {
            get => Owner.Sizeindex;
            set
            {

                string str = Owner.SizeItems[value];
                string[] splitedStr = str.Split(new char[] { ' ' });
                m_PictWidth = splitedStr[0];
                m_PictHeight = splitedStr[2];
                SetProperty(ref Owner.Sizeindex, value);
            }
        }

        //Type
        public List<System.Drawing.Imaging.ImageFormat> m_TypeItems
        {
            get => Owner.PictTypeItems;
            set => SetProperty(ref Owner.PictTypeItems, value);
        }

        private int TypeIndexRelay;
        public int m_Typeindex
        {
            get => Owner.Typeindex;
            set => SetProperty(ref Owner.Typeindex, value);
        }

        //TimeLine
        private bool DurationRelay;
        public bool m_duration
        {
            get => Owner.Duration;
            set => SetProperty(ref Owner.Duration, value);
        }
        private bool RemapRelay;
        public bool m_remap
        {
            get => Owner.Remap;
            set => SetProperty(ref Owner.Remap, value);
        }
        private double TimeInFrameRelay;
        public double m_timeInFrame
        {
            get => Owner.TimeInFrame;
            set => SetProperty(ref Owner.TimeInFrame, value);
        }

        //Time
        private double delayLogRelay;
        public double m_delayLog
        {
            get => Owner.DelayLog;
            set => SetProperty(ref Owner.DelayLog, value);
        }
        private bool SaveRelay;
        public bool m_save
        {
            get => Owner.Save;
            set => SetProperty(ref Owner.Save, value);
        }

        //Show
        private double SliderMultypleRelay;
        public double m_sliderMultiple
        {
            get => Owner.SliderMultiple;
            set => SetProperty(ref Owner.SliderMultiple, value);
        }
        private double ViewportReduceRelay;
        public double m_viewportReduce
        {
            get => Owner.ViewportReduce;
            set => SetProperty(ref Owner.ViewportReduce, value);
        }
        private bool ShowLabelRelay;
        public bool m_showLabel
        {
            get => Owner.ShowLabel;
            set => SetProperty(ref Owner.ShowLabel, value);
        }
        private double LabelSizeRelay;
        public double m_labelSize
        {
            get => Owner.LabelSize;
            set => SetProperty(ref Owner.LabelSize, value);
        }
        private bool ShowFrameRelay;
        public bool m_showFrame
        {
            get => Owner.ShowFrame;
            set => SetProperty(ref Owner.ShowFrame, value);
        }
        private bool ShowTimeRelay;
        public bool m_showTime
        {
            get => Owner.ShowTime;
            set => SetProperty(ref Owner.ShowTime, value);
        }
        private bool ShowPercentRelay;
        public bool m_showPercent
        {
            get => Owner.ShowPercent;
            set => SetProperty(ref Owner.ShowPercent, value);
        }
        private bool ShowRemainRelay;
        public bool m_showRemain
        {
            get => Owner.ShowRemain;
            set => SetProperty(ref Owner.ShowRemain, value);
        }
        private bool ShowGraphRelay;
        public bool m_showGraph
        {
            get => Owner.ShowGraph;
            set => SetProperty(ref Owner.ShowGraph, value);
        }
        private bool ShowGraphEventRelay;
        public bool m_showGraphEvent
        {
            get => Owner.ShowGraphOnEvent;
            set => SetProperty(ref Owner.ShowGraphOnEvent, value);
        }

        public bool m_useSlider
        {
            get => Owner.IsInputASlider;
            set => SetProperty(ref Owner.IsInputASlider, value);
        }

        public AdvancedOptions(Frame frame)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Owner = frame;

            FPSindexRelay = Owner.FPSindex;
            PictWidthRelay = Owner.PictWidth;
            PictHeightRelay = Owner.PictHeight;
            PictSizeIndexRelay = Owner.Sizeindex;

            TypeIndexRelay = Owner.Typeindex;

            DurationRelay = Owner.Duration;
            RemapRelay = Owner.Remap;
            TimeInFrameRelay = Owner.TimeInFrame;

            delayLogRelay = Owner.DelayLog;
            SaveRelay = Owner.Save;

            SliderMultypleRelay = Owner.SliderMultiple;
            ViewportReduceRelay = Owner.ViewportReduce;
            ShowLabelRelay = Owner.ShowLabel;
            LabelSizeRelay = Owner.LabelSize;
            ShowFrameRelay = Owner.ShowFrame;
            ShowTimeRelay = Owner.ShowTime;
            ShowPercentRelay = Owner.ShowPercent;
            ShowRemainRelay = Owner.ShowRemain;
            ShowGraphRelay = Owner.ShowGraph;
            ShowGraphEventRelay = Owner.ShowGraphOnEvent;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Owner.VariableParameterMaintenance();

            RhinoView rhinoView= Rhino.RhinoDoc.ActiveDoc.Views.Find("Whale", true);
            if(rhinoView != null)
            {
                int width = (int)((float)Owner.PictWidth / Owner.ViewportReduce);
                int height = (int)((float)Owner.PictHeight / Owner.ViewportReduce);
                rhinoView.Size = new System.Drawing.Size(width, height);
                rhinoView.Redraw();
            }
            
            Owner.ExpireSolution(true);
        }

        private void ButtonGraph_Click(object sender, RoutedEventArgs e)
        {
            Owner.ClearGraphData();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            RhinoView rhinoView = Rhino.RhinoDoc.ActiveDoc.Views.Find("Whale", true);
            if (rhinoView != null)
            {
                int width = (int)((float)Owner.PictWidth / Owner.ViewportReduce);
                int height = (int)((float)Owner.PictHeight / Owner.ViewportReduce);
                rhinoView.Size = new System.Drawing.Size(width, height);
                rhinoView.Redraw();
            }
            Owner.ExpireSolution(true);
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Owner.ExpireSolution(true);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Owner.FPSindex = FPSindexRelay;
            Owner.PictWidth = PictWidthRelay;
            Owner.PictHeight = PictHeightRelay;
            Owner.Sizeindex = PictSizeIndexRelay;

            Owner.Typeindex = TypeIndexRelay;

            Owner.Duration = DurationRelay;
            Owner.Remap = RemapRelay;
            Owner.TimeInFrame = TimeInFrameRelay;

            Owner.DelayLog = delayLogRelay;
            Owner.Save = SaveRelay;

            Owner.SliderMultiple = SliderMultypleRelay;
            Owner.ViewportReduce = ViewportReduceRelay;
            Owner.ShowLabel = ShowLabelRelay;
            Owner.LabelSize  = LabelSizeRelay;
            Owner.ShowFrame = ShowFrameRelay;
            Owner.ShowTime = ShowTimeRelay;
            Owner.ShowPercent = ShowPercentRelay;
            Owner.ShowRemain = ShowRemainRelay;
            Owner.ShowGraph = ShowGraphRelay;
            Owner.ShowGraphOnEvent = ShowGraphEventRelay;

            Owner.ExpireSolution(true);
            this.Close();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Owner.ResetSetting();
            Owner.ExpireSolution(true);
            this.Close();
        }
    }

}
