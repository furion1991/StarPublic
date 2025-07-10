using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using Microsoft.AspNetCore.SignalR;

namespace AuditService.Services;

public class DashboardService(IHubContext<DashboardHub> dashboardHub)
{
    public async Task UpdateDashboardAsync(DashboardDto dashboard)
    {
        await dashboardHub.Clients.All.SendAsync("dashboard", dashboard);
    }
}