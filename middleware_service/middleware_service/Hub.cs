using Microsoft.AspNet.SignalR;

namespace middleware_service
{
    public class EventHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }
}