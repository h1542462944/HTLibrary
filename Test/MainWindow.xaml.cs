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
using User;
using User.UMath;
using User.SoftWare;
using User.NetWork;
using System.Threading;
using User.UI;
using System.IO;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public NetworkMonitor NetworkMonitor = new NetworkMonitor();
        static public string StartupPath => AppDomain.CurrentDomain.BaseDirectory;
        ImagePickerMonitor imagepicker;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NetworkMonitor.Start();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Area.Start();
            This_Action();
        }

        private void This_Action()
        {
            //Console.WriteLine(Area.Arc);
            //Area.Qht = 12345;
            //Area. Top = new Point(1000.0, 1254.0);
            //Console.WriteLine(Area.Top);

            //var bitmap = User.UI.ImagePicker.GetScreenBitmap(new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));
            //var bitmapimage = ImagePicker.GetScreenBitmapImage(new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));
            //bitmap.Save(AppDomain.CurrentDomain.BaseDirectory + "1.bmp");
            //bitmapimage.Save(AppDomain.CurrentDomain.BaseDirectory + "1.bmp");
            //imagepicker = new ImagePickerMonitor(new ImagePickerAna(this, 3, 3))
            //{
            //    IsEnabled = true
            //};
            // Console.WriteLine(User.UI.PrimaryScreen.ScaleX);
            //Console.WriteLine(PrimaryScreen.WorkingArea);
            //Console.WriteLine(PrimaryScreen.DpiX);
            Console.WriteLine(PrimaryScreen.DESKTOP);
            //Console.WriteLine(PrimaryScreen.ScaleX);
            //Console.WriteLine(PrimaryScreen.ScaleY);
            //Console.WriteLine(Tools.GetMousePosition());
            //Console.WriteLine(this.Left);
            //Console.WriteLine(Mouse.GetPosition(this));
        }
    }
    public struct Cr : IUSettingsConvertArray
    {
        private int a;

        public Cr(int a)
        {
            this.a = a;
        }

        public int A { get => a; set => a = value; }

        public object USettingsConvertArray(object[] contents)
        {
            return new Cr((int)contents[0]);
        }

        public object[] USettingsConvertArray()
        {
            return new object[] { A };
        }
    }

    public static class Area
    {
        private static string localFolder = AppDomain.CurrentDomain.BaseDirectory;
        public static USettings USettings = new USettings(LocalFolder, "uSettings");

        public static USettingsProperty<Cr[]> ArcProperty = USettings.Register("Arc", new Cr[] { new Cr(3), new Cr(4), new Cr(5) });
        public static USettingsProperty<int> QhtProperty = USettings.Register("Qht", 123);
        public static USettingsProperty<Point> TopProperty = USettings.Register("Top", new Point(18, 18));

        public static Cr[] Arc { get => ArcProperty.Value; set => ArcProperty.Value = value; }
        public static int Qht { get => QhtProperty.Value; set => QhtProperty.Value = value; }
        public static Point Top { get => TopProperty.Value; set => TopProperty.Value = value; }

        public static string LocalFolder { get => localFolder; set => localFolder = value; }

        public static void Start()
        {
            USettings.USettingsChanged += USettings_USettingsChanged;
        }

        private static void USettings_USettingsChanged(USettingsKey key, USettingsChangedEventargs e)
        {
            if (key == ArcProperty)
            {
                MessageBox.Show(string.Format("i'm changed {0}", key.Name));

            }
            if (key == QhtProperty)
            {
                MessageBox.Show("!!!");
            }
        }
    }
}
