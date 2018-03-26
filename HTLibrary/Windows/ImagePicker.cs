using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drawing = System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.ComponentModel;

namespace User.Windows
{
    /// <summary>
    /// 屏幕检测的数据,扩展自Rect.
    /// </summary>
    public struct ScreenMonitorAna
    {
        Rect area;
        int xnum;
        int ynum;
        int middleColorValue;

        public ScreenMonitorAna(Rect area, int xnum, int ynum, int middleColorValue = 230)
        {
            this.area = area;
            this.xnum = xnum;
            this.ynum = ynum;
            this.middleColorValue = middleColorValue;
        }
        public ScreenMonitorAna(Window window, int xnum, int ynum, int middleColorValue = 230) : this(new Rect(window.Left, window.Top, window.Width, window.Height), xnum, ynum, middleColorValue)
        {
        }
        public ScreenMonitorAna(Window window) : this(window, 3, 3)
        {
        }

        public Rect Area { get => area; set => area = value; }
        public int Xnum { get => xnum; set => xnum = value; }
        public int Ynum { get => ynum; set => ynum = value; }
        public int Middlevalue { get => middleColorValue; set => middleColorValue = value; }
    }
    /// <summary>
    /// 表示一个屏幕检测的对象,用于监视 System.Windows.Window 的背景是亮的还是暗的.
    /// </summary>
    public class ScreenMonitor
    {
        private DispatcherTimer innertimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1),
            IsEnabled = false
        };
        private int tick = 0;
        private List<ScreenMonitorProperty> properties = new List<ScreenMonitorProperty>();
        private int xnum = 3;
        private int ynum = 3;
        private int middleColorValue = 230;

        public ScreenMonitor()
        {
            innertimer.Tick += Innertimer_Tick;
        }

        public bool IsEnabled { get => innertimer.IsEnabled; set => innertimer.IsEnabled = value; }
        public List<ScreenMonitorProperty> Properties { get => properties; }
        public int Xnum { get => xnum; set => xnum = value; }
        public int Ynum { get => ynum; set => ynum = value; }
        public int MiddleColorValue { get => middleColorValue; set => middleColorValue = value; }

        public ScreenMonitorProperty Register(Window window)
        {
            ScreenMonitorProperty property = new ScreenMonitorProperty(window);
            properties.Add(property);
            return property;
        }

        private void Innertimer_Tick(object sender, EventArgs e)
        {
            try
            {
                List<int> list = new List<int>();
                List<ScreenMonitorAna> imagepickeranas = new List<ScreenMonitorAna>();
                foreach (var item in properties)
                {
                    imagepickeranas.Add(new ScreenMonitorAna(item.Window, this.xnum, this.ynum, this.middleColorValue));
                }
                //Console.WriteLine(tick);
                if (tick <= 5 || tick == 13)
                {
                    bool[] values = GetScreenIsRelativeDark(imagepickeranas.ToArray());
                    bool ischanged = false;
                    for (int i = 0; i < properties.Count; i++)
                    {
                        if (tick >= 1)
                        {
                            if (properties[i].IsRelativeDark != values[i])
                            {
                                ischanged = true;
                            }
                        }
                        properties[i]._IsRelativeDark = values[i];
                    }
                    if (tick == 0)
                    {
                        tick = 1;
                    }
                    else
                    {
                        if (ischanged)
                        {
                            tick = 1;
                        }
                        else
                        {
                            if (tick <= 5)
                            {
                                tick++;
                            }
                            else
                            {
                                tick = 6;
                            }
                        }
                    }
                }
                else
                {
                    tick++;
                }
            }
            catch (Exception)//句柄无效
            {

            }
        }

        public static Drawing.Bitmap GetScreenBitmap(Rect area)
        {
            float scaling = PrimaryScreen.ScaleX;
            Drawing.Rectangle rx = new Drawing.Rectangle((int)(area.X * scaling), (int)(area.Y * scaling), (int)(area.Width * scaling), (int)(area.Height * scaling));
            var bitmap = new Drawing.Bitmap(rx.Width, rx.Height, Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Drawing.Graphics g = Drawing.Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rx.X, rx.Y, 0, 0, rx.Size, Drawing.CopyPixelOperation.SourceCopy);
            }
            return bitmap;

        }
        public static Drawing.Bitmap GetScreenBitmap()
        {
            Rect rect = new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            return GetScreenBitmap(rect);
        }
        public static Drawing.Bitmap GetScreenBitmap(Window window)
        {
            Rect rect = new Rect(window.Left, window.Top, window.Width, window.Height);
            return GetScreenBitmap(rect);
        }
        public static BitmapImage GetScreenBitmapImage(Rect area)
        {
            Drawing.Bitmap bitmap = GetScreenBitmap(area);
            try
            {
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool[] GetScreenIsRelativeDark(params ScreenMonitorAna[] anas)
        {
            float scaling = PrimaryScreen.ScaleX;
            List<bool> result = new List<bool>();
            Drawing.Bitmap bitmap = GetScreenBitmap();
            foreach (var item in anas)
            {
                int tresult = 0;
                List<double> Xs = new List<double>();
                for (int i = 0; i < item.Xnum; i++)
                {
                    Xs.Add((item.Area.Left + (i / (double)(item.Xnum - 1)) * item.Area.Width) * scaling);
                }
                List<double> Ys = new List<double>();
                for (int i = 0; i < item.Ynum; i++)
                {
                    Ys.Add((item.Area.Top + (i / (double)(item.Ynum - 1)) * item.Area.Height) * scaling);
                }
                for (int i = 0; i < item.Xnum; i++)
                {
                    for (int j = 0; j < item.Ynum; j++)
                    {
                        /*奇怪的问题,不能等于宽度*/
                        if (Xs[i] >= bitmap.Width)
                        {
                            Xs[i] = bitmap.Width - 1;
                        }
                        else if (Xs[i] < 0)
                        {
                            Xs[i] = 0;
                        }
                        if (Ys[j] >= bitmap.Height)
                        {
                            Ys[j] = bitmap.Height - 1;
                        }
                        else if (Ys[j] < 0)
                        {
                            Ys[j] = 0;
                        }
                        Drawing.Color color = bitmap.GetPixel((int)Xs[i], (int)Ys[j]);
                        if (color.R + color.G + color.B > item.Middlevalue)
                        {
                            //Console.WriteLine("x:{0} y:{1}", Xs[i], Ys[i]);
                        }
                        else
                        {
                            tresult++;
                        }
                    }
                }
                if (tresult > item.Xnum * item.Ynum / 2.0)
                {
                    result.Add(true);
                }
                else
                {
                    result.Add(false);
                }
            }
            return result.ToArray();
        }
    }
    /// <summary>
    /// 屏幕检测的单个属性,只能由 ScreenMonitor 注册而来.
    /// </summary>
    public class ScreenMonitorProperty:INotifyPropertyChanged
    {
        bool? isRelativeDark = null;
        Window window;

        public Window Window { get => window; }
        public bool IsRelativeDark { get => (bool)isRelativeDark; }
        internal bool _IsRelativeDark
        {
            set
            {
                if (isRelativeDark == null || (bool)isRelativeDark != value)
                {
                    isRelativeDark = value;
                    IsRelativeDarkChanged?.Invoke(this, new IsRelativeDarkChangedEventargs(window, value));
                    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs("IsRelativeDark"));
                }
                else
                {
                    isRelativeDark = value;
                }
            }
        }
        public event EventHandler<IsRelativeDarkChangedEventargs> IsRelativeDarkChanged;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        internal ScreenMonitorProperty(Window window)
        {
            this.window = window;
        }

    }
    /// <summary>
    /// 当背景的亮色或暗色发生改变时触发的数据.
    /// </summary>
    public class IsRelativeDarkChangedEventargs
    {
        Window window;
        bool isRelativeDark;

        public IsRelativeDarkChangedEventargs(Window window, bool isRelativeDark)
        {
            this.window = window;
            this.isRelativeDark = isRelativeDark;
        }
        /// <summary>
        /// 检测的窗体引用.
        /// </summary>
        public Window Window => window;
        /// <summary>
        /// 是否是相对暗的.
        /// </summary>
        public bool IsRelativeDark => isRelativeDark;
    }

    /// <summary>
    /// Win32API 可以获取Dpi和缩放比率.
    /// </summary>
    public static class PrimaryScreen
    {
        #region Win32 API  
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
        IntPtr hdc, // handle to DC  
        int nIndex // index of capability  
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        #endregion
        #region DeviceCaps常量  
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;
        #endregion

        #region 属性  
        /// <summary>  
        /// 获取屏幕分辨率当前物理大小  
        /// </summary>  
        public static Size WorkingArea
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                Size size = new Size
                {
                    Width = GetDeviceCaps(hdc, HORZRES),
                    Height = GetDeviceCaps(hdc, VERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }
        /// <summary>  
        /// 当前系统DPI_X 大小 一般为96  
        /// </summary>  
        public static int DpiX
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int DpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                ReleaseDC(IntPtr.Zero, hdc);
                return DpiX;
            }
        }
        /// <summary>  
        /// 当前系统DPI_Y 大小 一般为96  
        /// </summary>  
        public static int DpiY
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int DpiX = GetDeviceCaps(hdc, LOGPIXELSY);
                ReleaseDC(IntPtr.Zero, hdc);
                return DpiX;
            }
        }
        /// <summary>  
        /// 获取真实设置的桌面分辨率大小  
        /// </summary>  
        public static Size DESKTOP
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                Size size = new Size
                {
                    Width = GetDeviceCaps(hdc, DESKTOPHORZRES),
                    Height = GetDeviceCaps(hdc, DESKTOPVERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>  
        /// 获取宽度缩放百分比  
        /// </summary>  
        public static float ScaleX
        {
            //get
            //{
            //    IntPtr hdc = GetDC(IntPtr.Zero);
            //    int t = GetDeviceCaps(hdc, DESKTOPHORZRES);
            //    int d = GetDeviceCaps(hdc, HORZRES);
            //    float ScaleX = (float)GetDeviceCaps(hdc, DESKTOPHORZRES) / (float)GetDeviceCaps(hdc, HORZRES);
            //    ReleaseDC(IntPtr.Zero, hdc);
            //    if (DESKTOP.Width == 3840)
            //    {
            //        return ScaleX * 3.0f;
            //    }
            //    else
            //    {
            //        return ScaleX;
            //    }
            //}
            get
            {
                return (float)( DESKTOP.Width / SystemParameters.PrimaryScreenWidth);
            }
        }
        /// <summary>  
        /// 获取高度缩放百分比  
        /// </summary>  
        public static float ScaleY
        {
            //get
            //{
            //    IntPtr hdc = GetDC(IntPtr.Zero);
            //    float ScaleY = (float)(float)GetDeviceCaps(hdc, DESKTOPVERTRES) / (float)GetDeviceCaps(hdc, VERTRES);
            //    ReleaseDC(IntPtr.Zero, hdc);
            //    return ScaleX;
            //}
            get
            {
                return (float)( DESKTOP.Height / SystemParameters.PrimaryScreenHeight);
            }
        }
        #endregion
    }
}
