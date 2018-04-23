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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace User.UI
{
    /// <summary>
    /// TriggerSwitch.xaml 的交互逻辑
    /// </summary>
    public partial class TriggerSwitch : CheckControl
    {
        public TriggerSwitch()
        {
            InitializeComponent();
        }

        protected override void OnThemeColor()
        {
            if (IsOpened && IsChecked)
            {
                OnChecked();
            }
        }
        protected override void OnChecked()
        {
            if (IsOpened)
            {
                if (!IsChecked)
                {
                    OnControlStyleChanged();
                    Elp1.Margin = new Thickness(8, 5.5, 0, 0);
                }
                else
                {
                    Path1.Stroke = ThemeBrush;
                    Path1.Fill = ThemeBrush;
                    Elp1.Fill = Brushes.White;
                    Elp1.Margin = new Thickness(32, 5.5, 0, 0);
                }
            }
        }
        protected override void OnControlStyleChanged()
        {
            if (IsOpened && !IsChecked)
            {
                Path1.Fill = null;
                if (ControlStyle == ControlStyle.Light)
                {
                    Path1.Stroke = Brushes.Black;
                    Elp1.Stroke = Brushes.Black;
                    Bdr1.BorderBrush = Brushes.Black;
                }
                else
                {
                    Path1.Stroke = Brushes.White;
                    Elp1.Stroke = Brushes.White;
                    Bdr1.BorderBrush = Brushes.White;
                }
            }
        }
        protected override void OnHighLightChanged()
        {
            if (IsHighLight)
            {
                Bdr1.Visibility = Visibility.Visible;
            }
            else
            {
                Bdr1.Visibility = Visibility.Hidden;
            }
        }
        protected override void OnIsOpenChanged()
        {
            if (IsOpened)
            {
                OnChecked();
                OnControlStyleChanged();
            }
            else
            {
                if (IsChecked)
                {
                    Path1.Stroke = Brushes.Gray;
                    Path1.Fill = Brushes.Gray;
                    Elp1.Fill = Brushes.White;
                }
                else
                {
                    Path1.Stroke = Brushes.Gray;
                    Path1.Fill = null;
                    Elp1.Fill = Brushes.Gray;
                }
            }
        }
        public void InvokeCheck(bool value)
        {
            if (IsChecked == value)
            {
                return;
            }
            OnChecked();
            Thickness t1 = new Thickness(8, 5.5, 0, 0);
            Thickness t2 = new Thickness(32, 5.5, 0, 0);
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            };

            if (!IsChecked)
            {
                thicknessAnimation.From = t1;
                thicknessAnimation.To = t2;
            }
            else
            {
                thicknessAnimation.From = t2;
                thicknessAnimation.To = t1;
            }

            IsChecked = !IsChecked;

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(thicknessAnimation);
            Storyboard.SetTarget(thicknessAnimation, Elp1);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("Margin"));
            storyboard.Begin();
        }
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsOpened && e.ChangedButton == MouseButton.Left)
            {
                InvokeCheck(!IsChecked);
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpened)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Path1.Stroke = ThemeBrush;
                    Path1.Fill = ThemeBrush;
                    IsHighLight = false;
                }
            }
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            OnChecked();
            Path2.Visibility = Visibility.Hidden;
        }
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsOpened)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    Path1.Stroke = ThemeBrush;
                    Path1.Fill = ThemeBrush;
                }
                if (IsChecked && IsOpened)
                {
                    Path2.Visibility = Visibility.Visible;
                }
            }
        }
        
    }
}
