using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using User;
using User.SoftWare.HTStudio.Universial;

namespace User.SoftWare.HTStudio
{
    namespace Host
    {

    }
    namespace Universial
    {
        #region Base
        public interface IToStringArray
        {
            string[] ToStringArray();
        }
        public interface ICanSave
        {
            void Save();
        }

        public sealed class Logger
        {
            private static string dataDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Data\";
            private static string productsDirectory = DataDirectory + @"Products\";
            private static string loggerDirectory = DataDirectory + @"Logger\";
            private static string usersDirectory = DataDirectory + @"Users\";
            private static string productsListPath = ProductsDirectory + "ProductsList.xml";
            private static string usersDataPath = UsersDirectory + "basedata.xml";
            private static string downloadPath = DataDirectory + "download.xml";

            public static string DataDirectory { get => dataDirectory; }
            public static string ProductsDirectory { get => productsDirectory; }
            public static string LoggerDirectory { get => loggerDirectory; }
            public static string UsersDirectory { get => usersDirectory; }
            public static string ProductsListPath { get => productsListPath; }
            public static string UsersDataPath { get => usersDataPath; }
            public static string DownloadPath { get => downloadPath; set => downloadPath = value; }
        }
        public abstract class XmlBasedInfo
        {
            private DateTime lastaccessTime;
            protected const string lastaccesstimestring = "lastaccesstime";

            protected virtual string FilePath { get; }
            protected virtual string RootName { get; }
            protected DateTime LastaccessTime { get => lastaccessTime; }

