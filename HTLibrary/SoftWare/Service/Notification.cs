using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Collections;
using System.IO;
using User.HTStudioService;
using System.Collections.ObjectModel;

namespace User.SoftWare.Service
{
    public class Notification : XmlBase, ICollection<NotificationInfo>
    {
        List<NotificationInfo> list = new List<NotificationInfo>();
        HTStudioService.HTStudioService service = new HTStudioService.HTStudioService();

        public Notification(string folder, string rootName)
        {
            Folder = folder;
            RootName = rootName;
        }
        protected override string Comment => "应用于软件的通知储存文件.";
        protected override string FileType => "notification";
        protected override string FileVersion => "1.0.0.0";

        public DateTime LastTime { get
            {
                if (list.Any())
                {
                    return (from item in list orderby item.DateTime descending select item.DateTime).First();
                }
                else
                {
                    return new DateTime();
                }
            } }
        public int Count => list.Count;
        public bool IsReadOnly => false;

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
            ItemAdded?.Invoke(this, info);
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
        public bool Remove(NotificationInfo info)
        {
            if (File.Exists(FilePath))
            {
                XDocument xDocument = XDocument.Load(FilePath);
                try
                {
                    var ns = from item in xDocument.Root.Elements()
                             where item.Name == "add" ||
                             (item.Attribute("DateTime").Value == info.DateTime.ToString()
                             || item.Attribute("Title").Value == info.Title
                             || item.Attribute("Description").Value == info.Description
                             || item.Attribute("Button").Value == info.Button
                             || item.Attribute("ButtonEvent").Value == info.ButtonEvent)
                             select item;
                    ns.Remove();
                    xDocument.Save(FilePath);
                    list.Remove(info);
                    ItemRemoved?.Invoke(this, new EventArgs());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// 从<see cref="HTStudioService.HTStudioService"/>中下载更新,并写入通知.
        /// </summary>
        /// <param name="softWareName"></param>
        /// <param name="lastTime"></param>
        /// <returns></returns>
        public bool DownloadNew(string softWareName,DateTime lastTime,out int count)
        {
            try
            {
                var notices = service.GetNotificationInfos(softWareName, lastTime, true);
                foreach (var item in notices)
                {
                    Add(item);
                }
                count = notices.Length;
                return true;
            }
            catch (Exception)
            {
                count = 0;
                return false;
            }
        }
        /// <summary>
        /// 由<see cref="HTStudioService.HTStudioService"/>转发通知.
        /// </summary>
        /// <param name="softWareName"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Apply(string softWareName, NotificationInfo info)
        {
            try
            {
                service.ApplyNotification(softWareName, info, out bool a, out bool b);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
        public void Clear()
        {
            CreateXml();
            list.Clear();
        }
        public bool Contains(NotificationInfo item)
        {
            return list.Contains(item);
        }
        void ICollection<NotificationInfo>.CopyTo(NotificationInfo[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
        bool ICollection<NotificationInfo>.Remove(NotificationInfo item)
        {
            return Remove(item);
        }
        public IEnumerator<NotificationInfo> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public event EventHandler<NotificationInfo> ItemAdded;
        public event EventHandler ItemRemoved;

    }
}
