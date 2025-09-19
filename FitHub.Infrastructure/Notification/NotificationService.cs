using FitHub.Business.Dtos;
using FitHub.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Infrastructure.Notification
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(string userEmail ,NotificationDto message)
        {
            await _hubContext.Clients.Users(userEmail).SendAsync("SendNotification", message);
        }

        public async Task DailyNotification()
        {
            await _hubContext.Clients.All.SendAsync("SendNotification", "Have a Pleasent Day");
        }
    }
}
