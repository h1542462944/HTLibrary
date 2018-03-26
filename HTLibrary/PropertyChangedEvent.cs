using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User
{
    /// <summary>
    /// 属性改变或初始化加载时触发的事件数据.
    /// </summary>
    public sealed class UPropertyChangedEventArgs : EventArgs
    {
        object oldValue;
        object newValue;

        public UPropertyChangedEventArgs(object oldValue, object newValue)
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
    /// <summary>
    /// 属性改变或初始化加载时触发的事件数据.
    /// </summary>
    public sealed class UPropertyChangedEventArgs<TValue> : EventArgs
    {
        TValue oldValue;
        TValue newValue;

        public UPropertyChangedEventArgs(TValue oldValue, TValue newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        /// <summary>
        /// 设置改变前的值.
        /// </summary>
        public TValue OldValue => oldValue;
        /// <summary>
        /// 设置改变后的值.
        /// </summary>
        public TValue NewValue => newValue;
        /// <summary>
        ///是否是初始化的设置.
        /// </summary>
        public bool IsNewest => OldValue == null;

        public static implicit operator UPropertyChangedEventArgs(UPropertyChangedEventArgs<TValue> arg)
        {
            return new UPropertyChangedEventArgs(arg.oldValue, arg.newValue);
        }
    }
    public delegate void UPropertyChangedEventHandler(object sender, UPropertyChangedEventArgs e);
    public delegate void UPropertyChangedEventHandler<TValue>(object sender, UPropertyChangedEventArgs<TValue> e);
}
