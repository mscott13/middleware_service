using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.SignalR.EventObjects
{
    public class SignalArgs : EventArgs
    {
        public SignalArgs(string action, string data, string uid)
        {
            this.action = action;
            date = DateTime.Now;
            this.data = data;
            clientId = uid;
        }

        public string action { get; }
        public string data { get; }
        public string clientId { get; set; }
        public DateTime date { get; }
    }
}
