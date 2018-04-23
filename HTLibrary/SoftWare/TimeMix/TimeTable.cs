using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace User.SoftWare.TimeMix
{
    public class TimeTable : IList<TimeTableLayer>, IXmlFormat
    {
        List<TimeTableLayer> layers = new List<TimeTableLayer>();

        public TimeTable(params TimeTableLayer[] layers)
        {
            this.layers = layers.ToList();
        }

        public TimeTableLayer this[int index] { get => ((IList<TimeTableLayer>)layers)[index]; set => ((IList<TimeTableLayer>)layers)[index] = value; }
        public int Count => ((IList<TimeTableLayer>)layers).Count;
        public bool IsReadOnly => ((IList<TimeTableLayer>)layers).IsReadOnly;
        public void Add(TimeTableLayer item)
        {
            ((IList<TimeTableLayer>)layers).Add(item);
        }
        public void Clear()
        {
            ((IList<TimeTableLayer>)layers).Clear();
        }
        public bool Contains(TimeTableLayer item)
        {
            return ((IList<TimeTableLayer>)layers).Contains(item);
        }
        public void CopyTo(TimeTableLayer[] array, int arrayIndex)
        {
            ((IList<TimeTableLayer>)layers).CopyTo(array, arrayIndex);
        }
        public IEnumerator<TimeTableLayer> GetEnumerator()
        {
            return ((IList<TimeTableLayer>)layers).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<TimeTableLayer>)layers).GetEnumerator();
        }
        public int IndexOf(TimeTableLayer item)
        {
            return ((IList<TimeTableLayer>)layers).IndexOf(item);
        }
        public void Insert(int index, TimeTableLayer item)
        {
            ((IList<TimeTableLayer>)layers).Insert(index, item);
        }
        public bool Remove(TimeTableLayer item)
        {
            return ((IList<TimeTableLayer>)layers).Remove(item);
        }
        public void RemoveAt(int index)
        {
            ((IList<TimeTableLayer>)layers).RemoveAt(index);
        }
        public TimeTableLayer Select(DayOfWeek dayOfWeek)
        {
            foreach (var item in layers)
            {
                if (item.Range.IsDefined(dayOfWeek))
                {
                    return item;
                }
            }
            return null;
        }

        XElement IXmlFormat.Format()
        {
            return new XElement("TimeTable",
                (from item in layers select ((IXmlFormat)item).Format()).ToArray());
        }
        public void Save(string fileName)
        {
            ((IXmlFormat)this).Format().Save(fileName);
        }

        public static TimeTable Format(XElement element)
        {
            return new TimeTable()
            {
                layers =
                (from item in element.Elements() where item.Name == "TimeTableLayer" select (TimeTableLayer.Format(item))).ToList()
            };
        }
        public static TimeTable Load(string fileName)
        {
            XDocument xDocument = XDocument.Load(fileName);
            return Format(xDocument.Root);
        }
        /// <summary>
        /// 生成可分析的<see cref="TimeTable"/>
        /// </summary>
        /// <returns></returns>
        public TimeTable ToAnalyse()
        {
            TimeTableLayer[] layers = new TimeTableLayer[7];
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new TimeTableLayer(new DateOfWeekRange(i));
            }
            foreach (var item in this.layers)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (item.Range.IsDefined((DayOfWeek)i))
                    {
                        layers[i].Replace(item);
                    }
                }
            }
            return new TimeTable(layers);
        }
    }
    public class TimeTableLayer : ICollection<TimeSection>, IXmlFormat
    {
        public TimeTableLayer(DateOfWeekRange range)
        {
            Range = range;
        }
        public TimeTableLayer(DateOfWeekRange range, params TimeSection[] sections) : this(range)
        {
            Sections = new SortedSet<TimeSection>(sections);
        }
        public DateOfWeekRange Range { get; set; }
        SortedSet<TimeSection> Sections { get; set; } = new SortedSet<TimeSection>();
        public int Count => ((ICollection<TimeSection>)Sections).Count;
        public bool IsReadOnly => ((ICollection<TimeSection>)Sections).IsReadOnly;

        XElement IXmlFormat.Format()
        {
            return new XElement("TimeTableLayer", new XAttribute("range", Range),
                (from item in Sections select ((IXmlFormat)item).Format()).ToArray());
        }
        public static TimeTableLayer Format(XElement element)
        {
            return new TimeTableLayer(DateOfWeekRange.Parse(element.Attribute("range").Value),
                (from item in element.Elements() where element.Name == "TimeSection" select TimeSection.Format(item)).ToArray());
        }

        internal void Replace(TimeTableLayer other)
        {
            Sections.RemoveWhere((arg) => { return arg.Time >= other.First().Time && arg.Time <= other.Last().Time; });
            foreach (var item in other)
            {
                Sections.Add(item);
            }
        }
        public void Add(TimeSection item)
        {
            ((ICollection<TimeSection>)Sections).Add(item);
        }
        public void Clear()
        {
            ((ICollection<TimeSection>)Sections).Clear();
        }
        public bool Contains(TimeSection item)
        {
            return ((ICollection<TimeSection>)Sections).Contains(item);
        }
        public void CopyTo(TimeSection[] array, int arrayIndex)
        {
            ((ICollection<TimeSection>)Sections).CopyTo(array, arrayIndex);
        }
        public bool Remove(TimeSection item)
        {
            return ((ICollection<TimeSection>)Sections).Remove(item);
        }

        public IEnumerator<TimeSection> GetEnumerator()
        {
            return ((ICollection<TimeSection>)Sections).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<TimeSection>)Sections).GetEnumerator();
        }
    }
    public class TimeSection : IComparable<TimeSection>, IXmlFormat
    {
        public TimeSection(string name, DateTime time, int? type = null, string tag = "")
        {
            if (name == null || name == "")
            {
                throw new ArgumentException("name必须为有效值.");
            }
            Name = name;
            this.time = new DateTime(1, 1, 1, time.Hour, time.Minute, 0);
            Type = type;
            Tag = tag;
        }
        public TimeSection(string name, string time, int? type = null, string tag = "") : this(name, DateTime.Parse(time), type, tag)
        {
        }
        DateTime time;

        public string Name { get; set; }
        public DateTime Time
        {
            get => time;
            set
            {
                time = new DateTime(1, 1, 1, value.Hour, value.Minute, 0);
            }
        }
        public int? Type { get; set; }
        public string Tag { get; set; } = "";


        public override bool Equals(object obj)
        {
            if (obj is TimeSection v)
            {
                return Time == v.Time;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Time.GetHashCode();
        }
        XElement IXmlFormat.Format()
        {
            XElement e = new XElement("Timesection", new XAttribute("name", Name), new XAttribute("time", Time.ToShortTimeString()));
            if (Type != null)
            {
                e.SetAttributeValue("type", Type);
            }
            if (Tag != "")
            {
                e.SetAttributeValue("tag", Tag);
            }
            return e;
        }
        public static TimeSection Format(XElement element)
        {
            TimeSection t = new TimeSection(element.Attribute("name").Value, DateTime.Parse(element.Attribute("time").Value));
            if (element.Attribute("type") != null)
            {
                t.Type = int.Parse(element.Attribute("type").Value);
            }
            if (element.Attribute("tag") != null)
            {
                t.Tag = element.Attribute("tag").Value;
            }
            return t;
        }

        int IComparable<TimeSection>.CompareTo(TimeSection other)
        {
            return Time.CompareTo(other.Time);
        }
    }
    public struct DateOfWeekRange
    {
        int[] support;

        public DateOfWeekRange(params int[] support)
        {
            List<int> result = new List<int>();
            foreach (var item in support)
            {
                if (item >= 0 && item <= 6)
                {
                    result.Add(item);
                }
                else
                {
                    throw new ArgumentException("数字必须在0~6之间.");
                }
            }
            this.support = result.ToArray();

        }

        public bool IsEmpty => support == null || !support.Any();

        public static DateOfWeekRange Parse(string arg)
        {
            var s = (from item in arg.Split(',') orderby item select int.Parse(item)).Distinct(); ;
            return new DateOfWeekRange(s.ToArray());
        }
        public bool IsDefined(DayOfWeek dayOfWeek)
        {
            if (IsEmpty)
            {
                return false;
            }
            else
            {
                return support.Contains((int)dayOfWeek);
            }
        }
        public void SetValue(DayOfWeek dayOfWeek, bool isdefined)
        {
            if (IsEmpty)
            {
                if (isdefined)
                {
                    support = new int[] { (int)dayOfWeek };
                }
            }
            else
            {
                List<int> v = support.ToList();
                if (isdefined && !support.Contains((int)dayOfWeek))
                {
                    v.Add((int)dayOfWeek);
                }
                else if (!isdefined && support.Contains((int)dayOfWeek))
                {
                    v.Remove((int)dayOfWeek);
                }
                support = v.ToArray();
            }
        }
        public override string ToString()
        {
            if (IsEmpty)
            {
                return null;
            }
            else
            {
                return string.Join(",", support);
            }
        }
    }

    public interface IXmlFormat
    {
        XElement Format();
    }
}
