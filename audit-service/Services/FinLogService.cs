using AuditService.Database;
using AuditService.Repositories;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;

namespace AuditService.Services;

public class FinLogService(ApplicationDbContext dbContext)
{

    public async Task<DashboardBalanceDto> GetDashboardBalanceAsync()
    {
        //var balances = dbContext.fin
        return null;
    }
}