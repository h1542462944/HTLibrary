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
    /// QuickButton.xaml 的交互逻辑
    /// </summary>
    public partial class QuickButton : CheckControl
    {
        public QuickButton()
        {
            InitializeComponent();
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(QuickButton), new PropertyMetadata(null, new PropertyChangedCallback(ImageSource_Changed)));
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(QuickButton), new PropertyMetadata("关", new PropertyChangedCallback(Description_Changed)));
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(QuickButton), new PropertyMetadata("设置", new PropertyChangedCallback(Title_Changed)));

        void OnImageSourceChanged()
        {
            ImgIcon.Source = ImageSource;
        }
        void OnDescriptionChanged()
        {
            Lbl1.Content = Description;
        }
        void OnTitleChanged()
        {
            LblTitle.Content = Title;
        }
        protected override void OnControlStyleChanged()
        {
            if (ControlStyle == ControlStyle.Transparent)
            {
                if (!IsChecked)
                    GridMain.Background = Brushes.Transparent;
                Bdr1.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Bdr2.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Bdr.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Lbl1.Foreground = UserBrushes.DeepWhiteBrush;
                LblTitle.Foreground = UserBrushes.DeepWhiteBrush;
            }
            else if (ControlStyle == ControlStyle.Light)
            {
                if (!IsChecked)
                    GridMain.Background = UserBrushes.LightGrayBrush;
                Bdr1.BorderBrush = UserBrushes.RelativeBlackBrush;
                Bdr2.BorderBrush = UserBrushes.RelativeBlackBrush;
                Bdr.BorderBrush = UserBrushes.RelativeBlackBrush;
                Lbl1.Foreground = UserBrushes.DeepBlackBrush;
                LblTitle.Foreground = UserBrushes.DeepBlackBrush;
            }
            else
            {
                if (!IsChecked)
                    GridMain.Background = UserBrushes.DeepGrayBrush;
                Bdr1.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Bdr2.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Bdr.BorderBrush = UserBrushes.RelativeWhiteBrush;
                Lbl1.Foreground = UserBrushes.DeepWhiteBrush;
                LblTitle.Foreground = UserBrushes.DeepWhiteBrush;
            }
        }
        protected override void OnChecked()
        {
            if (IsChecked)
            {
                GridMain.Background = ThemeBrush;
            }
            else
            {
                OnControlStyleChanged();
            }
        }
        protected override void OnThemeColor()
        {
            OnChecked();
        }
        protected override void OnIsOpenChanged()
        {
            if (IsOpened)
            {
                Bdr2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Bdr2.Visibility = Visibility.Visible;
            }
        }
        private static void ImageSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QuickButton)d).OnImageSourceChanged();
        }
        private static void Description_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QuickButton)d).OnDescriptionChanged();
        }
        private static void Title_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QuickButton)d).OnTitleChanged();
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpened && e.ChangedButton == MouseButton.Left)
            {
                Bdr1.Visibility = Visibility.Visible;
                Scale.ScaleX = 0.95;
                Scale.ScaleY = 0.95;
            }
        }
        private void Control_MouseLeave(object sender, MouseEventArgs e)
        {
            Bdr.Visibility = Visibility.Collapsed;
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsOpened && e.LeftButton == MouseButtonState.Released)
            {
                Bdr.Visibility = Visibility.Visible;
            }
            else
            {
                Bdr.Visibility = Visibility.Collapsed;
            }
        }
        private void Control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Bdr1.Visibility = Visibility.Collapsed;
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;
        }
        private void Control_Tapped(object sender, RoutedEventArgs e)
        {
            if (CanAutoCheck)
            {
                IsChecked = !IsChecked;
            }
        }
    }
}
