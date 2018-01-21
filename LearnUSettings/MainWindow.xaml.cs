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
using User.SoftWare;
using User.SoftWare.Linq;
namespace LearnUSettings
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();     
        }


        public void Example()
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Area.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => 
            {
                double value = 1.0 / 3;
                for (int i = 1; i < 1000000000; i++)
                {
                    Console.WriteLine("value:{0} n:{1}", value, i);
                    value = Next(value, i);
                }
            }
                );
         
        }

        private void M()
        {
            Product product = new Product(
                new ProductInfo("timemix","1.0.0.0","timemix",DateTime.Now,
                    new UpdateInfo("1.0.0.0","123",DateTime.Now, new OperateInfo("download","+++++"))),
                new ProductInfo("ke","1.0.0.0","123",DateTime.Now)
              
                    ) ;
        }
        private double Next(double value,int n)
        {
            return value + (value * value) / (n*n);
        }
    }

    public static class Area
    {
        public static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public static USettings uSettings = new USettings(LocalFolder, "Settings");
        public static readonly USettingsProperty ExampleProperty = uSettings.Register("Example", new string[] { "123", "345" });
        public static readonly USettingsProperty AniProperty = uSettings.Register("Ani", 1234);

        public static string[] Example { get => (string[])ExampleProperty.Value; set => ExampleProperty.Value = value; }
        public static int Ani { get => (int)AniProperty.Value; set => AniProperty.Value = value; }
        public static void Start()
        {
            uSettings.USettingsChanged += USettings_USettingsChanged;
        }

        private static void USettings_USettingsChanged(USettingsKey key, USettingsChangedEventargs e)
        {
            if (key == ExampleProperty)
            {
                MessageBox.Show(string.Format("I'm Changed:{0}", key.Name));
            }
        }
    }

}
