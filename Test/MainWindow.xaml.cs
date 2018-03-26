using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using User;
using User.SoftWare;
using User.UI;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Area.uSettings.USettingsChanged += USettings_USettingsChanged;
            Area.Flush();
            Area.uSettings.ReSet(true);
        }

        private void USettings_USettingsChanged(USettingsProperty key, UPropertyChangedEventArgs e)
        {
            Console.WriteLine(key.Name + e.NewValue);
        }

        private void SlideBar_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Lbl1.Content = (int)e.NewValue;
        }

        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {

            //Application.Current.Shutdown();
        }

        private void TriggerImage_Tapped(object sender, RoutedEventArgs e)
        {
            foreach (var item in User.SoftWare.Service.Weatherwebxml.Test("杭州"))
            {
                Console.WriteLine(item);
            }
        }
    }

    public class Area
    {
        public static USettings uSettings = new USettings(AppDomain.CurrentDomain.BaseDirectory, "r",true);
        public static USettingsProperty<string> LabelTextProperty = uSettings.Register("LabelText","长",true);
        public static void Flush()
        {
            uSettings.Flush();
        }
    }
}
