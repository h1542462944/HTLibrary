using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace User.UI
{
    /// <summary>
    /// 统一管理的计时器清单.
    /// </summary>
    /// <typeparam name="Tkey">键的类型</typeparam>
    public sealed class TimerInventory<Tkey>
    {
        Dictionary<Tkey, TimerQueueInfo> inventory = new Dictionary<Tkey, TimerQueueInfo>();
        DispatcherTimer innertimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        public void Register(Tkey diaplayName, TimerQueueInfo arg)
        {
            if (!inventory.TryGetValue(diaplayName, out TimerQueueInfo n))
            {
                inventory.Add(diaplayName, arg);
            }
        }
        public void UnRegister(Tkey diaplayName)
        {
            if (inventory.TryGetValue(diaplayName, out TimerQueueInfo n))
            {
                inventory.Remove(diaplayName);
            }
        }
        public TimerQueueInfo this[Tkey displayName]
        {
            get
            {
                this.inventory.TryGetValue(displayName, out TimerQueueInfo que);
                return que;
            }
        }
        public TimerInventory()
        {
            innertimer.Start();
            innertimer.Tick += Innertimer_Tick;
        }
        private void Innertimer_Tick(object sender, EventArgs e)
        {
            foreach (var item in inventory)
            {
                try
                {
                    TimerQueueInfo arg = item.Value;
                    if (arg.IsStarted && (arg.TickTime == -1 || arg.TickTime >= arg.Intervel))
                    {
                        arg.CallBack?.Invoke(item, new EventArgs());
                    }
                    arg.TickTime++;
                    if (arg.TickTime > arg.Intervel)
                    {
                        arg.TickTime = 1;
                    }
                }
                catch
                {
                }
            }
        }
    }
    /// <summary>
    /// 计时器数据,用于TimerInventory
    /// </summary>
    public sealed class TimerQueueInfo
    {
        EventHandler callBack;
        int intervel;
        int tickTime = 0;
        bool isStarted = false;

        public EventHandler CallBack { get => callBack; set => callBack = value; }
        public int Intervel { get => intervel; set => intervel = value; }
        public bool IsStarted { get => isStarted; set => isStarted = value; }
        public int TickTime { get => tickTime; set => tickTime = value; }

        public TimerQueueInfo(int intervel, EventHandler CallBack, bool firsttick = false, bool isstarted = true)
        {
            if (intervel < 1)
            {
                throw new ArgumentException("inverval的值为非法值.");
            }
            this.intervel = intervel;
            this.callBack = CallBack;
            this.isStarted = isstarted;
            if (!firsttick)
            {
                TickTime = 0;
            }
            else
            {
                TickTime = -1;
            }
        }
    }
    public enum DialogX
    {
        Left,
        StarLeft,
        StarRight,
        Right
    }
    public enum DialogY
    {
        Top,
        StarTop,
        StarButtom,
        Buttom
    }
    public enum DialogAuto
    {
        Absolute,
        Star
    }
    public enum DialogType
    {
        Shadow,
        Dialog,
    }
    /// <summary>
    /// 用于Wpf对话框的辅助类.
    /// </summary>
    /// <typeparam name="Tkey">键的类型</typeparam>
    public sealed class DialogInventory<Tkey>
    {
        private Dictionary<Tkey, DialogInfo> inventory = new Dictionary<Tkey, DialogInfo>();
        public void Show(Tkey displayName, DialogInfo arg)
        {
            if (!inventory.TryGetValue(displayName, out DialogInfo n))
            {
                inventory.Add(displayName, arg);
                if (arg.DialogX == DialogX.Left || arg.DialogX == DialogX.StarLeft)
                {
                    arg.Dialog.HorizontalAlignment = HorizontalAlignment.Left;
                    if (arg.DialogY == DialogY.Top || arg.DialogY == DialogY.StarTop)
                    {
                        arg.Dialog.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else
                    {
                        arg.Dialog.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                }
                else
                {
                    arg.Dialog.HorizontalAlignment = HorizontalAlignment.Right;
                    if (arg.DialogY == DialogY.Top || arg.DialogY == DialogY.StarTop)
                    {
                        arg.Dialog.VerticalAlignment = VerticalAlignment.Top;
                    }
                    else
                    {
                        arg.Dialog.VerticalAlignment = VerticalAlignment.Bottom;
                    }
                }
                arg.Dialog.Margin = new Thickness((1 - GetP(arg.DialogX)) * arg.Point.X, (1 - GetP(arg.DialogY)) * arg.Point.Y, GetP(arg.DialogX) * arg.Point.X, GetP(arg.DialogY) * arg.Point.Y);
                if (arg.DialogType == DialogType.Dialog)
                {
                    arg.Back.Visibility = Visibility.Visible;
                    arg.Back.Children.Add(arg.Dialog);
                }
                else
                {
                    arg.Dialog.Visibility = Visibility.Visible;
                }
            }
        }
        public void Hide(Tkey displayName)
        {
            if (inventory.TryGetValue(displayName, out DialogInfo arg))
            {
                if (arg.DialogType == DialogType.Dialog)
                {
                    arg.Back.Children.Remove(arg.Dialog);
                    arg.Back.Visibility = Visibility.Hidden;
                }
                else
                {
                    arg.Dialog.Visibility = Visibility.Hidden;
                }
                inventory.Remove(displayName);
            }
        }
        public void Show(Tkey displayName, DialogAutoInfo arg)
        {
            Show(displayName, arg.GetDialogInfo());
        }
        public bool Exists(Tkey diaplayName)
        {
            return inventory.TryGetValue(diaplayName, out DialogInfo n);
        }
        public void Move()
        {
            foreach (var item in inventory.Values)
            {
                double[] v = new double[2];
                v[0] = item.Point.X; v[1] = item.Point.Y;
                if (item.DialogX == DialogX.StarLeft || item.DialogX == DialogX.StarRight)
                    v[0] = item.Analyse.X * item.Back.ActualWidth;
                if (item.DialogY == DialogY.StarTop || item.DialogY == DialogY.StarButtom)
                    v[1] = item.Analyse.Y * item.Back.ActualHeight;
                item.Dialog.Margin = new Thickness((1 - GetP(item.DialogX)) * v[0], (1 - GetP(item.DialogY)) * v[1], GetP(item.DialogX) * v[0], GetP(item.DialogY) * v[1]);
            }
        }
        private int GetP(DialogX arg)
        {
            if (arg == DialogX.Left || arg == DialogX.StarLeft)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private int GetP(DialogY arg)
        {
            if (arg == DialogY.Top || arg == DialogY.StarTop)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
    public sealed class DialogInfo
    {
        Grid back;
        FrameworkElement dialog;
        Point analyse;
        Point point;
        DialogX dialogX;
        DialogY dialogY;
        DialogType dialogType;

        public DialogInfo(FrameworkElement dialog, Point point, DialogX dialogX, DialogY dialogY, DialogType dialogType, Grid back)
        {
            this.back = back;
            this.dialog = dialog;
            this.dialogX = dialogX;
            this.dialogY = dialogY;
            this.dialogType = dialogType;
            this.point = point;
            double[] v = new double[2];
            if (dialogX == DialogX.Left || dialogX == DialogX.Right)
            {
                v[0] = point.X;
            }
            else
            {
                v[0] = point.X / back.ActualWidth;
            }
            if (dialogY == DialogY.Top || dialogY == DialogY.Buttom)
            {
                v[1] = point.Y;
            }
            else
            {
                v[1] = point.Y / back.ActualHeight;
            }
            analyse = new Point(v[0], v[1]);
        }

        public Grid Back => back;
        public FrameworkElement Dialog => dialog;
        public Point Point => point;
        public Point Analyse => analyse;
        public DialogX DialogX => dialogX;
        public DialogY DialogY => dialogY;
        public DialogType DialogType => dialogType;
    }
    /// <summary>
    /// 以自动化方式处理Dialog,用于DialogInventory
    /// </summary>
    public sealed class DialogAutoInfo
    {
        Grid back;
        FrameworkElement dialog;
        DialogAuto dialogAuto;
        Point point;
        DialogType dialogType;
        double offset;
        Size dialogsize;

        public DialogAutoInfo(Grid back, FrameworkElement dialog, DialogAuto dialogAuto, Point point, DialogType dialogType, double offset)
        {
            this.back = back;
            this.dialog = dialog;
            this.dialogAuto = dialogAuto;
            this.point = point;
            this.dialogType = dialogType;
            this.offset = offset;
            this.dialogsize = new Size(dialog.ActualWidth, dialog.ActualHeight);
        }
        public DialogAutoInfo(Grid back, FrameworkElement dialog, DialogAuto dialogAuto, Point point, DialogType dialogType, double offset, Size size)
        {
            this.back = back;
            this.dialog = dialog;
            this.dialogAuto = dialogAuto;
            this.point = point;
            this.dialogType = dialogType;
            this.offset = offset;
            this.dialogsize = size;
        }

        Grid Back => back;
        FrameworkElement Dialog => dialog;
        DialogAuto DialogAuto => DialogAuto;
        Point Point => point;
        DialogType DialogType => DialogType;
        double Offset => offset;
        Size DialogSize => dialogsize;

        /// <summary>
        /// 返回可识别的DialogInfo对象.
        /// </summary>
        public DialogInfo GetDialogInfo()
        {
            int Tx = GetP(out DialogX dialogX, out double X);
            int Ty = GetP(out DialogY dialogY, out double Y);
            return new DialogInfo(dialog, new Point(X, Y), dialogX, dialogY, dialogType, back);
        }

        private int GetP(out DialogX dialogX, out double X)
        {
            if (point.X + dialogsize.Width + offset > back.ActualWidth)
            {
                if (dialogAuto == DialogAuto.Absolute)
                {
                    dialogX = DialogX.Right;
                }
                else
                {
                    dialogX = DialogX.StarRight;
                }
                X = back.ActualWidth - point.X + offset;
                return 1;
            }
            else
            {
                if (dialogAuto == DialogAuto.Absolute)
                {
                    dialogX = DialogX.Left;
                }
                else
                {
                    dialogX = DialogX.StarLeft;
                }
                X = point.X + offset;
                return 0;
            }
        }
        private int GetP(out DialogY dialogY, out double Y)
        {
            Console.WriteLine(dialog.ActualHeight);
            if (point.Y + DialogSize.Height + offset > back.ActualHeight)
            {
                if (dialogAuto == DialogAuto.Absolute)
                {
                    dialogY = DialogY.Buttom;
                }
                else
                {
                    dialogY = DialogY.StarButtom;
                }
                Y = back.ActualHeight - point.Y + offset;
                return 1;
            }
            else
            {
                if (dialogAuto == DialogAuto.Absolute)
                {
                    dialogY = DialogY.Top;
                }
                else
                {
                    dialogY = DialogY.StarTop;
                }
                Y = point.Y + offset;
                return 0;
            }
        }
    }
}
