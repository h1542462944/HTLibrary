using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Globalization;

namespace User
{
    public static class Tools
    {
        public static System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X / Windows.PrimaryScreen.ScaleX, point.Y / Windows.PrimaryScreen.ScaleY);
        }
        public static string ShortTimeStringInvoke(string hour, string minute)
        {
            return hour + ":" + minute;
        }
        public static bool IsShortTimeString(string value)
        {
            try
            {
                string[] s = value.Split(':');
                if (s.Length == 2)
                {
                    if (int.Parse(s[0]) >= 0 && int.Parse(s[0]) < 24)
                    {
                        if (int.Parse(s[1]) >= 0 && int.Parse(s[1]) < 60)
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public static double GetBi(byte v1, byte v2, byte vm)
        {
            return ((double)vm - (double)v1) / ((double)v2 - (double)v1);
        }
        public static DateTime GetDateTimeFromstring(string value)
        {
            int a0 = int.Parse(value.Substring(0, 4));
            int a1 = int.Parse(value.Substring(4, 2));
            int a2 = int.Parse(value.Substring(6, 2));
            return new DateTime(a0, a1, a2);
        }
        public static string Add0(string arg, int length)
        {
            return new string('0', length - arg.Length) + arg;
        }
        public static double Checkdouble(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
        public static double GetSlideValue(Border border, double offset)
        {
            return Checkdouble((Mouse.GetPosition(border).X - offset) / (border.ActualWidth - 2 * offset), 0, 1);
        }
        public static int GetSlideValueInt(double value, int min, int max)
        {
            return (int)(min + (max - min) * value);
        }
        public static bool IsDateTimeString(string arg)
        {
            try
            {
                string[] v = new string[] { arg.Substring(0, 4), arg.Substring(4, 2), arg.Substring(6, 2) };
                DateTime dateTime = new DateTime(int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public static class Extension
    {
       
        public static string GetDateString(this DateTime arg)
        {
            string[] ts = arg.ToShortDateString().Split('/');
            return string.Format("{0}{1}{2}", Tools.Add0(ts[0], 4), Tools.Add0(ts[1], 2), Tools.Add0(ts[2], 2));
        }
        public static string GetDayOfWeekString(this DateTime arg)
        {
            string[] dayOfWeekString = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return dayOfWeekString[(int)arg.DayOfWeek];
        }
        public static int WeekOfYear(this DateTime arg,CalendarWeekRule rule= CalendarWeekRule.FirstDay,DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
        {
            CultureInfo culture = new CultureInfo("zh-CN");
            System.Globalization.Calendar cal = culture.Calendar;
            return cal.GetWeekOfYear(arg, rule, firstDayOfWeek);
        }
        public static Type GetMemberType(this Type arg)
        {
            Type result = arg.GetElementType();
            return result;
        }
        public static void Save(this BitmapImage arg, string filepath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(arg));
            using (var fs = new FileStream(filepath, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

    }
    /// <summary>
    /// 基础Xml服务
    /// </summary>
    public abstract class XmlBase
    {
        private string fileName;

        /// <summary>
        /// 总注释.
        /// </summary>
        protected abstract string Comment { get; }
        /// <summary>
        /// 文件类型.
        /// </summary>
        protected abstract string FileType { get; }
        /// <summary>
        /// 文件版本.
        /// </summary>
        protected abstract string FileVersion { get; }
        protected string FileName
        {
            get
            {
                if (fileName == null || fileName == "")
                    return RootName;
                else return fileName;
            }
            set
            {
                fileName = value;
            }
        }
        /// <summary>
        /// 所在文件夹路径.
        /// </summary>
        protected string Folder { get; set; }
        /// <summary>
        /// 文件名及根命名空间.
        /// </summary>
        protected string RootName { get; set; }
        /// <summary>
        /// 文件路径.
        /// </summary>
        protected string FilePath => Folder + FileName + ".xml";
        /// <summary>
        /// 创建一个新的Xml.
        /// </summary>
        protected void CreateXml()
        {
            XDocument xDocument = new XDocument(
              new XComment(Comment),
              new XElement(RootName, new XAttribute("filetype", FileType), new XAttribute("version", FileVersion))
            );
            xDocument.Save(FilePath);
        }
    }
}
