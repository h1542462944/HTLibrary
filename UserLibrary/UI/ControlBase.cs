using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace User.UI
{
    public enum ControlStyle
    {
        Transparent,
        Light,
        Dark
    }
    public enum SlideStyle
    {
        Default,
        Brush,
        TickValue
    }

    public class ControlBase
    {
        /// <summary>
        /// 颜色为66FFFFFF的纯色笔刷.
        /// </summary>
        public static readonly Brush UWhiteBrush = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
        public static readonly Brush UBlackBrush = new SolidColorBrush(Color.FromArgb(102, 0, 0, 0));
        /// <summary>
        /// 颜色为#CCFFFFFF的纯色笔刷.
        /// </summary>
        public static readonly Brush DWhiteBrush = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
        /// <summary>
        /// 颜色为#CC000000的纯色笔刷.
        /// </summary>
        public static readonly Brush DBlackBrush = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
        /// <summary>
        /// 颜色为#CC333333的纯色笔刷.
        /// </summary>
        public static readonly Brush DeepGrayBrush = new SolidColorBrush(Color.FromArgb(204, 51, 51, 51));
        /// <summary>
        /// 颜色为#CCCCCCCC的笔刷.
        /// </summary>
        public static readonly Brush LightGrayBrush = new SolidColorBrush(Color.FromArgb(204, 204, 204, 204));
    }
    /// <summary>
    /// 为用户控件提供统一的内容模型.
    /// </summary>
    public class UControl : UserControl
    {
        public static readonly DependencyProperty ControlStyleProperty =
           DependencyProperty.Register("ControlStyle", typeof(ControlStyle), typeof(UControl), new PropertyMetadata(ControlStyle.Transparent, new PropertyChangedCallback(ControlStyle_Changed)));
        public static readonly DependencyProperty IsHighLightProperty =
           DependencyProperty.Register("IsHighLight", typeof(bool), typeof(UControl), new PropertyMetadata(false, new PropertyChangedCallback(IsHighLight_Changed)));
        public static readonly DependencyProperty ThemeColorProperty =
           DependencyProperty.Register("ThemeColor", typeof(Color), typeof(UControl), new PropertyMetadata(Colors.DeepSkyBlue, new PropertyChangedCallback(ThemeColor_Changed)));
        bool isLeftMouseDown;

        public UControl()
        {
            this.MouseLeftButtonDown += UControl_MouseLeftButtonDown;
            this.MouseLeave += UControl_MouseLeave;
            this.MouseLeftButtonUp += UControl_MouseLeftButtonUp;
        }

        public ControlStyle ControlStyle
        {
            get { return (ControlStyle)GetValue(ControlStyleProperty); }
            set { SetValue(ControlStyleProperty, value); }
        }
        public bool IsHighLight
        {
            get { return (bool)GetValue(IsHighLightProperty); }
            set { SetValue(IsHighLightProperty, value); }
        }
        public Color ThemeColor
        {
            get { return (Color)GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }
        public event RoutedEventHandler Tapped;
        protected virtual void OnControlStyleChanged() { }
        protected virtual void OnHighLightChanged() { }
        protected virtual void OnThemeColor() { }
        private void UControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLeftMouseDown = true;
        }
        private void UControl_MouseLeave(object sender, MouseEventArgs e)
        {
            isLeftMouseDown = false;
        }
        private void UControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isLeftMouseDown)
            {
                Tapped?.Invoke(this, new RoutedEventArgs());
            }
        }
        private static void ControlStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnControlStyleChanged();
        }
        private static void IsHighLight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnHighLightChanged();
        }
        private static void ThemeColor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UControl arg = (UControl)d;
            arg.OnThemeColor();
        }
    }
    /// <summary>
    /// 为具有Check属性的控件提供基类.
    /// </summary>
    public class CheckControl : UControl
    {
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public bool CanAutoCheck
        {
            get { return (bool)GetValue(CanAutoCheckProperty); }
            set { SetValue(CanAutoCheckProperty, value); }
        }
        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckControl), new PropertyMetadata(false, new PropertyChangedCallback(IsChecked_Changed)));
        public static readonly DependencyProperty CanAutoCheckProperty =
           DependencyProperty.Register("CanAutoCheck", typeof(bool), typeof(CheckControl), new PropertyMetadata(true));
        public static new readonly DependencyProperty IsEnabledProperty =
           DependencyProperty.Register("IsEnabled", typeof(bool), typeof(CheckControl), new PropertyMetadata(true, new PropertyChangedCallback(IsEnabled_Changed)));


        protected virtual void OnChecked() { }
        protected override void OnControlStyleChanged()
        {
        }
        protected override void OnHighLightChanged()
        {
        }
        protected virtual void OnIsEnabled()
        {
        }
        private static void IsChecked_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckControl arg = (CheckControl)d;
            arg.OnChecked();
        }
        private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckControl arg = (CheckControl)d;
            arg.OnIsEnabled();
        }
    }
}
