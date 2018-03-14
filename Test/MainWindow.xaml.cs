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
        Area area = new Area();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            area.uSettings.USettingsChanged += USettings_USettingsChanged;
            area.Flush();
        }

        private void USettings_USettingsChanged(USettingsProperty key, PropertyChangedEventargs e)
        {
            Console.WriteLine(key.Name + e.NewValue);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Lbl1.SetBinding(ContentProperty, new Binding("SlideValueInt") { Source = SliderBar1 });
            ColorPicker1.Value = new ColorP(Colors.OrangeRed);
        }
        private void SlideBar_SlideValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Lbl1.Content = (int)e.NewValue;
        }

        private void UImageMenu_Tapped(object sender, RoutedEventArgs e)
        {

            //Application.Current.Shutdown();
        }
        
    }

    public class Area
    {
        public USettings uSettings = new USettings(AppDomain.CurrentDomain.BaseDirectory, "r");
        public USettingsProperty<Point> E2Property;
        public USettingsProperty<Size> S2Property;
        public USettingsProperty<Point> E1Property;
        public USettingsProperty<Size> S1Property;
        public USettingsProperty<Point> E4Property;
        public USettingsProperty<Size> S4Property;
        public USettingsProperty<Point> E5Property;
        public USettingsProperty<Size> S5Property;
        public USettingsProperty<Point> E6Property;
        public Area()
        {
            E2Property = uSettings.Register("E2", new Point(5, 7));
            S2Property = uSettings.Register("S2", new Size(4, 8));
        }
        public void Flush()
        {
            uSettings.Flush();
        }
    }
}
