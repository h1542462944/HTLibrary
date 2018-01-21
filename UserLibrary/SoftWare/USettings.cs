using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

[assembly: CLSCompliant(true)]
namespace User.SoftWare
{
    /// <summary>
    /// 支持USettings转换[数据集合的方式.]
    /// </summary>
    public interface IUSettingsConvertArray
    {
        object USettingsConvertArray(object[] contents);
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
    public sealed class USettings:XmlBase
    {
        /// <summary>
        /// 依赖的单个设置数据对象.
        /// </summary>
        class USettingInfo
        {
            string name;
            object value;
            Type type;
            bool isLoaded;

            public USettingInfo(string name, object value, Type type)
            {
                this.name = name;
                this.value = value;
                this.type = type;
            }

            public string Name { get => name; }
            public object Value { get => value; }
            public Type Type { get => type; }
            internal bool IsLoaded { get => isLoaded; }

            public void Replace(object value)
            {
                this.value = value;
                this.isLoaded = true;
            }
            public void SetIsLoaded()
            {
                isLoaded = true;
            }
        }
        /// <summary>
        /// 新建设置实例.
        /// </summary>
        /// <param name="folder">所在的文件夹名称[完全限定名]</param>
        /// <param name="rootName">根命名及文件名</param>
        public USettings(string folder, string rootName)
        {
            Folder = folder;
            RootName = rootName;
        }
        private readonly string filetype = "settings";
        private readonly string comment = "这是设置1.0.3.0版本的本地文件,可以实现改变字段值自动保存的功能,支持保存数组.";
        Dictionary<string, USettingInfo> content = new Dictionary<string, USettingInfo>();
        Dictionary<string, USettingInfo> contentdefault = new Dictionary<string, USettingInfo>();
        protected override string Comment => comment;
        protected override string FileType => filetype;
        protected override string FileVersion => "1.0.3.0";
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
        public USettingsProperty<T> Register<T>(string name, T value)
        {
            if (content.TryGetValue(name, out USettingInfo arg))
            {
                return null;
            }
            else
            {
                content.Add(name, new USettingInfo(name, value, value.GetType()));
                contentdefault.Add(name, new USettingInfo(name, value, value.GetType()));
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
                        USettingsChanged?.Invoke(new USettingsKey(this,name), new USettingsChangedEventargs(oldvalue, value));
                        return value;
                    }
                    else
                    {
                        arg.SetIsLoaded();
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
                if (content.TryGetValue(name, out USettingInfo arg))
                {
                    object oldvalue = arg.Value;
                    arg.Replace(value);
                    SetValue(arg);
                    USettingsChanged?.Invoke(new USettingsKey(this,name), new USettingsChangedEventargs(oldvalue, value));
                }
                else
                {
                    throw new ArgumentNullException();
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
        internal void ReSet(string name,bool writeinfile = false)
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
                if (Tools.SettingsType.TryGetValue(type, out int settingstype))
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
            if (Tools.SettingsType.TryGetValue(type, out int settingstype))
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
            else if(IsDefaultConvert(type))
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
            if (Tools.SettingsType.ContainsKey(type))
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
    /// <summary>
    /// 设置改变或初始化加载时触发的事件数据.
    /// </summary>
    public sealed class USettingsChangedEventargs:EventArgs
    {
        object oldValue;
        object newValue;

        public USettingsChangedEventargs(object oldValue, object newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        /// <summary>
        /// 设置改变前的值.
        /// </summary>
        public object OldValue => oldValue;
        /// <summary>
        /// 设置改变后的值.
        /// </summary>
        public object NewValue => newValue;
        /// <summary>
        ///是否是初始化的设置.
        /// </summary>
        public bool IsNewest => OldValue == null;



    }

    public sealed class USettingsKey:IEquatable<USettingsKey>,IEquatable<USettingsProperty>
    {
        internal USettings linkuSettings;
        string name;

        internal USettingsKey(USettings linkuSettings, string name)
        {
            this.linkuSettings = linkuSettings;
            this.name = name;
        }

        public string Name { get => name;  }

        public bool Equals(USettingsProperty other)
        {
            return (linkuSettings == other.linkuSettings && name == other.Name);
     
        }
        public bool Equals(USettingsKey other)
        {
            return linkuSettings == other.linkuSettings && name == other.name;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(USettingsKey))
            {
                return this == (USettingsKey)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty))
            {
                return this == (USettingsProperty)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty))
            {
                return obj.Equals(this);
            }
            else { return false; }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(USettingsKey v1, USettingsProperty v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(USettingsKey v1, USettingsProperty v2)
        {
            return !(v1 == v2);
        }
        public static bool operator ==(USettingsKey v1, USettingsKey v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(USettingsKey v1, USettingsKey v2)
        {
            return !(v1 == v2);
        }
    }
    public sealed class USettingsProperty:IEquatable<USettingsKey>, IEquatable<USettingsProperty>
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
        public string Name { get => name;  }
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
        public bool Equals(USettingsKey other)
        {
            return this.linkuSettings == other.linkuSettings && name == other.Name;
        }

        public static bool operator ==(USettingsProperty v2, USettingsKey v1)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=(USettingsProperty v2, USettingsKey v1)
        {
            return !(v2 == v1);
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
            if (obj.GetType() == typeof(USettingsKey))
            {
                return this == (USettingsKey)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty))
            {
                return this == (USettingsProperty)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty<>))
            {
                return obj.Equals(this);
            }
            else { return false; }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public sealed class USettingsProperty<T> : IEquatable<USettingsKey>,IEquatable<USettingsProperty>,IEquatable<USettingsProperty<T>> 
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

        public  bool Equals(USettingsKey other)
        {
            return this.linkuSettings == other.linkuSettings && name == other.Name;
        }
        public bool Equals(USettingsProperty other)
        {
            return this.linkuSettings == other.linkuSettings && name == other.Name;
        }
        public bool Equals(USettingsProperty<T> other)
        {
            return this.linkuSettings == other.linkuSettings && name == other.Name;
        }
        public static bool operator ==(USettingsProperty<T> v2, USettingsKey v1)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=(USettingsProperty<T> v2, USettingsKey v1)
        {
            return !(v2 == v1);
        }
        public static bool operator ==(USettingsProperty<T> v2, USettingsProperty v1)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=(USettingsProperty<T> v2, USettingsProperty v1)
        {
            return !(v2 == v1);
        }
        public static bool operator ==( USettingsKey v1, USettingsProperty<T> v2)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=( USettingsKey v1, USettingsProperty<T> v2)
        {
            return !(v2 == v1);
        }
        public static bool operator ==( USettingsProperty v1,USettingsProperty<T> v2)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=( USettingsProperty v1, USettingsProperty<T> v2)
        {
            return !(v2 == v1);
        }
        public static bool operator ==(USettingsProperty<T> v2, USettingsProperty<T> v1)
        {
            return v2.Equals(v1);
        }
        public static bool operator !=(USettingsProperty<T> v2, USettingsProperty<T> v1)
        {
            return !(v2 == v1);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(USettingsKey))
            {
                return this == (USettingsKey)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty))
            {
                return this == (USettingsProperty)obj;
            }
            else if (obj.GetType() == typeof(USettingsProperty<T>))
            {
                return this == (USettingsProperty<T>)obj;
            }
            else { return false; }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
    public delegate void USettingsChangedEventHander(USettingsKey key, USettingsChangedEventargs e);
}
