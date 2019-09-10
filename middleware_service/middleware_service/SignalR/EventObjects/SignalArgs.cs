using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.SignalR.EventObjects
{
    public class SignalArgs : EventArgs
    {
        public SignalArgs(string msg, string from, string uid)
        {
            message = msg;
            date = DateTime.Now;
            username = from;
            clientId = uid;
        }
        public string message { get; }
        public string username { get; }
        public string clientId { get; set; }
        public DateTime date { get; }
    }
}
