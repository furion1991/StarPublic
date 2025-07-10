using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Repositories;

public interface IPrizeDrawResultUserServiceRepository : IUserServiceRepository<PrizeDrawResult>
{
    Task<PrizeDrawResult?> GetLastPrizeDrawResult();
}