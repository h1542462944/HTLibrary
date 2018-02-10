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
        public static Brush UWhiteBrush = new SolidColorBrush(Color.FromArgb(102, 255, 255, 255));
        public static Brush UBlackBrush = new SolidColorBrush(Color.FromArgb(102, 0, 0, 0));
        public static Brush DWhiteBrush = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
        public static Brush DBlackBrush = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));


    }
    /// <summary>
    /// 为用户控件提供统一的内容模型.
    /// </summary>
    public class UContol : UserControl
    {
        public static readonly DependencyProperty ControlStyleProperty =
           DependencyProperty.Register("ControlStyle", typeof(ControlStyle), typeof(UContol), new PropertyMetadata(ControlStyle.Transparent, new PropertyChangedCallback(ControlStyle_Changed)));
        public static readonly DependencyProperty IsHighLightProperty =
           DependencyProperty.Register("IsHighLight", typeof(bool), typeof(UContol), new PropertyMetadata(false, new PropertyChangedCallback(IsHighLight_Changed)));
        public static readonly DependencyProperty ThemeColorProperty =
           DependencyProperty.Register("ThemeColor", typeof(Color), typeof(UContol), new PropertyMetadata(Colors.DeepSkyBlue, new PropertyChangedCallback(ThemeColor_Changed)));


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

        protected virtual void OnControlStyleChanged() { }
        protected virtual void OnHighLightChanged() { }
        protected virtual void OnThemeColor() { }

        private static void ControlStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UContol arg = (UContol)d;
            arg.OnControlStyleChanged();
        }
        private static void IsHighLight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UContol arg = (UContol)d;
            arg.OnHighLightChanged();
        }
        private static void ThemeColor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UContol arg = (UContol)d;
            arg.OnThemeColor();
        }
    }
    /// <summary>
    /// 为具有Check属性的控件提供基类.
    /// </summary>
    public class CheckControl : UContol
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
