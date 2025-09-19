using FitHub.Infrastructure.Notification;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Infrastructure.BackgroundJobs
{
    public static class HangfireJobs
    {
        public static void RegisterDailyNotification()
        {
            RecurringJob.AddOrUpdate<NotificationService>(
            "daily-reminder",
            job => job.DailyNotification(),
            Cron.Daily(9)
            );
        }
    }
}
