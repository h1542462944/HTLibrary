using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace User.UI
{
    /// <summary>
    /// SlideBar.xaml 的交互逻辑
    /// </summary>
    public partial class SlideBar : UContol
    {
        public SlideBar()
        {
            this.Focusable = true;
            InitializeComponent();
        }

        bool isLeftMouseDown = false;
        private double slideValueOld = double.NaN;
        public double SlideValueMin
        {
            get { return (double)GetValue(SlideValueMinProperty); }
            set { SetValue(SlideValueMinProperty, value); }
        }
        public double SlideValueMax
        {
            get { return (double)GetValue(SlideValueMaxProperty); }
            set { SetValue(SlideValueMaxProperty, value); }
        }
        public double SlideValue
        {
            get { return (double)GetValue(SlideValueProperty); }
            set
            {
                SetValue(SlideValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SlideValue)));
            }
        }
        public int SlideValueInt
        {
            get { return (int)GetValue(SlideValueIntProperty); }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SlideValueInt)));
                SetValue(SlideValueIntProperty, value);
            }
        }
        public SlideStyle SlideStyle
        {
            get { return (SlideStyle)GetValue(SlideStyleProperty); }
            set { SetValue(SlideStyleProperty, value); }
        }
        public Brush SlideBrush
        {
            get { return (Brush)GetValue(SlideBrushProperty); }
            set { SetValue(SlideBrushProperty, value); }
        }
        public double TickValue
        {
            get { return (double)GetValue(TickValueProperty); }
            set { SetValue(TickValueProperty, value); }
        }

        public static readonly DependencyProperty SlideValueMinProperty =
            DependencyProperty.Register("SlideValueMin", typeof(double), typeof(SlideBar), new PropertyMetadata(0.0, new PropertyChangedCallback(SlideValueMin_Changed)));
        public static readonly DependencyProperty SlideValueMaxProperty =
            DependencyProperty.Register("SlideValueMax", typeof(double), typeof(SlideBar), new PropertyMetadata(100.0, new PropertyChangedCallback(SlideValueMax_Changed)));
        public static readonly DependencyProperty SlideValueProperty =
            DependencyProperty.Register("SlideValue", typeof(double), typeof(SlideBar), new PropertyMetadata(15.0, new PropertyChangedCallback(SlideValue_Changed)));
        public static readonly DependencyProperty SlideStyleProperty =
            DependencyProperty.Register("SlideStyle", typeof(SlideStyle), typeof(SlideBar), new PropertyMetadata(SlideStyle.Default, new PropertyChangedCallback(SlideStyle_Changed)));
        public static readonly DependencyProperty SlideBrushProperty =
            DependencyProperty.Register("SlideBrush", typeof(Brush), typeof(SlideBar), new PropertyMetadata(null, new PropertyChangedCallback(SlideBrush_Changed)));
        public static readonly DependencyProperty SlideValueIntProperty =
            DependencyProperty.Register("SlideValueInt", typeof(int), typeof(SlideBar), new PropertyMetadata(15));
        public static readonly DependencyProperty TickValueProperty =
            DependencyProperty.Register("TickValue", typeof(double), typeof(SlideBar), new PropertyMetadata(15.0, new PropertyChangedCallback(TickValue_Changed)));
     
        /// <summary>
        /// SlideValue发生变化.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SlideValueChanged;
        private event PropertyChangedEventHandler PropertyChanged;

        protected void OnSlideValueChanged()
        {
            double bi = Tools.Checkdouble((SlideValue - SlideValueMin) / (SlideValueMax - SlideValueMin), 0, 1);
            this.ColumnDefi0.Width = new GridLength(bi, GridUnitType.Star);
            this.ColumnDefi1.Width = new GridLength(1 - bi, GridUnitType.Star);
            if (SlideStyle == SlideStyle.Default)
            {
                Color cb;
                if (ControlStyle == ControlStyle.Light)
                {
                    cb = Color.FromArgb(48, 0, 0, 0);
                }
                else
                {
                    cb = Color.FromArgb(48, 255, 255, 255);
                }
                GradientStop[] stops = new GradientStop[]
                {
                    new GradientStop(ThemeColor,0),
                    new GradientStop(ThemeColor,bi),
                    new GradientStop(cb,bi),
                    new GradientStop(cb,1)
                };
                LinearGradientBrush brush = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0.5),
                    EndPoint = new Point(1, 0.5),
                    GradientStops = new GradientStopCollection(stops)
                };
                BdrBack.Background = brush; 
            }
            else if (SlideStyle == SlideStyle.TickValue)
            {
                OnTickValueChanged();
            }
            if (SlideValue != slideValueOld && IsLoaded)
            {
                SlideValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(slideValueOld, SlideValue));
                slideValueOld = SlideValue;
            }
            SlideValueInt = (int)SlideValue;
        }
        protected void OnSlideStyleChanged()
        {
            if (SlideStyle == SlideStyle.Brush)
            {
                BdrBack.Background = SlideBrush;
            }
            else if(SlideStyle == SlideStyle.Default)
            {
                OnSlideValueChanged();
            }
            else
            {
                OnTickValueChanged();
            }
        }
        protected void OnSlideBrushChanged()
        {
            if (SlideStyle == SlideStyle.Brush)
            {
                OnSlideStyleChanged();
            }
        }
        protected override void OnControlStyleChanged()
        {
            if (ControlStyle == ControlStyle.Light)
            {
                BdrM.Background = ControlBase.UBlackBrush;
                BdrHighlight.BorderBrush = Brushes.Black;
            }
            else
            {
                BdrM.Background = ControlBase.UWhiteBrush;
                BdrHighlight.BorderBrush = Brushes.White;
            }
            OnSlideValueChanged();
        }
        protected override void OnHighLightChanged()
        {
            if (IsHighLight)
            {
                BdrHighlight.Visibility = Visibility.Visible;
            }
            else
            {
                BdrHighlight.Visibility = Visibility.Hidden;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                SlideValue = Tools.Checkdouble(SlideValue + 0.05 * (SlideValueMax - SlideValueMin), SlideValueMin, SlideValueMax);
            }
            else if (e.Key == Key.Left)
            {
                SlideValue = Tools.Checkdouble(SlideValue - 0.05 * (SlideValueMax - SlideValueMin), SlideValueMin, SlideValueMax);
            }
        }
        protected void OnTickValueChanged()
        {
            if (SlideStyle == SlideStyle.TickValue)
            {
                double bi = Tools.Checkdouble(TickValue / SlideValue, 0, 1);
                Color cb;
                if (ControlStyle == ControlStyle.Light)
                {
                    cb = Color.FromArgb(48, 0, 0, 0);
                }
                else
                {
                    cb = Color.FromArgb(48, 255, 255, 255);
                }
                GradientStop[] stops = new GradientStop[]
                {
                    new GradientStop(ThemeColor,0),
                    new GradientStop(ThemeColor,bi),
                    new GradientStop(cb,bi),
                    new GradientStop(cb,1)
                };
                LinearGradientBrush brush = new LinearGradientBrush()
                {
                    StartPoint = new Point(0, 0.5),
                    EndPoint = new Point(1, 0.5),
                    GradientStops = new GradientStopCollection(stops)
                };
                BdrBack.Background = brush;
            }
        }

        private static void SlideValueMin_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideBar arg = (SlideBar)d;
            arg.OnSlideValueChanged();
        }
        private static void SlideValueMax_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideBar arg = (SlideBar)d;
            arg.OnSlideValueChanged();
        }
        private static void SlideValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideBar arg = (SlideBar)d;
            arg.OnSlideValueChanged();
        }
        private static void SlideStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideBar arg = (SlideBar)d;
            arg.OnSlideStyleChanged();
        }
        private static void SlideBrush_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlideBar arg = (SlideBar)d;
            arg.OnSlideBrushChanged();
        }
        private static void TickValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SlideBar)d).OnTickValueChanged();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isLeftMouseDown = true;
                SetSlideValue(e.GetPosition(this).X);
                if (IsHighLight)
                {
                    Focus();
                }
            }
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isLeftMouseDown)
            {
                SetSlideValue(e.GetPosition(this).X);
            }
            if (ControlStyle == ControlStyle.Light)
            {
                BdrM.Background = ControlBase.DBlackBrush;
            }
            else
            {
                BdrM.Background = ControlBase.DWhiteBrush;
            }
        }
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isLeftMouseDown = false;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            //isLeftMouseDown = false;
            if (ControlStyle == ControlStyle.Light)
            {
                BdrM.Background = ControlBase.UBlackBrush;
            }
            else
            {
                BdrM.Background = ControlBase.UWhiteBrush;
            }
        }

        private void SetSlideValue(double value)
        {
            double v = Tools.Checkdouble((value - 8) / (this.ActualWidth - 16), 0, 1);
            SlideValue = SlideValueMin + (SlideValueMax - SlideValueMin) * v;
        }

    }
}
