using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Repositories;

public interface IPrizeUserServiceRepository : IUserServiceRepository<PrizeDraw>
{
    Task<PrizeDraw?> GetCurrentPrizeDraw();
}