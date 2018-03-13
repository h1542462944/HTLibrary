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
    /// TriggleImage.xaml 的交互逻辑
    /// </summary>
    public partial class TriggerImage : CheckControl
    {
        public TriggerImage()
        {
            InitializeComponent();
        }
        public ImageSource ImageSourceChecked
        {
            get => (ImageSource)GetValue(ImageSourceCheckedProperty);
            set => SetValue(ImageSourceCheckedProperty, value);
        }
        public ImageSource ImageSourceUnchecked
        {
            get => (ImageSource)GetValue(ImageSourceUncheckedProperty);
            set => SetValue(ImageSourceUncheckedProperty, value);
        }

        public static readonly DependencyProperty ImageSourceCheckedProperty =
           DependencyProperty.Register("ImageSourceChecked", typeof(ImageSource), typeof(TriggerImage), new PropertyMetadata(null, new PropertyChangedCallback(ImageSourceChecked_Changed)));
        public static readonly DependencyProperty ImageSourceUncheckedProperty =
            DependencyProperty.Register("ImageSourceUnchecked", typeof(ImageSource), typeof(TriggerImage), new PropertyMetadata(null, new PropertyChangedCallback(ImageSourceUnchecked_Changed)));

        private static void ImageSourceChecked_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TriggerImage arg = (TriggerImage)d;
            arg.OnImageSourceChecked((ImageSource)e.NewValue);
        }
        private static void ImageSourceUnchecked_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TriggerImage arg = (TriggerImage)d;
            arg.OnImageSourceUnchecked((ImageSource)e.NewValue);
        }

        protected override void OnChecked()
        {
            if (IsChecked)
            {
                this.Img.Source = ImageSourceChecked;
            }
            else
            {
                this.Img.Source = ImageSourceUnchecked;
            }
        }
        protected override void OnControlStyleChanged()
        {
            if (ControlStyle == ControlStyle.Transparent)
            {
                this.BdrBack.Background = Brushes.Transparent;
                this.Bdr.BorderBrush = ControlBase.UWhiteBrush;
                this.BdrHighlight.BorderBrush = Brushes.White;
            }
            else if (ControlStyle == ControlStyle.Light)
            {
                this.BdrBack.Background = ControlBase.DWhiteBrush;
                this.Bdr.BorderBrush = ControlBase.UBlackBrush;
                this.BdrHighlight.BorderBrush = Brushes.Black;
            }
            else
            {
                this.BdrBack.Background = ControlBase.DBlackBrush;
                this.Bdr.BorderBrush = ControlBase.UWhiteBrush;
                this.BdrHighlight.BorderBrush = Brushes.White;
            }
        }
        protected override void OnHighLightChanged()
        {
            if (IsHighLight)
            {
                this.BdrHighlight.Visibility = Visibility.Visible;
            }
            else
            {
                this.BdrHighlight.Visibility = Visibility.Hidden;
            }
        }
        protected void OnImageSourceChecked(ImageSource source)
        {
            if (IsChecked)
            {
                this.Img.Source = source;
            }
        }
        protected void OnImageSourceUnchecked(ImageSource source)
        {
            if (!IsChecked)
            {
                this.Img.Source = source;
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IsHighLight = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Bdr.Visibility = Visibility.Visible;
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.Bdr.Visibility = Visibility.Hidden;
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (CanAutoCheck && e.ChangedButton == MouseButton.Left)
            {
                IsChecked = !IsChecked;
            }
        }
    }
}
