using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace User.NetWork
{
    public class NetworkMonitor
    {
        private DispatcherTimer innertimer;
        List<NetworkAdapter> adapters = new List<NetworkAdapter>();
        internal List<NetworkAdapter> monitoredAdapters = new List<NetworkAdapter>();

        public NetworkMonitor()
        {
            GetNetworkAdapters();
            innertimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
                IsEnabled = false  
            };
            innertimer.Tick += Innertimer_Tick;
        }
        public NetworkAdapter[] Adapters => adapters.ToArray();

        public void Start()
        {
            if (adapters.Count >0)
            {
                foreach (var item in adapters)
                {
                    if (!monitoredAdapters.Contains(item))
                    {
                        monitoredAdapters.Add(item);
                        item.ReSet();
                    }
                }
                innertimer.IsEnabled = true;
            }
        }
        public void Stop()
        {
            monitoredAdapters.Clear();
            innertimer.IsEnabled = false;
        }
        internal void Start(NetworkAdapter adapter)
        {
            if (!monitoredAdapters.Contains(adapter))
            {
                monitoredAdapters.Add(adapter);
                adapter.ReSet();
            }
            innertimer.IsEnabled = true;
        }
        internal void Stop(NetworkAdapter adapter)
        {
            if (monitoredAdapters.Contains(adapter))
                monitoredAdapters.Remove(adapter);
            if (monitoredAdapters.Count == 0)
                innertimer.IsEnabled = false;
        }

        private void Innertimer_Tick(object sender, EventArgs e)
        {
            foreach (var item in monitoredAdapters)
                item.Refresh();
        }
        private void GetNetworkAdapters()
        {
            const string networkname = "Network Interface";
            adapters = new List<NetworkAdapter>();
            PerformanceCounterCategory category = new PerformanceCounterCategory(networkname);
            foreach (var name in category.GetInstanceNames())
            {
                if (name == "MS TCP Loopback interface")
                    continue;
                NetworkAdapter adapter = new NetworkAdapter(name,
                    new PerformanceCounter(networkname, "Bytes Received/sec", name),
                    new PerformanceCounter(networkname, "Bytes Sent/sec", name),
                    this);
                adapters.Add(adapter);
            }
        }
    }
}
