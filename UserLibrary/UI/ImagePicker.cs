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

namespace User.UI
{
    /// <summary>
    /// 实现截图器的辅助类.
    /// </summary>
    public static class ImagePicker
    {
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

        public static bool[] GetScreenIsRelativeDark(params ImagePickerAna[] anas)
        {
            List<bool> result = new List<bool>();
            Drawing.Bitmap bitmap = GetScreenBitmap();
            foreach (var item in anas)
            {
                int tresult = 0;
                List<double> Xs = new List<double>();
                for (int i = 0; i < item.Xnum; i++)
                {
                    Xs.Add(item.Area.Left + (i / (double)(item.Xnum - 1)) * item.Area.Width);
                }
                List<double> Ys = new List<double>();
                for (int i = 0; i < item.Ynum; i++)
                {
                    Ys.Add(item.Area.Top + (i / (double)(item.Ynum - 1)) * item.Area.Height);
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
                        else if (Xs[i] <0)
                        {
                            Xs[i] = 0;
                        }
                        if (Ys[j] >= bitmap.Height)
                        {
                            Ys[j] = bitmap.Height - 1;
                        }
                        else if (Ys[j] <0)
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
    /// ImagePicker 和 ImagePickerMonitor的数据,注意:如果指定window则Area直接重定向至窗体的数据.
    /// </summary>
    public class ImagePickerAna
    {
        Rect area;
        int xnum;
        int ynum;
        int middlevalue;
        Window window;

        public ImagePickerAna(Rect area, int xnum, int ynum, int middlevalue = 230)
        {
            this.area = area;
            this.xnum = xnum;
            this.ynum = ynum;
            this.middlevalue = middlevalue;
            window = null;
        }
        public ImagePickerAna(Window window, int xnum, int ynum, int middlevalue = 230) : this(new Rect(window.Left, window.Top, window.Width, window.Height), xnum, ynum, middlevalue)
        {
           this.window = window;
        }
        public ImagePickerAna(Window window) : this(window, 3, 3)
        {
            this.window = window;
        }

        public Rect Area { get
            {
                if (window == null)
                {
                    return area;
                }
                else
                {
                    return new Rect(this.window.Left, this.window.Top, this.window.Width, this.window.Height);
                }
            }
            set => area = value; }
        public int Xnum { get => xnum; set => xnum = value; }
        public int Ynum { get => ynum; set => ynum = value; }
        public int Middlevalue { get => middlevalue; set => middlevalue = value; }
        public Window Window { get => window; set => window = value; }
    }
    /// <summary>
    /// 自动变色监视器(以集合,同步的方式进行),已内置了计时器.
    /// </summary>
    public class ImagePickerMonitor
    {
        private ImagePickerAna[] anas;
        /// <summary>
        ///0为初始值,1-5为活跃状态,6-13为休眠状态.
        /// </summary>
        private int tick;
        private bool[] prev;
        private DispatcherTimer innertimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1),
            IsEnabled = false
        };
        public event EventHandler<ImagePickerEventargs> IsRelativeDarkChanged;

        public ImagePickerMonitor(params ImagePickerAna[] anas)
        {
            this.anas = anas;
            this.tick = 0;
            this.prev = new bool[anas.Length];
            innertimer.Tick += Innertimer_Tick;
        }
        private void Innertimer_Tick(object sender, EventArgs e)
        {
            List<int> list = new List<int>();
            //Console.WriteLine(tick);
            if (tick <= 5 || tick == 13)
            {
                bool[] values = ImagePicker. GetScreenIsRelativeDark(anas);

                if (tick == 0)
                {
                    for (int i = 0; i < prev.Length; i++)
                    {
                        list.Add(i);
                    }
                    tick++;

                }
                else
                {
                    bool ischanged = false;
                    for (int i = 0; i < values.Length; i++)
                    {
                        //Console.WriteLine("{0} {1}",prev[i],values[i]);
                        if (prev[i] != values[i])
                        {
                            ischanged = true;
                            list.Add(i);
                        }
                    }
                    if (ischanged)
                    {
                        tick = 1;
                    }
                    else
                    {
                        if (tick == 13)
                        {
                            tick = 6;
                        }
                        else
                        {
                            tick++;
                        }
                        return;
                    }
                }
                prev = values;
                IsRelativeDarkChanged?.Invoke(this, new ImagePickerEventargs(list.ToArray(), values));
            }
            else
            {
                tick++;
            }
        }

        public bool IsEnabled { get => innertimer.IsEnabled; set => innertimer.IsEnabled = value; }
        public ImagePickerAna[] Anas => anas;
    }
    /// <summary>
    /// 自动变色监视器的数据.
    /// </summary>
    public struct ImagePickerEventargs
    {
        /// <summary>
        /// 发生改变的ImagePickerAna的索引集合,值由注册ImagePickerMonitor的数组决定.
        /// </summary>
        int[] indexs;
        /// <summary>
        /// 发生改变时,是否是暗色背景的集合,Length = 注册ImagePickerMonitor的数组长度.
        /// </summary>
        bool[] isRelativeDarks;

        public ImagePickerEventargs(int[] indexs, bool[] isRelativeDarks)
        {
            this.indexs = indexs;
            this.isRelativeDarks = isRelativeDarks;
        }

        public int[] Indexs => indexs;
        public bool[] IsRelativeDarks => isRelativeDarks;
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
                Size size = new Size();
                size.Width = GetDeviceCaps(hdc, HORZRES);
                size.Height = GetDeviceCaps(hdc, VERTRES);
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
                Size size = new Size();
                size.Width = GetDeviceCaps(hdc, DESKTOPHORZRES);
                size.Height = GetDeviceCaps(hdc, DESKTOPVERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>  
        /// 获取宽度缩放百分比  
        /// </summary>  
        public static float ScaleX
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int t = GetDeviceCaps(hdc, DESKTOPHORZRES);
                int d = GetDeviceCaps(hdc, HORZRES);
                float ScaleX = (float)GetDeviceCaps(hdc, DESKTOPHORZRES) / (float)GetDeviceCaps(hdc, HORZRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleX;
            }
        }
        /// <summary>  
        /// 获取高度缩放百分比  
        /// </summary>  
        public static float ScaleY
        {
            get
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                float ScaleY = (float)(float)GetDeviceCaps(hdc, DESKTOPVERTRES) / (float)GetDeviceCaps(hdc, VERTRES);
                ReleaseDC(IntPtr.Zero, hdc);
                return ScaleY;
            }
        }
        #endregion
    }
}
