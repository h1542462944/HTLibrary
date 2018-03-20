using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace User.SoftWare
{
    /// <summary>
    /// 支持USettings转换[数据集合的方式.]
    /// </summary>
    public interface IUSettingsConvertArray
    {
        IUSettingsConvertArray USettingsConvertArray(object[] contents);
        object[] USettingsConvertArray();
    }
    /// <summary>
    /// 支持USettings转换[字符串方式.]
    /// 注意:支持转换的类必须声明为public,重写ToString,并具有默认构造函数.
    /// </summary>
    public interface IUSettingsConvert
    {
        dynamic USettingsConvert(string content);
        string ToString();
    }
    /// <summary>
    /// 标准的USettings[版本:1.0.3.0],支持数组.
    /// </summary>
    public sealed class USettings : XmlBase
    {
        /// <summary>
        /// 支持转换的类型, 0:可以直接转换,1:需要借助Parse,2:需要自定义转换方法,3:使用ISettingsConvert接口.
        /// </summary>
        static Dictionary<Type, int> SettingsType = new Dictionary<Type, int>
        {
            {typeof(byte),0 },
            {typeof(sbyte),0},
            {typeof(short),0 },
            {typeof(int) ,0},
            {typeof(long),0},
            {typeof(ushort),0},
            {typeof(uint),0 },
            {typeof(ulong),0},
            {typeof(float),0 },
            {typeof(double),0},
            {typeof(char),0},
            {typeof(string),0 },
            {typeof(decimal),0},
            {typeof(bool),0 },
            {typeof(DateTime),0},
            {typeof(System.Windows.Point),1},
            {typeof(System.Windows.Size),1},
            {typeof(System.Windows.Media.Color),2}
        };
        /// <summary>
        /// 依赖的单个设置数据对象.
        /// </summary>
        class USettingInfo
        {
            string name;
            object value;
            Type type;
            bool isLoaded;
            bool isEvent;

            public USettingInfo(string name, object value, Type type, bool isEvent)
            {
                this.name = name;
                this.value = value;
                this.type = type;
                this.isEvent = isEvent;
            }

            public string Name { get => name; }
            public object Value { get => value; }
            public Type Type { get => type; }
            internal bool IsLoaded { get => isLoaded; set => isLoaded = value; }
            internal bool IsEvent { get => isEvent; }

            public void Replace(object value)
            {
                this.value = value;
                this.isLoaded = true;
            }
        }
        /// <summary>
        /// 新建设置实例.
        /// </summary>
        /// <param name="folder">所在的文件夹名称[完全限定名]</param>
        /// <param name="rootName">根命名及文件名</param>
        public USettings(string folder, string rootName,bool isMonitor= false)
        {
            Folder = folder;
            RootName = rootName;
            IsMonitor = isMonitor;
            if (IsMonitor && File.Exists(base.FilePath))
            {
                //>>>>新建Watcher
                Watcher = new FileSystemWatcher(Folder, "*.xml");
                Watcher.Changed += Watcher_Changed;
                Watcher.EnableRaisingEvents = true;
            }
        }
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed && e.FullPath == FilePath)
            {
                //do something.
                foreach (var v in content.Values)
                {
                    v.IsLoaded = false;
                    //>>>>引发读取操作.
                    object obj = this[v.Name];
                }
            }
        }
        private readonly string filetype = "settings";
        private readonly string comment = "这是设置1.0.3.0版本的本地文件,可以实现改变字段值自动保存的功能,支持保存数组.";
        Dictionary<string, USettingInfo> content = new Dictionary<string, USettingInfo>();
        Dictionary<string, USettingInfo> contentdefault = new Dictionary<string, USettingInfo>();
        protected override string Comment => comment;
        protected override string FileType => filetype;
        protected override string FileVersion => "1.0.3.0";
        FileSystemWatcher Watcher { get; set; }
        bool IsMonitor { get; set; }
        /// <summary>
        /// 设置的集合.
        /// </summary>
        public USettingsPropertyCollection Properties
        {
            get
            {
                Dictionary<string, USettingsProperty> e = new Dictionary<string, USettingsProperty>();
                foreach (var item in content)
                {
                    e.Add(item.Key, new USettingsProperty(this, item.Key));
                }
                return new USettingsPropertyCollection(e.Values.ToArray());
            }
        }
        /// <summary>
        /// 只读设置键值集合,不会触发事件.
        /// </summary>
        public Dictionary<string, object> ReadonlyProperties
        {
            get
            {
                Dictionary<string, object> e = new Dictionary<string, object>();
                foreach (var item in content)
                {
                    e.Add(item.Key, GetValue(item.Value));
                }
                return e;
            }
        }

        /// <summary>
        /// 通知设置值以改变或者初始化.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event USettingsChangedEventHander USettingsChanged;
        /// <summary>
        /// 注册一个设置对象.
        /// </summary>
        /// <typeparam name="T">设置的类型.</typeparam>
        /// <param name="name">设置的名称.</param>
        /// <param name="value">设置的值.</param>
        /// <returns></returns>
        public USettingsProperty<T> Register<T>(string name, T value, bool isEvent = false)
        {
            if (content.TryGetValue(name, out USettingInfo arg))
            {
                return null;
            }
            else
            {
                content.Add(name, new USettingInfo(name, value, value.GetType(), isEvent));
                contentdefault.Add(name, new USettingInfo(name, value, value.GetType(), isEvent));
                return new USettingsProperty<T>(this, name);
            }
        }

        /// <summary>
        /// 通过设置的键来查询设置值.
        /// </summary>
        /// <param name="name">设置的键</param>
        /// <returns></returns>
        internal object this[string name]
        {
            get
            {
                if (content.TryGetValue(name, out USettingInfo arg))
                {
                    if (arg.IsLoaded == false)
                    {
                        var oldvalue = arg.Value;
                        var value = GetValue(arg);
                        arg.Replace(value);
                        if (arg.IsEvent)
                        {
                            USettingsChanged?.Invoke(new USettingsProperty(this, name), new PropertyChangedEventargs(oldvalue, value));
                        }
                        return value;
                    }
                    else
                    {
                        arg.IsLoaded = true;
                        return arg.Value;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("不存在的属性.");
                }
            }
            set
            {
                if (Watcher !=null)
                {
                    Watcher.EnableRaisingEvents = false;
                }
                if (content.TryGetValue(name, out USettingInfo arg))
                {
                    object oldvalue = arg.Value;
                    arg.Replace(value);
                    SetValue(arg);
                    if (arg.IsEvent)
                    {
                        USettingsChanged?.Invoke(new USettingsProperty(this, name), new PropertyChangedEventargs(oldvalue, value));
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }
                if (Watcher != null)
                {
                    Watcher.EnableRaisingEvents = true;
                }
            }
        }
        /// <summary>
        /// 对已注册的设置进行读取操作,将触发[USettingsChanged⚡]
        /// </summary>
        public void Flush()
        {
            foreach (var item in content)
            {
                object t = this[item.Key];
            }
        }
        /// <summary>
        /// 对已注册的设置的值进行初始化,并写入文件,将触发[USettingsChanged⚡].
        /// </summary>
        public void ReSet(bool writeinfile = false)
        {
            foreach (var item in content)
            {
                string key = item.Key;
                this[key] = contentdefault[key].Value;
            }
            if (!writeinfile)
            {
                CreateXml();
            }
        }
        /// <summary>
        /// 对已注册的设置的某一项设置进行初始化,并写入文件,将触发[USettingsChanged⚡].
        /// </summary>
        /// <param name="name">设置的键</param>
        internal void ReSet(string name, bool writeinfile = false)
        {
            this[name] = contentdefault[name].Value;
            if (!writeinfile)
            {
                XDocument xDocument = XDocument.Load(FilePath);
                xDocument.Root.Element(name).Remove();
            }
        }
        /// <summary>
        /// 从文件中读取数据并转化为object[T:type]类型.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private object GetValue(USettingInfo info)
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    XElement eElement = XDocument.Load(FilePath).Root.Element(info.Name);
                    return ConvertToValue(eElement, info.Type, info.Name);
                }
                catch (OverflowException)
                {
                    throw;
                }
                catch (Exception)
                {
                    return info.Value;
                }
            }
            else
            {
                return info.Value;
            }
        }
        /// <summary>
        ///将数据转化为string类型并写入文件.
        /// </summary>
        /// <param name="info"></param>
        private void SetValue(USettingInfo info)
        {
            if (!File.Exists(FilePath))
            {
                CreateXml();
                //>>>>新建Watcher
                Watcher = null;
                if (IsMonitor)
                {
                    Watcher = new FileSystemWatcher(Folder, "*.xml");
                    Watcher.Changed += Watcher_Changed;
                    Watcher.EnableRaisingEvents = true;
                }
                SetValue(info);
            }
            else
            {
                XElement element = null;
                try
                {
                    element = ConvertToXElement(info.Value, info.Type, info.Name);
                }
                catch (OverflowException)
                {
                    throw;
                }
                catch (Exception)
                { }
                Task.Run(() =>
                {
                    XDocument xDocument = null;
                    bool isreaded = false;
                    while (!isreaded)
                    {
                        try
                        {
                            xDocument = XDocument.Load(FilePath);
                            xDocument.Root.Elements(info.Name).Remove();
                            xDocument.Root.Add(element);
                            isreaded = true;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(100);
                        }
                    }
                    bool issaved = false;
                    while (!issaved)
                    {
                        try
                        {
                            xDocument.Save(FilePath);
                            issaved = true;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                );

            }
        }
        /// <summary>
        /// 将 string 转化为 object[T:type]类型.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object Transfer(string value, Type type)
        {
            try
            {
                if (SettingsType.TryGetValue(type, out int settingstype))
                {
                    try
                    {
                        if (settingstype == 0)
                        {
                            object t = Convert.ChangeType(value, type);
                            return t;
                        }
                        else
                        {
                            if (type == typeof(System.Windows.Point))
                            {
                                return System.Windows.Point.Parse(value);
                            }
                            else if (type == typeof(System.Windows.Size))
                            {
                                return System.Windows.Size.Parse(value);
                            }
                            else
                            {
                                string[] ts = value.Split(',');
                                return System.Windows.Media.Color.FromArgb(byte.Parse(ts[0]), byte.Parse(ts[1]), byte.Parse(ts[2]), byte.Parse(ts[3]));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else if (type.GetInterface(typeof(IUSettingsConvert).ToString()) != null)
                {
                    IUSettingsConvert t = (IUSettingsConvert)Activator.CreateInstance(type, null);
                    dynamic result = t.USettingsConvert(value);
                    return result;
                }
                else
                {
                    throw new OverflowException("无法实现的转换.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private string Transfer(object value, Type type)
        {
            if (SettingsType.TryGetValue(type, out int settingstype))
            {
                string result = "";
                if (settingstype == 2)
                {
                    if (type == typeof(System.Windows.Media.Color))
                    {
                        System.Windows.Media.Color color = (System.Windows.Media.Color)value;
                        result = color.A + "," + color.R + "," + color.G + "," + color.B;
                    }
                }
                else
                {
                    result = value.ToString();
                }
                return result;
            }
            else if (type.GetInterface(typeof(IUSettingsConvert).ToString()) != null)
            {
                return value.ToString();
            }
            else
            {
                throw new OverflowException("无法实现的转换.");
            }
        }
        object ConvertToValue(XElement content, Type type, string name = "add")
        {
            if (type.IsArray)
            {
                var elements = content.Elements();
                Type listtype = typeof(List<>);
                Type membertype = type.GetMemberType();
                listtype = listtype.MakeGenericType(membertype);
                dynamic list = Activator.CreateInstance(listtype, null);
                foreach (var item in elements)
                {
                    //将其解析为动态对象.
                    dynamic t = ConvertToValue(item, membertype);
                    list.Add(t);
                }
                return list.ToArray();
            }
            else if (IsUSettingsConvertArray(type))
            {
                IUSettingsConvertArray t = (IUSettingsConvertArray)Activator.CreateInstance(type, null);
                List<object> list = new List<object>();
                foreach (var item in content.Elements())
                {
                    list.Add(Transfer(item.Value, Type.GetType(item.Attribute("type").Value)));
                }
                return t.USettingsConvertArray(list.ToArray());
            }
            else if (IsDefaultConvert(type))
            {
                return Transfer(content.Value, type);
            }
            else
            {
                throw new OverflowException();
            }
        }
        XElement ConvertToXElement(object value, Type type, string name = "add")
        {
            if (type.IsArray)
            {
                List<XElement> listelement = new List<XElement>();
                foreach (var item in (Array)value)
                {
                    listelement.Add(ConvertToXElement(item, item.GetType()));
                }
                return new XElement(name, new XAttribute("type", type.ToString()), listelement.ToArray());
            }
            else if (IsUSettingsConvertArray(type))
            {
                IUSettingsConvertArray t = (IUSettingsConvertArray)value;
                object[] args = t.USettingsConvertArray();
                List<XElement> list = new List<XElement>();
                foreach (var item in args)
                {
                    list.Add(
                         ConvertToXElement(item, item.GetType())
                        );
                }
                return new XElement(name, new XAttribute("type", type.ToString()), list.ToArray());
            }
            else if (IsDefaultConvert(type))
            {
                return new XElement(name, new XAttribute("type", type.ToString()), Transfer(value, type));
            }
            else
            {
                throw new OverflowException();
            }
        }

        private bool IsUSettingsConvertArray(Type type)
        {
            if (type.GetInterface(typeof(IUSettingsConvertArray).ToString()) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsDefaultConvert(Type type)
        {
            if (SettingsType.ContainsKey(type))
            {
                return true;
            }
            else if (type.GetInterface(typeof(IUSettingsConvert).ToString()) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public sealed class USettingsProperty : IEquatable<USettingsProperty>
    {
        internal USettings linkuSettings;
        string name;

        internal USettingsProperty(USettings linkuSettings, string name)
        {
            this.linkuSettings = linkuSettings;
            this.name = name;
        }

        /// <summary>
        /// 设置的名称.
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// 设置的值.
        /// </summary>
        public object Value { get => linkuSettings[name]; set => linkuSettings[name] = value; }
        public void ReSet(bool writeinfile = false)
        {
            linkuSettings.ReSet(name, writeinfile);
        }
        public void Take()
        {
            Value = Value;
        }

        public bool Equals(USettingsProperty other)
        {
            return linkuSettings == other.linkuSettings && name == other.name;
        }
        public static bool operator ==(USettingsProperty v1, USettingsProperty v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(USettingsProperty v1, USettingsProperty v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            try
            {
                USettingsProperty arg = (USettingsProperty)obj;
                if (this.Equals(arg))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public sealed class USettingsProperty<T>
    {
        internal USettings linkuSettings;
        string name;

        internal USettingsProperty(USettings linkuSettings, string name)
        {
            this.linkuSettings = linkuSettings;
            this.name = name;
        }

        /// <summary>
        /// 设置的名称.
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// 设置的值.
        /// </summary>
        public T Value { get => (T)linkuSettings[name]; set => linkuSettings[name] = value; }
        public void ReSet(bool writeinfile = false)
        {
            linkuSettings.ReSet(name, writeinfile);
        }
        public void Take()
        {
            Value = Value;
        }

        public static implicit operator USettingsProperty(USettingsProperty<T> arg)
        {
            return new USettingsProperty(arg.linkuSettings, arg.name);
        }
    }

    public sealed class USettingsPropertyCollection : IReadOnlyCollection<USettingsProperty>
    {
        private Dictionary<string, USettingsProperty> properties = new Dictionary<string, USettingsProperty>();
        internal USettingsPropertyCollection(USettingsProperty[] properties)
        {
            foreach (var item in properties)
            {
                this.properties.Add(item.Name, item);
            }
        }
        public USettingsProperty this[string name] => properties[name];
        public int Count => properties.Count;
        public IEnumerator<USettingsProperty> GetEnumerator()
        {
            return properties.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.Values.GetEnumerator();
        }

    }
    public delegate void USettingsChangedEventHander(USettingsProperty key, PropertyChangedEventargs e);
}
