using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace User.SoftWare
{
    /// <summary>
    /// 数据记录器[静态方法].
    /// </summary>
    public static class ULogger
    {
        static string folder = AppDomain.CurrentDomain.BaseDirectory;
        static ULoggerRecorder innerloggerrecorder = new ULoggerRecorder(Folder);

        public static string Folder
        {
            get => folder;
            set
            {
                if (folder != value)
                {
                    folder = value;
                    innerloggerrecorder = new ULoggerRecorder(value);
                }
            }
        }
        public static void WriteException(Exception ex)
        {
            innerloggerrecorder.Write(ex);
        }
    }
    /// <summary>
    /// 数据记录器.
    /// </summary>
    public class ULoggerRecorder : XmlBase
    {
        protected override string Comment => "这是用于记录错误信息的文件";
        protected override string FileType => "logger";
        protected override string FileVersion => "1.0.0.0";

        public ULoggerRecorder(string folder)
        {
            base.Folder = folder;
            base.RootName = "Logger";
        }

        public void Write(Exception ex)
        {
            if (!File.Exists(FilePath))
            {
                CreateXml();
                Write(ex);
            }
            else
            {
                XDocument xDocument = XDocument.Load(FilePath);
                xDocument.Root.Add(ConvertToXElement(ex));
                xDocument.Save(FilePath);
            }
        }
        public void Write(params object[] message)
        {
            if (!File.Exists(FilePath))
            {
                CreateXml();
                Write(message);
            }
            else
            {
                XDocument xDocument = XDocument.Load(FilePath);
                xDocument.Root.Add(ConvertToXElement(message));
                xDocument.Save(FilePath);
            }
        }

        private XElement ConvertToXElement(Exception ex)
        {
            List<XAttribute> contents = new List<XAttribute>
            {
                new XAttribute("type", ex.GetType().ToString())
            };
            if (ex.Message != null)
                contents.Add(new XAttribute("message", ex.Message));
            contents.Add(new XAttribute("time", DateTime.Now.ToString()));
            if (ex.HelpLink != null)
                contents.Add(new XAttribute("helplink", ex.HelpLink));
            if (ex.Source != null)
                contents.Add(new XAttribute("source", ex.Source));
            if (ex.StackTrace != null)
                contents.Add(new XAttribute("stacktrace", ex.StackTrace));
            if (ex.InnerException != null)
            {
                return new XElement("logger",
                    contents.ToArray(), ConvertToXElement(ex.InnerException)
                    );
            }
            else
            {
                return new XElement("logger",
                    contents.ToArray()
                    );
            }

        }
        private XElement ConvertToXElement(object[] message)
        {
            List<XElement> elements = new List<XElement>();
            foreach (var item in message)
            {
                elements.Add(new XElement("add", new XAttribute("type", item.GetType().ToString()), item.ToString()));
            }
            return new XElement("message", elements.ToArray()
                );
        }
        private ExceptionInfo ConvertToValue(XElement element)
        {
            Dictionary<string, string> contents = new Dictionary<string, string>
            {
                { "message", element.Attribute("message").Value }
            };
            throw new NotImplementedException();
        }
    }
    public class ExceptionInfo
    {
        DateTime time;
        string message;
        string type;
        string helplink;
        string source;
        string stackTrace;
        ExceptionInfo innerException;

        public ExceptionInfo(DateTime time, string message, string type, string helplink, string source, string stackTrace)
        {
            this.time = time;
            this.message = message;
            this.type = type;
            this.helplink = helplink;
            this.source = source;
            this.stackTrace = stackTrace;
        }

        public DateTime Time => time;
        public string Message => message;
        public string Type => type;
        public string Helplink => helplink;
        public string Source => source;
        public string StackTrace => stackTrace;
        public ExceptionInfo InnerException => innerException;
    }
    public class ExceptionInfoCollection : IReadOnlyCollection<ExceptionInfo>
    {
        List<ExceptionInfo> list;
        public ExceptionInfoCollection(ExceptionInfo[] info)
        {
            list = info.ToList();
        }
        public int Count => list.Count;
        public IEnumerator<ExceptionInfo> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
