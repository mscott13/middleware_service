using Microsoft.AspNet.SignalR;
using middleware_service.Other_Classes;
using middleware_service.SignalR.EventObjects;
using System;
using System.Threading.Tasks;

namespace middleware_service
{
    public class EventHub : Hub
    {
  
        public override Task OnConnected()
        {
            var username = Context.QueryString["username"];
            string connectionId = Context.ConnectionId;
            new Log().Save("User connected: " + username + ", clientId: " + connectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;
            new Log().Save("User disconnected: "+ connectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string connectionId = Context.ConnectionId;
            new Log().Save("User reconnected: " + connectionId);
            return base.OnReconnected();
        }
 
        public void Send(string action, string data)
        {
            EventBridge.OnSignalReceived(this, new SignalArgs(action, data, Context.ConnectionId));
        }
    }
}