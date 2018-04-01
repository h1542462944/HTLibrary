using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.SoftWare.Service
{
    public class NotificationInfo
    {
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Button { get; set; }
        public string ButtonEvent { get; set; }
    }
}
