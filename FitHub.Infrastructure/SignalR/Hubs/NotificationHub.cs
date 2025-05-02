using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Infrastructure.SignalR.Hubs
{
    public class NotificationHub:Hub
    { 
       public async Task SendMessage(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public async Task SendToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
