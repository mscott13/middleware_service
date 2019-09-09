using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace middleware_service
{
    public class EventHub : Hub
    {
        public override Task OnConnected()
        {
            var username = Context.QueryString["username"];
            string connectionId = Context.ConnectionId;
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string connectionId = Context.ConnectionId;
            return base.OnReconnected();
        }

        public void Send(string name, string message)
        {
            Clients.All.sendMessage(name, message);
        }
    }
}