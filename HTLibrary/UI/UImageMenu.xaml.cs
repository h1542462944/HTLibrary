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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace User.UI
{
    /// <summary>
    /// UImageMenu.xaml 的交互逻辑
    /// </summary>
    public partial class UImageMenu : UControl
    {
        public UImageMenu()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UImageMenu), new PropertyMetadata("标题", new PropertyChangedCallback(Text_Changed)));
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UImageMenu), new PropertyMetadata(null, new PropertyChangedCallback(ImageSource_Changed)));

        protected override void OnControlStyleChanged()
        {
            if (ControlStyle == ControlStyle.Transparent)
            {
                this.Grid1.Background = Brushes.Transparent;
                this.Bdr.BorderBrush = ControlBase.UWhiteBrush;
                this.LabelTitle.Foreground = Brushes.White;
                Bdr1.Background = ControlBase.UWhiteBrush;
            }
            else if (ControlStyle == ControlStyle.Light)
            {
                this.Grid1.Background = ControlBase.LightGrayBrush;
                this.Bdr.BorderBrush = ControlBase.UBlackBrush;
                this.LabelTitle.Foreground = Brushes.Black;
                Bdr1.Background = ControlBase.UBlackBrush;
            }
            else
            {
                this.Grid1.Background = ControlBase.DeepGrayBrush;
                this.Bdr.BorderBrush = ControlBase.UWhiteBrush;
                this.LabelTitle.Foreground = Brushes.White;
                Bdr1.Background = ControlBase.UWhiteBrush;
            }
        }
        void OnTextChanged()
        {
            LabelTitle.Content = Text;
        }
        void OnImageSourceChanged()
        {
            ImageIcon.Source = ImageSource;
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Bdr1.Visibility = Visibility.Visible;
                Scale.ScaleX = 0.95;
                Scale.ScaleY = 0.95;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Bdr.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Bdr.Visibility = Visibility.Collapsed;
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }

        private static void Text_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UImageMenu)d).OnTextChanged();
        }
        private static void ImageSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UImageMenu)d).OnImageSourceChanged();
        }

    }
}