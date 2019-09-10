using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.SignalR.EventObjects
{
    public class Logging
    {
        public Logging()
        {
            eventType = "evt_logging";
        }

        public Logging(string msg)
        {
            eventType = "evt_logging";
            message = msg;
            date = DateTime.Now;
        }

        public string eventType { get; }
        public string message { get; }
        public DateTime date { get; }
    }
}
