using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace FitHub.Endpoints.Filters
{
    public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
