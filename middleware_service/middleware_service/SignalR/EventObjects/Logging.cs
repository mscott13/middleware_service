using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.SignalR.EventObjects
{
    class Logging
    {
        public Logging()
        {
            eventType = "EVT_LOGGING";
        }

        public Logging(string msg)
        {
            eventType = "EVT_LOGGING";
            message = msg;
            date = DateTime.Now;
        }

        public string eventType { get; }
        public string message { get; }
        public DateTime date { get; }
    }
}
