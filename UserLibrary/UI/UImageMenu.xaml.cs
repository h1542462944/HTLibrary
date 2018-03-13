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
    public partial class UImageMenu : User.UI.UControl
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
                this.Border1.BorderBrush = ControlBase.UWhiteBrush;
                this.LabelTitle.Foreground = Brushes.White;
                BdrBack.Background = ControlBase.UWhiteBrush;
            }
            else if (ControlStyle == ControlStyle.Light)
            {
                this.Grid1.Background = ControlBase.DWhiteBrush;
                this.Border1.BorderBrush = ControlBase.UBlackBrush;
                this.LabelTitle.Foreground = Brushes.Black;
                BdrBack.Background = ControlBase.UBlackBrush;
            }
            else
            {
                this.Grid1.Background = ControlBase.DBlackBrush;
                this.Border1.BorderBrush = ControlBase.UWhiteBrush;
                this.LabelTitle.Foreground = Brushes.White;
                BdrBack.Background = ControlBase.UWhiteBrush;
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
            base.OnMouseDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                BdrBack.Visibility = Visibility.Visible;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Border1.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Border1.Visibility = Visibility.Collapsed;
            BdrBack.Visibility = Visibility.Collapsed;
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            BdrBack.Visibility = Visibility.Collapsed;
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