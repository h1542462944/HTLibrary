using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using User.SoftWare;
namespace User.UI
{
    /// <summary>
    /// 可以拖动的窗体.
    /// </summary>
    public class DragWindow : Window
    {
        string folder = "";
        string rootName = "";
        string settingName = "";
        USettings uSettings;
        USettingsProperty<Point> AppLocationProperty;
        List<FrameworkElement> allowdragelement = new List<FrameworkElement>();
        object mousedownelement;
        Point mousepoint;
        DispatcherTimer innertimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(10),
            IsEnabled = false
        };

        Point AppLocation { get => AppLocationProperty.Value; set => AppLocationProperty.Value = value; }
        Size ScreenSize => new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        protected string SettingName { get => settingName; set => settingName = value; }

        protected DragWindow(string settingName)
        {
            this.settingName = settingName;
            SetSettingsFolder(AppDomain.CurrentDomain.BaseDirectory, "Local");
            AppLocationProperty = uSettings.Register(settingName + "AppLocation", new Point(0.4, 0.4));
            this.Left = ScreenSize.Width * AppLocation.X;
            this.Top = ScreenSize.Height * AppLocation.Y;
            this.innertimer.Tick += Innertimer_Tick;
            this.Closing += Window_Closing;
        }
        private void Innertimer_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Tools.GetMousePosition();
                Point tpoing = new Point(p.X - mousepoint.X, p.Y - mousepoint.Y);
                this.Left = tpoing.X; this.Top = tpoing.Y;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            AppLocation = new Point(this.Left / ScreenSize.Width, this.Top / ScreenSize.Height);
        }
        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mousedownelement = sender;
                mousepoint = e.GetPosition(null);
                innertimer.IsEnabled = true;
            }
        }
        private void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (Left < 0)
                    Left = 0;
                else if (Left > ScreenSize.Width - Width)
                    Left = ScreenSize.Width - Width;
                if (Top < 0)
                    Top = 0;
                if (Top > ScreenSize.Height - Height)
                    Top = ScreenSize.Height - Height;
            }
            innertimer.IsEnabled = false;
        }

        protected void SetSettingsFolder(string folder, string rootName)
        {
            if (this.folder != folder || this.rootName != rootName)
            {
                this.uSettings = new USettings(folder, rootName);
            }
        }
        protected void Register(params FrameworkElement[] element)
        {
            foreach (var item in element)
            {
                if (!allowdragelement.Contains(item))
                {
                    allowdragelement.Add(item);
                    item.MouseDown += Item_MouseDown;
                    item.MouseUp += Item_MouseUp;
                }
            }
        }
    }
}
