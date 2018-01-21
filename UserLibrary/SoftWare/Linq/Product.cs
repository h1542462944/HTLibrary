using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.SoftWare.Linq
{
    public class Product
    {
        IEnumerable<ProductInfo> products;

        public Product()
        {
        }
        public Product(params ProductInfo[] products)
        {
            this.products = products;
        }
        public IEnumerable<ProductInfo> Products { get => products; set => products = value; }
    }
    public class ProductInfo
    {
        string name;
        string version;
        string title;
        DateTime lastUpdateTime;
        IEnumerable<UpdateInfo> updates;
        IEnumerable<AssetsInfo> assets;

        public ProductInfo(string name, string version, string title, DateTime lastUpdateTime)
        {
            this.name = name;
            this.version = version;
            this.title = title;
            this.lastUpdateTime = lastUpdateTime;
        }
        public ProductInfo(string name, string version, string title, DateTime lastUpdateTime,params object[] content):this(name,version,title,lastUpdateTime)
        {
            List<UpdateInfo> updates = new List<UpdateInfo>();
            List<AssetsInfo> assets = new List<AssetsInfo>();
            foreach (var item in content)
            {
                if (item is UpdateInfo)
                    updates.Add((UpdateInfo)item);
                else if (item is AssetsInfo)
                    assets.Add((AssetsInfo)item);
                else throw new FormatException();
            }
            this.updates = updates;
            this.assets = assets;
        }

        public string Name { get => name; set => name = value; }
        public string Version { get => version; set => version = value; }
        public string Title { get => title; set => title = value; }
        public DateTime LastUpdateTime { get => lastUpdateTime; set => lastUpdateTime = value; }
        public IEnumerable<UpdateInfo> Updates { get => updates; set => updates = value; }
        public IEnumerable<AssetsInfo> Assets { get => assets; set => assets = value; }

    }
    public class UpdateInfo
    {
        string version;
        string title;
        DateTime lastUpdateTime;
        IEnumerable<OperateInfo> operates;

        public UpdateInfo(string version, string title, DateTime lastUpdateTime)
        {
            this.version = version;
            this.title = title;
            this.lastUpdateTime = lastUpdateTime;
        }
        public UpdateInfo(string version, string title, DateTime lastUpdateTime, params OperateInfo[] operates) : this(version, title, lastUpdateTime)
        {
            this.operates = operates;
        }

        public string Version { get => version; set => version = value; }
        public string Title { get => title; set => title = value; }
        DateTime  LastUpdateTime { get => lastUpdateTime; set => lastUpdateTime = value; }
        public IEnumerable<OperateInfo> Operates { get => operates; set => operates = value; }
    }
    public class AssetsInfo
    {
        string key;
        string title;
        DateTime lastUpdateTime;
        IEnumerable<OperateInfo> operates;

        public AssetsInfo(string key, string title, DateTime lastUpdateTime)
        {
            this.key = key;
            this.title = title;
            this.lastUpdateTime = lastUpdateTime;
        }
        public AssetsInfo(string key, string title, DateTime lastUpdateTime,params OperateInfo[] operates):this(key,title,lastUpdateTime)
        {
            this.operates = operates;
        }

        public string Key { get => key; set => key = value; }
        public string Title { get => title; set => title = value; }
        public DateTime LastUpdateTime { get => lastUpdateTime; set => lastUpdateTime = value; }
        public IEnumerable<OperateInfo> Operates { get => operates; set => operates = value; }
    }
    public class OperateInfo
    {
        string method;
        string path;
        int size;

        public OperateInfo(string method, string path)
        {
            this.method = method;
            this.path = path;
        }
        public OperateInfo(string method, string path, int size) : this(method, path)
        {
            this.size = size;
        }

        public string Method { get => method; set => method = value; }
        public string Path { get => path; set => path = value; }
        public int Size { get => size; set => size = value; }
    }

    public class CTimeMix
    { }
    public class CTimeTable : IDictionary<string, CTimeTableLayer>
    {
        public CTimeTableLayer this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<CTimeTableLayer> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(string key, CTimeTableLayer value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, CTimeTableLayer> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, CTimeTableLayer> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, CTimeTableLayer>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, CTimeTableLayer>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, CTimeTableLayer> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out CTimeTableLayer value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class CTimeTableLayer :ICollection  <CTimePointCollection>
    {
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(CTimePointCollection item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(CTimePointCollection item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(CTimePointCollection[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<CTimePointCollection> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(CTimePointCollection item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class CTimePointCollection : ICollection<CTimePoint>
    {
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(CTimePoint item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(CTimePoint item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(CTimePoint[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<CTimePoint> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(CTimePoint item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class CTimePoint
    { }
}
