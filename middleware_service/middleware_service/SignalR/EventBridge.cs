using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service
{
    public static class EventBridge
    {
        public delegate void SignalEventHandler(object source, EventArgs args);
        public static event SignalEventHandler SignalReceived;

        public static void OnSignalReceived(object e, EventArgs args)
        {
            SignalReceived?.Invoke(e, args);
        }
    }
}