            protected void ReadAccessTime()
            {
                try
                {
                    XDocument xDocument = XDocument.Load(FilePath);
                    lastaccessTime = DateTime.Parse(xDocument.Root.Attribute(lastaccesstimestring).Value);
                }
                catch (Exception)
                {
                    lastaccessTime = DateTime.MinValue;
                }
            }
            protected void Flush()
            {
                lastaccessTime = DateTime.Now;
                XDocument xDocument = XDocument.Load(FilePath);
                xDocument.Root.ReplaceAttributes(lastaccesstimestring, lastaccessTime.ToString());
                xDocument.Save(FilePath);
            }
            protected void Create()
            {
                lastaccessTime = DateTime.Now;
                XDocument xDocument = new XDocument(
                    new XElement(RootName, new XAttribute(lastaccesstimestring, lastaccessTime.ToString()))
                    );
                xDocument.Save(FilePath);
            }
            public static bool IsNewest(string path, DateTime comparetime)
            {
                DateTime lastaccessTime = GetAccessTime(path);
                return DateTime.Compare(comparetime, lastaccessTime) > 0;
            }
            public static DateTime GetAccessTime(string path)
            {
                try
                {
                    XDocument xDocument = XDocument.Load(path);
                    return DateTime.Parse(xDocument.Root.Attribute(lastaccesstimestring).Value);
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }
        #endregion
        #region Product

        public class ProductBase : XmlBasedInfo, IToStringArray
        {
            public readonly static ProductBase Empty = new ProductBase();

            string name;
            string version;
            string detail;
            DateTime lastupdatetime;
            string startup;
            bool icon = false;
            string environment = "";

            public ProductBase()
            {
            }

            public ProductBase(string name, string version, string detail, DateTime lastupdatetime, string startup)
            {
                Name = name;
                Version = version;
                Detail = detail;
                Lastupdatetime = lastupdatetime;
                Startup = startup;
            }

            public string Name { get => name; set => name = value; }
            public string Version { get => version; set => version = value; }
            public string Detail { get => detail; set => detail = value; }
            public DateTime Lastupdatetime { get => lastupdatetime; set => lastupdatetime = value; }
            public string Startup { get => startup; set => startup = value; }
            public bool Icon { get => icon; set => icon = value; }
            public string Environment { get => environment; set => environment = value; }

            public string[] ToStringArray()
            {
                List<string> infolist = new List<string>
            {
                Name,
                Version,
                Detail,
                Lastupdatetime.ToString(),
                Startup,
                Icon.ToString(),
                Environment
            };
                return infolist.ToArray();
            }
            public static ProductBase Parse(string[] info)
            {
                ProductBase productBase = new ProductBase
                {
                    Name = info[0],
                    Version = info[1],
                    Detail = info[2],
                    Lastupdatetime = DateTime.Parse(info[3]),
                    Startup = info[4],
                    Icon = bool.Parse(info[5]),
                    Environment = info[6]
                };
                return productBase;
            }

            public int CompareVersion(string otherversion)
            {
                string[] t1 = Version.Split('.');
                int i1 = 1000 * int.Parse(t1[0]) + 100 * int.Parse(t1[1]) + 10 * int.Parse(t1[2]) + int.Parse(t1[3]);
                string[] t2 = otherversion.Split('.');
                int i2 = 1000 * int.Parse(t2[0]) + 100 * int.Parse(t2[1]) + 10 * int.Parse(t2[2]) + int.Parse(t2[3]);
                return i1 - i2;
            }
        }
        public sealed class Product : ProductBase
        {
            protected override string FilePath => Logger.ProductsListPath;
            protected override string RootName => "Product";

            public new static readonly Product Empty = new Product();

            List<UpdateInfo> update = new List<UpdateInfo>();
            List<AssetsInfo> assets = new List<AssetsInfo>();

            public Product(string name, string version, string detail, DateTime lastupdatetime, string startup) : base(name, version, detail, lastupdatetime, startup)
            {
            }
            private Product() { }

            public List<UpdateInfo> Update { get => update; set => update = value; }
            public List<AssetsInfo> Assets { get => assets; set => assets = value; }

            public static Product Read(string name)
            {
                Product product = new Product();
                //try
                //{
                XDocument xDocument = XDocument.Load(Logger.ProductsListPath);
                var productelements = from item in xDocument.Root.Elements()
                                      where item.Attribute("name").Value == name
                                      select item;
                var productelement = productelements.First();
                {
                    Console.WriteLine("inreadproducts");
                    List<UpdateInfo> update = new List<UpdateInfo>();
                    List<AssetsInfo> assets = new List<AssetsInfo>();
                    product.Name = name;
                    product.Version = productelement.Attribute("version").Value;
                    product.Detail = productelement.Attribute("detail").Value;
                    product.Lastupdatetime = DateTime.Parse(productelement.Attribute("lastupdatetime").Value);
                    product.Startup = productelement.Attribute("startup").Value;
                    foreach (var operateelements in productelement.Elements())
                    {
                        Console.WriteLine("inReadUpdate/Assets");
                        List<OperateInfo> operates = new List<OperateInfo>();
                        List<string> userlimits = new List<string>();
                        string tag = "";
                        if (operateelements.Name == "update")
                        {
                            tag = @"Products\" + product.Name + @"\version" + operateelements.Attribute("version").Value + @"\";
                        }
                        else if (operateelements.Name == "assets")
                        {
                            tag = @"Products\" + product.Name + @"\assets" + operateelements.Attribute("title").Value + @"\";
                        }
                        foreach (var item in operateelements.Elements())
                        {
                            if (item.Name == "download" || item.Name == "createdirectory")
                            {
                                int size;
                                try
                                {
                                    size = int.Parse(item.Attribute("size").Value);
                                }
                                catch (Exception)
                                {
                                    size = 0;
                                }
                                Console.WriteLine("inreadoperate!", item.Name);
                                operates.Add(new OperateInfo(item.Name.LocalName, item.Attribute("path").Value, tag, size));
                            }
                            if (item.Name == "userlimits")
                            {
                                Console.WriteLine("inReaduserslimit");
                                foreach (var item2 in item.Elements())
                                {
                                    if (item2.Name == "add")
                                    {
                                        userlimits.Add(item2.Value);
                                    }
                                }
                            }
                        }
                        if (operateelements.Name == "update")
                        {
                            update.Add(
                                new UpdateInfo(operateelements.Attribute("version").Value, operateelements.Attribute("title").Value,
                                operateelements.Attribute("detail").Value, DateTime.Parse(operateelements.Attribute("time").Value))
                                {
                                    Operates = operates
                                }
                                );
                        }
                        if (operateelements.Name == "assets")
                        {
                            assets.Add(
                                new AssetsInfo(operateelements.Attribute("title").Value,
                                operateelements.Attribute("detail").Value, DateTime.Parse(operateelements.Attribute("time").Value))
                                {
                                    Operates = operates,
                                    Userlimitslist = userlimits
                                }
                                );
                        }
                    }
                    product.Icon = bool.Parse(productelement.Attribute("icon").Value);
                    product.Environment = productelement.Attribute("environment").Value;

                    product.Update = update;
                    product.Assets = assets;
                    Console.WriteLine("+" + product.Update.Count);
                    Console.WriteLine("+" + product.Assets.Count);
                }
                //}
                //catch (Exception)
                //{

                //}
                product.ReadAccessTime();
                return product;
            }
            /// <summary>
            ///只保存基础信息,若已存在,则更新信息.
            /// </summary>
            public void SaveBaseInfo()
            {
                XDocument xDocument = XDocument.Load(Logger.ProductsListPath);
                try
                {
                    var productelement = SelectProductXElement(xDocument);
                    Console.WriteLine("inChangeProductBaseInfoAndSave");
                    productelement.ReplaceAttributes(new XAttribute("name", this.Name),
                        new XAttribute("version", this.Version),
                        new XAttribute("detail", this.Detail),
                        new XAttribute("lastupdatetime", this.Lastupdatetime.ToString()),
                        new XAttribute("startup", this.Startup),
                        new XAttribute("icon", this.Icon.ToString()),
                        new XAttribute("environment", this.Environment)
                        );
                }
                catch (Exception)
                {
                    Console.WriteLine("inCreateProductBaseInfoAndSave");
                    xDocument.Root.Add
                        (
                        new XElement(
                            "product", new XAttribute("name", this.Name),
                        new XAttribute("version", this.Version),
                        new XAttribute("detail", this.Detail),
                        new XAttribute("lastupdatetime", this.Lastupdatetime.ToString()),
                        new XAttribute("startup", this.Startup),
                        new XAttribute("icon", this.Icon.ToString()),
                        new XAttribute("environment", this.Environment)
                            ));
                }
                xDocument.Save(Logger.ProductsListPath);
                Flush();
            }
            public void SaveUpdateInfo(UpdateInfo info)
            {
                Console.WriteLine("inSaveUpdateInfo");
                XDocument xDocument = XDocument.Load(Logger.ProductsListPath);
                XElement productelement = SelectProductXElement(xDocument);
                try
                {
                    var updateelements = from item in productelement.Elements()
                                         where item.Name.LocalName == "update" && item.Attribute("version").Value == info.Version
                                         select item;
                    updateelements.Remove();
                }
                catch (Exception)
                {
                }
                {
                    List<XElement> operates = GetOperatesXElement(info);
                    productelement.Add(
            new XElement("update",
            new XAttribute("version", info.Version),
            new XAttribute("title", info.Title),
            new XAttribute("detail", info.Detail),
            new XAttribute("time", info.Time.ToString()),
            operates.ToArray()
            ));
                }
                xDocument.Save(Logger.ProductsListPath);
                Flush();
            }
            public void SaveAssetsInfo(AssetsInfo info)
            {
                Console.WriteLine("inSaveAssetsInfo");
                XDocument xDocument = XDocument.Load(Logger.ProductsListPath);
                XElement productelement = SelectProductXElement(xDocument);
                try
                {
                    var assetselements = from item in productelement.Elements()
                                         where item.Name.LocalName == "assets" && item.Attribute("title").Value == info.Title && item.Attribute("detail").Value == info.Detail
                                         select item;
                    assetselements.Remove();
                }
                catch (Exception)
                {
                }
                {
                    List<XElement> operates = GetOperatesXElement(info);
                    XElement userlimits = GetuserlimintXElement(info);
                    productelement.Add(
                        new XElement("assets",
                        new XAttribute("title", info.Title),
                        new XAttribute("detail", info.Detail),
                        new XAttribute("time", info.Time.ToString()),
                        userlimits,
                        operates.ToArray()
                        ));
                    xDocument.Save(Logger.ProductsListPath);
                    Flush();
                }
            }

            public OperateList GetUpdateOperateList(string oldversion)
            {
                Dictionary<string, OperateInfo> dictionary = new Dictionary<string, OperateInfo>();
                foreach (var updatelist in Update)
                {

                    Console.WriteLine(Update.Count);
                    foreach (var item in updatelist.Operates)
                    {
                        //try
                        //{
                        dictionary.Add(item.Path, item);
                        Console.WriteLine(item.Path);
                        //}
                        //catch (Exception)
                        //{
                        //}
                    }
                }

                OperateList operateList = new OperateList
                {
                    Operates = dictionary.Values.ToList()
                };
                Console.WriteLine("T" + operateList.Operates.Count);
                operateList.Analyse();
                return operateList;
            }
            public static string[] GetProductsList()
            {
                List<string> products = new List<string>();
                XDocument xDocument = XDocument.Load(Logger.ProductsListPath);
                foreach (var item in xDocument.Root.Elements())
                {
                    products.Add(item.Attribute("name").Value);
                }
                return products.ToArray();
            }

            private static XElement GetuserlimintXElement(AssetsInfo info)
            {
                XElement userlimits;
                List<XElement> userlimitslist = new List<XElement>();
                foreach (var item in info.Userlimitslist)
                {
                    userlimitslist.Add(
                        new XElement("add", item)
                        );
                }
                userlimits = new XElement(
                    "userlimits",
                    userlimitslist.ToArray()
                    );
                return userlimits;
            }
            private static List<XElement> GetOperatesXElement(UpdateInfo info)
            {
                List<XElement> operates = new List<XElement>();
                foreach (var item in info.Operates)
                {
                    operates.Add
                        (
                        new XElement(item.Method, new XAttribute("path", item.Path), new XAttribute("size", item.Size))
                        );
                }
                return operates;
            }
            private static List<XElement> GetOperatesXElement(AssetsInfo info)
            {
                List<XElement> operates = new List<XElement>();
                foreach (var item in info.Operates)
                {
                    operates.Add
                        (
                        new XElement(item.Method, new XAttribute("path", item.Path), new XAttribute("size", item.Size))
                        );
                }
                return operates;
            }
            private XElement SelectProductXElement(XDocument xDocument)
            {
                var productelements = from item in xDocument.Root.Elements()
                                      where item.Attribute("name").Value == Name
                                      select item;
                var productelement = productelements.First();
                return productelement;
            }

        }

        public sealed class UpdateInfo : IToStringArray
        {
            string version;
            string title;
            string detail;
            DateTime time;
            List<OperateInfo> operates;

            public UpdateInfo(string version, string title, string detail, DateTime time)
            {
                Version = version;
                Title = title;
                Detail = detail;
                Time = time;
            }

            public string Version { get => version; set => version = value; }
            public string Title { get => title; set => title = value; }
            public string Detail { get => detail; set => detail = value; }
            public DateTime Time { get => time; set => time = value; }
            public List<OperateInfo> Operates { get => operates; set => operates = value; }

            public string[] ToStringArray()
            {
                List<string> infolist = new List<string>
                {
                    Version,
                    Title,
                    Detail,
                    Time.ToString()
                };
                foreach (var item in Operates)
                {
                    infolist.Add(item.Method);
                    infolist.Add(item.Path);
                    infolist.Add(item.Tag);
                    infolist.Add(item.Size.ToString());
                }
                return infolist.ToArray();
            }
            public static UpdateInfo Parse(string[] info)
            {
                UpdateInfo updateInfo = new UpdateInfo(info[0], info[1], info[2], DateTime.Parse(info[3]));
                List<OperateInfo> operate = new List<OperateInfo>();
                for (int i = 4; i < info.Length; i += 4)
                {
                    operate.Add(new OperateInfo(info[i], info[i + 1], info[i + 2], int.Parse(info[i + 3])));
                }
                updateInfo.Operates = operate;
                return updateInfo;
            }
        }
        public sealed class AssetsInfo : IToStringArray
        {
            string title;
            string detail;
            DateTime time;
            List<OperateInfo> operates;
            List<string> userlimitslist = new List<string>();

            public AssetsInfo(string title, string detail, DateTime time)
            {
                Title = title;
                Detail = detail;
                Time = time;
            }

            public AssetsInfo()
            {
            }

            public string Title { get => title; set => title = value; }
            public string Detail { get => detail; set => detail = value; }
            public DateTime Time { get => time; set => time = value; }
            public List<OperateInfo> Operates { get => operates; set => operates = value; }
            public List<string> Userlimitslist { get => userlimitslist; set => userlimitslist = value; }

            public string[] ToStringArray()
            {
                List<string> infolist = new List<string>()
                {
                    Title,
                    Detail,
                    Time.ToString(),
                    Userlimitslist.Count.ToString()
                };
                foreach (var item in Userlimitslist)
                {
                    infolist.Add(item);
                }
                foreach (var item in Operates)
                {
                    infolist.Add(item.Method);
                    infolist.Add(item.Path);
                    infolist.Add(item.Tag);
                    infolist.Add(item.Size.ToString());
                }
                return infolist.ToArray();
            }
            public AssetsInfo Parse(string[] info)
            {
                AssetsInfo assetsInfo = new AssetsInfo(info[0], info[1], DateTime.Parse(info[2]));
                string[] userslimit = new string[int.Parse(info[3])];
                List<OperateInfo> operates = new List<OperateInfo>();
                for (int i = 0; i < userslimit.Length; i++)
                {
                    userslimit[i] = info[i + 4];
                }
                for (int i = 4 + userslimit.Length; i < info.Length; i += 4)
                {
                    operates.Add(new OperateInfo(info[i], info[i + 1], info[i + 2], int.Parse(info[i + 3])));
                }
                assetsInfo.Userlimitslist = userlimitslist.ToList();
                assetsInfo.Operates = operates;
                return assetsInfo;
            }
        }
        public sealed class OperateInfo
        {
            string method;
            string path;
            string tag;
            int size;
            int location;

            public OperateInfo(string method, string path, string tag, int size)
            {
                Method = method;
                Path = path;
                Tag = tag;
                Size = size;
            }

            public OperateInfo()
            {
            }

            public string Method { get => method; set => method = value; }
            public string Path { get => path; set => path = value; }
            public string Tag { get => tag; set => tag = value; }
            public int Size { get => size; set => size = value; }
            public int Location { get => location; set => location = value; }

            public void ReStart()
            {
                Location = 0;
            }
            public void AppendPath(string arg)
            {
                Path = arg + Path;
            }
        }

        public sealed class OperateList : IToStringArray
        {
            int operationCount;
            int size;
            List<OperateInfo> operates = new List<OperateInfo>();

            public OperateList()
            {
            }
            public OperateList(int operationCount, int size)
            {
                OperationCount = operationCount;
                Size = size;
            }

            public int OperationCount { get => operationCount; set => operationCount = value; }
            public int Size { get => size; set => size = value; }
            public List<OperateInfo> Operates { get => operates; set => operates = value; }

            public void Analyse()
            {
                foreach (var item in Operates)
                {
                    OperationCount++;
                    Size += item.Size;
                }
            }
            public string[] ToStringArray()
            {
                List<string> infolist = new List<string>()
                {
                    OperationCount.ToString(),
                    Size.ToString()
                };
                foreach (var item in Operates)
                {
                    infolist.Add(item.Method);
                    infolist.Add(item.Path);
                    infolist.Add(item.Tag);
                    infolist.Add(item.Size.ToString());
                }
                return infolist.ToArray();
            }
            public static OperateList Parse(string[] info)
            {
                OperateList operateList = new OperateList(int.Parse(info[0]), int.Parse(info[1]));
                for (int i = 2; i < info.Length; i += 4)
                {
                    operateList.Operates.Add(new OperateInfo(info[i], info[i + 1], info[i + 2], int.Parse(info[i + 3])));
                }
                return operateList;
            }
            public void AppendPath(string arg)
            {
                foreach (var item in Operates)
                {
                    item.AppendPath(arg);
                }
            }

        }
        #endregion
        #region Users
        public sealed class UserInfo
        {
            public readonly static UserInfo Empty = new UserInfo();

            string name = "";
            string password;
            bool isAdmin = false;
            int exp = 0;
            bool isautologin = true;
            DateTime lastaccesstime = new DateTime();
            string lastremoteip = "";

            public UserInfo()
            {
            }
            public UserInfo(string name, string password)
            {
                Name = name;
                Password = password;
            }
            public UserInfo(string name, string password, bool isAdmin, int exp) : this(name, password)
            {
                IsAdmin = isAdmin;
                Exp = exp;
            }

            /// <summary>
            /// 用户的姓名.
            /// </summary>
            public string Name
            {
                get => name; set
                {
                    name = value;
                }
            }
            /// <summary>
            /// 用户的密码.
            /// </summary>
            public string Password { get => password; set => password = value; }
            /// <summary>
            /// 是否是管理员.
            /// </summary>
            public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
            /// <summary>
            /// 经验
            /// </summary>
            public int Exp { get => exp; set => exp = value; }
            public bool Isautologin { get => isautologin; set => isautologin = value; }
            public DateTime Lastaccesstime { get => lastaccesstime; set => lastaccesstime = value; }
            public string Lastremoteip { get => lastremoteip; set => lastremoteip = value; }

            public static UserInfo Read(string name)
            {
                UserInfo userInfo = new UserInfo();
                string userDirectory = Logger.UsersDirectory + name + @"\";
                if (File.Exists(Logger.UsersDataPath))
                {
                    XDocument xDocument = XDocument.Load(Logger.UsersDataPath);
                    var userelements = from item in xDocument.Root.Elements()
                                       where item.Name == "user" && item.Attribute("name").Value == name
                                       select item;
                    var userelement = userelements.First();
                    try
                    {
                        userInfo.Name = name;
                        userInfo.Password = userelement.Attribute("password").Value;

                            userInfo.Exp = int.Parse(userelement.Attribute("exp").Value);
                            userInfo.IsAdmin = bool.Parse(userelement.Attribute("isadmin").Value);
                        userInfo.Lastaccesstime = DateTime.Parse(userelement.Attribute("lastaccesstime").Value);
                        userInfo.Isautologin = bool.Parse(userelement.Attribute("isautologin").Value);
                        userInfo.Lastremoteip = userelement.Attribute("lastremoteip").Value;

                    }
                    catch (Exception)
                    {
                        return Empty;
                    }
                }
                return userInfo;
            }
            public void Save()
            {
                Console.WriteLine("inSave!");
                string userDirectory = Logger.UsersDirectory + this.Name + @"\";
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }
                XDocument xDocument;
                if (!File.Exists(Logger.UsersDataPath))
                {
                    xDocument = new XDocument(
                        new XElement("basedata"));
                    xDocument.Save(Logger.UsersDataPath);
                }
                xDocument = XDocument.Load(Logger.UsersDataPath);
                try
                {
                    var userelements = from item in xDocument.Root.Elements()
                                       where item.Name == "user" && item.Attribute("name").Value == this.Name
                                       select item;
                    userelements.Remove();
                }
                catch (Exception)
                {

                }
                xDocument.Root.Add(new XElement("user",
                        new XAttribute("name", this.Name),
                        new XAttribute("password", this.Password),
                        new XAttribute("isadmin", this.IsAdmin.ToString()),
                        new XAttribute("exp", this.Exp.ToString()),
                        new XAttribute("lastaccesstime", this.Lastaccesstime.ToString()),
                        new XAttribute("isautologin", this.Isautologin.ToString()),
                        new XAttribute ("lastremoteip",this.Lastremoteip.ToString ())
                
                        
                        ));
                xDocument.Save(Logger.UsersDataPath);
            }
            public static void Remove(string name)
            {
                try
                {
                    XDocument xDocument;
                    xDocument = XDocument.Load(Logger.UsersDataPath);
                    var userelements = from item in xDocument.Root.Elements()
                                       where item.Name == "user" && item.Attribute("name").Value == name
                                       select item;
                    userelements.Remove();
                }
                catch (Exception)
                {
                }
            }
            public static bool Exist(string name)
            {
                try
                {
                    XDocument xDocument = XDocument.Load(Logger.UsersDataPath);
                    var userelements = from item in xDocument.Root.Elements()
                                       where item.Name == "user" && item.Attribute("name").Value == name
                                       select item;
                    var userelement = userelements.First();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static string[] ListUser()
            {
                List<string> listuser = new List<string>();
                try
                {
                    XDocument xDocument = XDocument.Load(Logger.UsersDataPath);
                    var userelements = from item in xDocument.Root.Elements()
                                       where item.Name == "user"
                                       select item;
                    foreach (var item in userelements)
                    {
                        listuser.Add(item.Attribute("name").Value);
                    }
                }
                catch (Exception)
                {
                    return new string[0];
                }
                return listuser.ToArray();
            }
        }
        #endregion
    }
    namespace Service
    {
        #region interface
        [ServiceContract]
        public interface IProduct
        {
            [OperationContract]
            string[] Product_List();

            [OperationContract]
            string[] Product_GetInfo(string name);

            [OperationContract]
            string[] Product_GetOperation(string name, string oldversion);

            [OperationContract]
            DateTime Product_GetAccessTime();

            [OperationContract]
            bool Product_ListIsNewest(DateTime comparetime);

            [OperationContract]
            bool Product_NeedUpdate(string name, string oldversion);

        }
        [ServiceContract]
        public interface IChannel
        {
            [OperationContract]
            byte[] Channel_DownLoad(string path, int index);
            [OperationContract]
            bool Channel_UpLoad(string path, int index, byte[] data);
        }
        [ServiceContract]
        public interface IHTStudio : IProduct, IChannel
        {
        }
        #endregion
        public sealed class HTStudioService : IHTStudio
        {
            public byte[] Channel_DownLoad(string path, int index)
            {
                lock (this)
                {
                    string filepath = Logger.DataDirectory + path;
                    long startposition = Client.Channel.channelsize * index;
                    FileInfo fileInfo = new FileInfo(filepath);
                    if (index == 0 && Client.Channel.channelsize >= fileInfo.Length)
                    {
                        return File.ReadAllBytes(filepath);
                    }
                    else
                    {
                        long perchannelsize;
                        FileStream stream = fileInfo.OpenRead();
                        if (fileInfo.Length - startposition > Client.Channel.channelsize)
                        {
                            perchannelsize = Client.Channel.channelsize;
                        }
                        else
                        {
                            perchannelsize = fileInfo.Length - startposition;
                        }
                        stream.Position = startposition;
                        byte[] data = new byte[perchannelsize];
                        for (int i = 0; i < perchannelsize; i++)
                        {
                            data[i] = (byte)stream.ReadByte();
                        }
                        stream.Dispose();
                        stream = null;
                        return data;
                    }
                }
            }
            public bool Channel_UpLoad(string path, int index, byte[] data)
            {
                throw new NotImplementedException();
            }
            public DateTime Product_GetAccessTime()
            {
                return XmlBasedInfo.GetAccessTime(Logger.ProductsListPath);
            }
            public string[] Product_GetInfo(string name)
            {
                ProductBase product = (ProductBase)Product.Read(name);
                return product.ToStringArray();
            }
            public string[] Product_GetOperation(string name, string oldversion)
            {
                Product product = Product.Read(name);
                return product.GetUpdateOperateList(oldversion).ToStringArray();
            }
            public string[] Product_List()
            {
                return Product.GetProductsList();
            }
            public bool Product_ListIsNewest(DateTime comparetime)
            {
                return XmlBasedInfo.IsNewest(Logger.ProductsListPath, comparetime);
            }
            public bool Product_NeedUpdate(string name, string oldversion)
            {
                Product product = Product.Read(name);
                return product.CompareVersion(oldversion) > 0;
            }
        }
    }
    namespace Client
    {
        #region Channel
        public delegate void ChannelLocationChangedHander(object sender, ChannelLocationChangedEventargs e);
        public delegate void ChannelEventHander(object sender, ChannelEventargs e);

        public sealed class ChannelEventargs : EventArgs
        {
            string tag;
            string path;
            int index;
            string targetDirectory;

            public ChannelEventargs(string tag, string path, int index, string targetDirectory)
            {
                Tag = tag;
                Path = path;
                Index = index;
                TargetDirectory = targetDirectory;
            }

            public string Tag { get => tag; set => tag = value; }
            public string Path { get => path; set => path = value; }
            public int Index { get => index; set => index = value; }
            public string TargetDirectory { get => targetDirectory; set => targetDirectory = value; }
        }
        public sealed class ChannelLocationChangedEventargs : EventArgs
        {
            int location;
            int size;

            public ChannelLocationChangedEventargs(int location, int size)
            {
                Location = location;
                Size = size;
            }

            public int Location { get => location; set => location = value; }
            public int Size { get => size; set => size = value; }
        }

        public abstract class Channel : XmlBasedInfo
        {
            public const int channelsize = 1024 * 1024;
            //public const int maxthreadcount = 4;
            OperateList operates = new OperateList();
            bool isCompleted = false;
            bool isPaused = false;
            int location = 0;
            //int threadcount = 0;
            string targetDireactory;

            public OperateList OperatesList { get => operates; set => operates = value; }
            public bool IsCompleted { get => isCompleted; set => isCompleted = value; }
            public bool IsPaused { get => isPaused; set => isPaused = value; }
            public int Location
            {
                get => location; set
                {
                    location = value;
                    if (LocationChanged != null)
                    {
                        LocationChanged.Invoke(this, new ChannelLocationChangedEventargs(value, OperatesList.Size));
                    }
                }
            }
            public string TargetDireactory { get => targetDireactory; set => targetDireactory = value; }
            //public int Threadcount { get => threadcount; set => threadcount = value; }

            public void Start()
            {
                lock (this)
                {
                    if (!IsCompleted)
                    {
                        IsPaused = false;
                        while (!IsCompleted && !IsPaused)
                        {
                            Push();
                        }
                    }
                }
            }
            public void Pause()
            {
                IsPaused = true;
            }
            public void ReStart()
            {
                for (int i = 0; i < OperatesList.Operates.Count; i++)
                {
                    OperatesList.Operates[i].ReStart();
                }
                this.Location = 0;
                IsCompleted = false;
                Start();
            }
            protected abstract void Push();
            public event ChannelLocationChangedHander LocationChanged;
        }
        public sealed class DownLoadInfo : Channel
        {
            public static event ChannelEventHander DownLoad;
            protected override void Push()
            {
                try
                {
                    OperateInfo task = new OperateInfo();
                    for (int i = 0; i < OperatesList.Operates.Count; i++)
                    {
                        if (OperatesList.Operates[i].Method == "download" && OperatesList.Operates[i].Location < OperatesList.Operates[i].Size)
                        {
                            task = OperatesList.Operates[i];
                        }
                    }
                    if (task.Path == null || task.Path == "")
                    {
                        IsCompleted = true;
                        return;
                    }
                    int downloadindex = Location / channelsize;
                    if (DownLoad != null)
                    {
                        if (task.Size - task.Location > channelsize)
                        {
                            task.Location += channelsize;
                            Location += channelsize;
                        }
                        else
                        {
                            Location += task.Size - task.Location;
                            task.Location = task.Size;
                        }
                    }
                    else
                    {
                        IsCompleted = true;
                        return;
                    }
                    DownLoad.Invoke(this, new ChannelEventargs(task.Tag, task.Path, downloadindex, TargetDireactory));
                }
                catch (Exception)
                {
                    IsCompleted = true;
                }
            }
        }
        public sealed class UpLoadInfo:Channel
        {
            public static event ChannelEventHander UpLoad;
            protected override void Push()
            {
                try
                {
                    OperateInfo task = new OperateInfo();
                    for (int i = 0; i < OperatesList.Operates.Count; i++)
                    {
                        if (OperatesList.Operates[i].Method == "download" && OperatesList.Operates[i].Location < OperatesList.Operates[i].Size)
                        {
                            task = OperatesList.Operates[i];
                        }
                    }
                    if (task.Path == null || task.Path == "")
                    {
                        IsCompleted = true;
                        return;
                    }
                    int uploadindex = Location / channelsize;
                    if (UpLoad!= null)
                    {
                        if (task.Size - task.Location > channelsize)
                        {
                            task.Location += channelsize;
                            Location += channelsize;
                        }
                        else
                        {
                            Location += task.Size - task.Location;
                            task.Location = task.Size;
                        }
                    }
                    else
                    {
                        IsCompleted = true;
                        return;
                    }
                    UpLoad.Invoke(this, new ChannelEventargs(task.Tag, task.Path, uploadindex, TargetDireactory));
                }
                catch (Exception)
                {
                    IsCompleted = true;
                }
            }
        }
        public class ChannelListSystem
        {
            List<DownLoadInfo> downLoadList = new List<DownLoadInfo>();
            List<UpLoadInfo> upLoad = new List<UpLoadInfo>();
        }
        #endregion
    }
}
