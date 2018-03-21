using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace User.SoftWare.Service
{
    public abstract class AutoText
    {
        protected AutoText()
        {
        }

        public abstract string Key { get; }
        string StartText => "#" + Key + "{";
        string EndText => "}";
        public RichTextBox Target { get; set; }
        public string NewestText { get; private set; }

        protected abstract string GetText();
        public void Next()
        {
            string text = GetText();
            if (text!=null)
            {
                NewestText = StartText + GetText() + EndText;
                ReplaceText();
            }
        }
        public string FindText()
        {
            if (Target != null)
            {
                FindPointer(out TextPointer startp, out TextPointer endp);
                if (startp != null && endp != null)
                {
                    Target.Selection.Select(startp, endp);
                    return Target.Selection.Text;
                }
            }
            return null;
        }
        void ReplaceText()
        {
            if (Target != null)
            {
                FindPointer(out TextPointer startp, out TextPointer endp);
                if (startp != null && endp != null)
                {
                    Target.Selection.Select(startp, endp);
                }
                else
                {
                    Target.Selection.Select(Target.Document.ContentEnd, Target.Document.ContentEnd);
                }
                Target.Selection.Text = NewestText;
            }
        }
        void FindPointer(out TextPointer startp, out TextPointer endp)
        {
            startp = null; endp = null;
            if (Target != null)
            {
                IEnumerable<TextRange> startPointers = FindWordRange(Target, StartText);
                IEnumerable<TextRange> endPointers = FindWordRange(Target, EndText);
                if (startPointers.Count() > 0)
                {
                    startp = startPointers.First().Start;
                    if (endPointers.Count() > 0)
                    {
                        foreach (var item in endPointers)
                        {
                            if (item.Start.CompareTo(startPointers.First().End) > 0)//唯一右侧.
                            {
                                endp = item.End;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<TextRange> FindWordRange(RichTextBox richTextBox, string word)
        {
            List<TextRange> matchText = new List<TextRange>();
            TextPointer pointer = richTextBox.Document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    //带有内容的文本.
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);

                    //查找关键字在这文本中的位置.
                    int indexInRun = textRun.IndexOf(word);
                    int indexHistory = 0;
                    while (indexInRun >= 0)
                    {
                        TextPointer start = pointer.GetPositionAtOffset(indexInRun + indexHistory);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        matchText.Add(new TextRange(start, end));

                        indexHistory = indexHistory + indexInRun + word.Length;
                        textRun = textRun.Substring(indexInRun + word.Length);//去掉已经采集过的内容
                        indexInRun = textRun.IndexOf(word);//重新判断新的字符串是否还有关键字
                    }
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
            return matchText;
        }
    }
}
