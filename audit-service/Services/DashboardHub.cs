using DataTransferLib.CommunicationsServices;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using Microsoft.AspNetCore.SignalR;

namespace AuditService.Services;

public class DashboardHub(CasesCommService casesCommService,
    UsersCommService usersCommService,
    OpenedCasesService openedCasesService) : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public async Task UpdateDashboard()
    {
    }

    
}