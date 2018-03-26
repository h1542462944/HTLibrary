using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.SoftWare.Service
{
    public struct AutoCheck:IUSettingsConvertArray,IComparable<AutoCheck>
    {
        public AutoCheck(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public int Num { get; set; }
        public bool IsPositiveInifinity { get; set; }

        public int CompareTo(AutoCheck other)
        {
            return Name.CompareTo(other.Name);
        }
        public override string ToString()
        {
            if (IsPositiveInifinity)
            {
                return Name + "∞";
            }
            else
            {
                return Name + Num;
            }
        }
        public IUSettingsConvertArray USettingsConvertArray(object[] contents)
        {
            return new AutoCheck((string)contents[0]) { Num = (int)contents[1], IsPositiveInifinity = (bool)contents[2] };
        }
        public object[] USettingsConvertArray()
        {
            return new object[] { Name, Num, IsPositiveInifinity };         
        }
    }
    public class AutoCheckCollection : ICollection<AutoCheck>,IUSettingsConvertArray
    {
        List<AutoCheck> list = new List<AutoCheck>();
        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(AutoCheck item)
        {
            list.Add(item);
            list.Sort();
        }
        public void Clear()
        {
            list.Clear();
        }
        public bool Contains(AutoCheck item)
        {
            return list.Contains(item);
        }
        public void CopyTo(AutoCheck[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
        public IEnumerator<AutoCheck> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        public bool Remove(AutoCheck item)
        {
            return list.Remove(item);
        }
        public IUSettingsConvertArray USettingsConvertArray(object[] contents)
        {
            return new AutoCheckCollection { list = new List<AutoCheck>((AutoCheck[])contents[0])};
        }
        public object[] USettingsConvertArray()
        {
            return new object[] { list.ToArray() };
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
    public class AutoCheckText : AutoText
    {
        public AutoCheckCollection AutoCheckCollection { get; set; }
        public override string Key => "autocheck";
        protected override string GetText()
        {
            if (AutoCheckCollection == null)
            {
                return null;
            }
            else
            {
                string start = "老师到达次数(名字+次数)\n";
                string lr = string.Join(";", AutoCheckCollection.ToArray());
                return start + lr;
            }
        }
    }

}
