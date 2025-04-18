using FitHub.Business.Dtos;
using FitHub.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Infrastructure.Notification
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(int userid ,NotificationDto message)
        {
            await _hubContext.Clients.Users(userid.ToString()).SendAsync("SendNotification", message);
        }

        public async Task DailyNotification()
        {
            await _hubContext.Clients.All.SendAsync("SendNotification", "Pleasent Day");
        }
    }
}
