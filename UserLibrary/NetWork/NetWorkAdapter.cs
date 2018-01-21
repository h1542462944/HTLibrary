using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.NetWork
{
    public class NetworkAdapter
    {
        readonly NetworkMonitor linkNetworkMonitor;
        readonly string name;
        long downLoadSpeed;
        long upLoadSpeed;
        long downLoadValue;
        long upLoadValue;
        long downLoadValueOld;
        long upLoadValueOld;
        PerformanceCounter downLoadCounter;
        PerformanceCounter upLoadCounter;
        public event EventHandler Tick;

        /// <summary>
        /// 创建一个网速监控辅助器.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downLoadCount"></param>
        /// <param name="upLoadCount"></param>
        internal NetworkAdapter(string name, PerformanceCounter downLoadCount, PerformanceCounter upLoadCount, NetworkMonitor linkNetworkMonitor)
        {
            this.name = name;
            this.downLoadCounter = downLoadCount;
            this.upLoadCounter = upLoadCount;
            this.linkNetworkMonitor = linkNetworkMonitor;
        }

        public string Name => name;
        public NetworkSpeed DownLoadSpeed => new NetworkSpeed(downLoadSpeed);
        public NetworkSpeed UpLoadSpeed =>new NetworkSpeed(upLoadSpeed);
        public bool IsEnabled
        {
            get => linkNetworkMonitor.monitoredAdapters.Contains(this);
            set
            {
                if (value)
                {
                    linkNetworkMonitor.Start(this);
                }
                else
                {
                    linkNetworkMonitor.Stop(this);
                }
            }
        }

        /// <summary>
        /// 重置计数器.
        /// </summary>
        internal void ReSet()
        {
            downLoadValueOld = downLoadCounter.NextSample().RawValue;
            upLoadValueOld = upLoadCounter.NextSample().RawValue;
        }
        /// <summary>
        /// 刷新计数器.
        /// </summary>
        internal void Refresh()
        {
            downLoadValue = downLoadCounter.NextSample().RawValue;
            upLoadValue = upLoadCounter.NextSample().RawValue;
            downLoadSpeed = downLoadValue - downLoadValueOld;
            upLoadSpeed = upLoadValue - upLoadValueOld;
            downLoadValueOld = downLoadValue;
            upLoadValueOld = upLoadValue;
            Tick?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            return name;
        }
    }
    public struct NetworkSpeed
    {
        long rowSpeed;
        const string kbpsstring = "K/s";
        const string mbpsstring = "M/s";
        const string gbpsstring = "G/s";

        public NetworkSpeed(long rowSpeed)
        {
            this.rowSpeed = rowSpeed;
        }

        public long RowSpeed { get => rowSpeed; }
        public double SpeedKbps
        {
            get => ((int)(rowSpeed / 1024.0 * 100)) / 100.0;
       
        }
        public double SpeedMbps
        {
            get => ((int)(rowSpeed / 1024.0 / 1024.0 * 100)) / 100.0;
        }
        public double SpeedGbps
        {
            get => ((int)(rowSpeed / 1024.0 / 1024.0 / 1024.0 * 100) / 100.0);
        }
        public double Value
        {
            get
            {
                if (SpeedMbps >=1000.00)
                {
                    return SpeedGbps;
                }
                else if (SpeedKbps >= 1000.00)
                {
                    return SpeedMbps;
                }
                else 
                {
                    return SpeedKbps;
                }
            }
        }
        public string SpeedUnit
        {
            get
            {
                if (SpeedMbps >=1000.00)
                {
                    return gbpsstring;
                }
                else if (SpeedKbps >= 1000.00)
                {
                    return mbpsstring;
                }
                else
                {
                    return kbpsstring;
                }
            }
        }
        public string ValueUnit
        {
            get
            {
                return SpeedUnit.Substring(0, 1);
            }
        }
        public string SpeedString
        {
            get
            {
                if (SpeedKbps >= 1000.00 )
                {
                    return string.Format("{0} {1}", SpeedMbps, mbpsstring);
                }
                else
                {
                    return string.Format("{0} {1}", SpeedKbps, kbpsstring);
                }
            }
        }

        public override string ToString()
        {
            return SpeedString;
        }
    }
}
