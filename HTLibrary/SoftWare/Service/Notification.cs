using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Collections;
using System.IO;

namespace User.SoftWare.Service
{
    public class Notification : XmlBase, IEnumerable<NotificationInfo>
    {
        List<NotificationInfo> list = new List<NotificationInfo>();
        public Notification(string folder,string rootName)
        {
            Folder = folder;
            RootName = rootName;
        }
        protected override string Comment => "应用于软件的通知储存文件.";
        protected override string FileType => "notification";
        protected override string FileVersion => "1.0.0.0";
        /// <summary>
        /// 将从<see cref="Path"/>中加载Notification,这仅用于初始化.
        /// </summary>
        public void Load()
        {
            if (File.Exists(FilePath))
            {
                XDocument xDocument = XDocument.Load(FilePath);
                try
                {
                    var ns = from item in xDocument.Root.Elements()
                             where item.Name == "add"
                             select new NotificationInfo
                             {
                                 DateTime = DateTime.Parse(item.Attribute("DateTime").Value),
                                 Title = item.Attribute("Title").Value,
                                 Description = item.Attribute("Description").Value,
                                 Button = item.Attribute("Button").Value,
                                 ButtonEvent = item.Attribute("ButtonEvent").Value
                             };
                    list = ns.ToList();
                }
                catch (Exception)
                {
                }
            }
        }
        /// <summary>
        /// 将通知加入list,并且立即写入文件.
        /// </summary>
        /// <param name="info"></param>
        public void Add(NotificationInfo info)
        {
            list.Add(info);
            ListAdded?.Invoke(this, info);
            var xElement = new XElement("add", new XAttribute("DateTime", info.DateTime.ToString()),
                    new XAttribute("Title", info.Title), new XAttribute("Description", info.Description),
                    new XAttribute("Button", info.Button), new XAttribute("ButtonEvent", info.ButtonEvent));
            if (!File.Exists(FilePath))
            {
                CreateXml();
            }
            XDocument xDocument = XDocument.Load(FilePath);
            xDocument.Root.Add(xElement);
            xDocument.Save(FilePath);
        }
        public bool TryGetValue(string title, out NotificationInfo[] infos)
        {
            var x = from item in list where item.Title == title orderby item.DateTime select item;
            if (x.Count() == 0)
            {
                infos = null;
                return false;
            }
            else
            {
                infos = x.ToArray();
                return true;
            }
        }
        public event EventHandler<NotificationInfo> ListAdded;

        public IEnumerator<NotificationInfo> GetEnumerator()
        {
            return ((IEnumerable<NotificationInfo>)list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<NotificationInfo>)list).GetEnumerator();
        }
    }

}
