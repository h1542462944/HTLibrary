using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace User.UI
{
    public sealed class PageNavigationHelper
    {
        List<Type> pages = new List<Type>();
        public PageNavigationHelper()
        {
        }
        public void Add(Type page)
        {
            pages.Add(page);
            PageChanged?.Invoke(this, new PageNavigationEventargs(pages.Count == 1, page));
        }
        public void Back()
        {
            if (pages.Count > 1)
            {
                pages.RemoveAt(pages.Count - 1);
                PageChanged?.Invoke(this, new PageNavigationEventargs(pages.Count == 1, pages[pages.Count - 1]));
            }
        }
        public event PageNavigationEventHandler PageChanged;
        public IReadOnlyList<Type> Pages => pages;
    }
    public sealed class PageNavigationEventargs : EventArgs
    {
        bool isFirstPage;
        Type page;

        public PageNavigationEventargs(bool isFirstPage, Type page)
        {
            this.isFirstPage = isFirstPage;
            this.page = page;
        }

        public bool IsFirstPage { get => isFirstPage; }
        public Type Page { get => page; }
    }
    public delegate void PageNavigationEventHandler(object sender, PageNavigationEventargs e);
}
