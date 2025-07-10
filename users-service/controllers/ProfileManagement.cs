using System.Text.Json;
using DataTransferLib.CacheServices;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Financial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Common.Logs;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;
using Newtonsoft.Json;
using UsersService.DtoFactories;
using UsersService.Models;
using UsersService.Models.DbModels;
using InventoryRecordDto = DtoClassLibrary.DataTransferObjects.CasesItems.InventoryRecordDto;


namespace UsersService.controllers
{
    [Route("users")]
    [ApiController]
    public class ProfileManagement(
        AuthCommService authCommService,
        ApplicationDbContext dbContext,
        ILogger<ProfileManagement> logger,
        FinancialCommService financialCommService,
        UserService userService,
        CasesCommService casesCommService, CasesCache casesCache, ItemsCache itemsCache,
        AdminLogRabbitMqService adminLogRabbitMqService)
        : ControllerBase
    {

        [HttpGet("users-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsers();
            if (users.Count == 0)
            {
                return NotFound("No users found");
            }
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = await UserDtoFactory.CreateUserGetDto(user, casesCommService, casesCache, itemsCache);
                var balance = await financialCommService.GetUserBalance(user.Id!);
                if (balance != null)
                    userDto.CurrentBalance = balance.Result;
                userDtos.Add(userDto);
            }
            return new RequestService().GetResponse("Users:", userDtos);
        }


        [HttpPut("chanceboost")]
        public async Task<IActionResult> SetChanceBoostForPlayer([FromBody] ChanceBoostRequest request)
        {
            var user = await dbContext.User
                .Include(e => e.UserStatistics)
                .Where(p => p.Id == request.UserId).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();
            user.ChanceBoost = request.ChanceBoost;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("Chance boost updated: ", user.ChanceBoost);
        }


        [HttpPut("stats/update/upgrade")]
        public async Task<IActionResult> UpdateStatsAfterUpgrade([FromBody] UpdateStatsAfterUpgradeRequest request)
        {
            logger.LogWarning($"Starting to update after upgrade request: {JsonConvert.SerializeObject(request)}");

            var userStats = await dbContext.UserStatistics
                .Include(e => e.User)
                .Where(s => s.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (userStats == null)
                return NotFound("Пользователь не найден");

            // обновляем статистику
            userStats.UpgradesPlayed += 1;
            userStats.TotalCasesSpent += request.AddSpent;
            userStats.TotalCasesProfit += request.AddProfit;

            if (request.ResetFailScore)
                userStats.FailScore = 0;
            else
                userStats.FailScore += request.AddFailScore;

            // записываем в историю апгрейдов
            var upgradeRecord = new UpgradeHistoryRecord()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                ItemSpentId = request.ItemSpent.Id!,
                ItemResultId = request.ItemGot.Id!,
                IsSuccessful = request.ItemGot.Id != request.ItemSpent.Id,
                User = userStats.User!,
                DateOfUpgrade = DateTime.UtcNow
            };

            await dbContext.AddAsync(upgradeRecord);

            try
            {
                logger.LogWarning($"Saving upgrade history record: {upgradeRecord.Id}");
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError($"Error occured: {e.Message}");
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("Статистика апгрейда успешно обновлена", true);
        }

        [HttpPut("stats/update/contract")]
        public async Task<IActionResult> UpdateStatsAfterContract([FromBody] UpdateContractsStatisticsRequest request)
        {
            logger.LogWarning($"Starting to update after contract request: {JsonConvert.SerializeObject(request)}");

            var userStats = await dbContext.UserStatistics
                .Include(e => e.User)
                .Where(s => s.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (userStats == null)
                return NotFound("Пользователь не найден");


            userStats.ContractsPlaced += 1;
            userStats.TotalCasesSpent += request.AddSpent;
            userStats.TotalCasesProfit += request.AddProfit;

            if (request.ResetFailScore)
                userStats.FailScore = 0;
            else
                userStats.FailScore += request.AddFailScore;

            var contractHistoryRecord = new ContractHistoryRecord()
            {
                UserId = request.UserId,
                DateOfContract = DateTime.UtcNow,
                ItemsFromIds = request.ItemsUsedInContract,
                ResultItemId = request.ResultItem,
                User = userStats.User
            };

            await dbContext.ContractHistoryRecords.AddAsync(contractHistoryRecord);

            try
            {
                logger.LogWarning($"Saving contract history record: {contractHistoryRecord.Id}");
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError($"Error occured: {e.Message}");
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("Статистика по контракту обновлена", true);
        }


        [HttpPut("stats/update")]
        public async Task<IActionResult> UpdateStats([FromBody] UpdateStatisticsRequest request)
        {
            var userStats = await dbContext.UserStatistics
                .Where(s => s.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (userStats == null)
                return NotFound("Пользователь не найден");

            userStats.CasesBought += request.AddCasesBought;
            userStats.TotalCasesSpent += request.AddSpent;
            userStats.TotalCasesProfit += request.AddProfit;

            if (request.ResetFailScore)
                userStats.FailScore = 0;
            else
                userStats.FailScore += request.AddFailScore;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("Success", true);
        }


        [HttpGet("internal/{id}")]
        public async Task<IActionResult> GetInternalUserData(string id)
        {
            var userBase = await userService.GetUserById(id);
            if (userBase == null)
                return NotFound();
            var content = await UserDtoFactory.GetInternalUser(userBase, casesCommService, casesCache, itemsCache);
            var financeResult = await financialCommService.GetUserBalance(id);
            if (financeResult != null)
            {
                content.PublicData.CurrentBalance = financeResult.Result;
            }

            return new RequestService().GetResponse("User:", content);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            logger.LogInformation("getting user by id " + id);
            var userBase = await userService.GetUserById(id);


            if (userBase == null)
                return NotFound();

            var content = await UserDtoFactory.CreateUserGetDto(userBase, casesCommService, casesCache, itemsCache);
            var financeResult = await financialCommService.GetUserBalance(id);
            if (financeResult != null)
            {
                content.CurrentBalance = financeResult.Result;
                content.IsSubscribedToVk = (await authCommService.CheckVkSubscriptionStatus(userBase.Id)).Result;
                content.IsSubscribedToTg = (await authCommService.CheckTgSubscriptionStatus(userBase.Id)).Result;
            }

            return new RequestService().GetResponse("User:", content);
        }

        [HttpGet("mail/{mail}")]
        public async Task<IActionResult> GetUserByMail(string mail)
        {
            var userBase = await userService.GetUserByEmail(mail);
            if (userBase is null)
            {
                return NotFound();
            }

            var content = await UserDtoFactory.CreateUserGetDto(userBase, casesCommService, casesCache, itemsCache);
            try
            {
                if (content.Id != null)
                {
                    var balance = await financialCommService.GetUserBalance(content.Id);
                    if (balance != null)
                        content.CurrentBalance = balance.Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return new RequestService().GetResponse("User:", content);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await dbContext.User.FirstOrDefaultAsync(e => e.Id == request.Id);
            if (existingUser != null)
            {
                return Conflict("user already exists");
            }

            //Создание записи в таблице пользователей 
            var user = new User
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfRegistration = DateTime.UtcNow,
                IsDeleted = false,
                ProfileImagePath = request.ProfileImageUrl
            };

            //Созданеи записи в таблице ролей
            var userRole = new UserRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                UserId = user.Id,
            };

            user.UserRoleId = userRole.Id;

            //Создание записи в таблице статистики 
            var userStat = new UserStatistics()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                CasesBought = 0,
                OrdersPlaced = 0,
                CrashRocketsPlayed = 0,
                LuckBaraban = 0,
                PromocodesUsed = 0
            };


            user.UserStatisticsId = userStat.Id;

            //Создание записи в таблице инвентарь
            var userInventory = new UserInventory()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id
            };

            user.UserInventoryId = userInventory.Id;

            var blockStatus = new BlockStatus()
            {
                Id = Guid.NewGuid().ToString(),
                IsBlocked = false,
                PerformedById = "",
                Reason = "",
                UserId = user.Id
            };

            user.BlockStatusId = blockStatus.Id;


            try
            {
                await dbContext.UserInventory.AddAsync(userInventory);
                await dbContext.UserRole.AddAsync(userRole);
                await dbContext.UserStatistics.AddAsync(userStat);
                await dbContext.User.AddAsync(user);
                await dbContext.BlockStatus.AddAsync(blockStatus);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }


            await SendDashboardUpdateMessage();

            return new RequestService().GetResponse("User created:",
                await UserDtoFactory.CreateUserGetDto(user, casesCommService, casesCache, itemsCache));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] CreateUpdateUserRequest request)
        {
            var user = await dbContext.User
                .Include(e => e.UserStatistics)
                .Include(e => e.UserInventory)
                .Include(e => e.UserRole)
                .Include(e => e.BlockStatus)
                .Where(p => p.Id == id).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                user.UserName = request.UserName;
            }

            if (TryValidateModel(!string.IsNullOrEmpty(request.Phone)))
            {
                user.Phone = request.Phone;
            }

            dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("User updated: ",
                await UserDtoFactory.CreateUserGetDto(user, casesCommService, casesCache, itemsCache));
        }

        //TODO: Нужно добавить лог при обновлении данных о роли пользователя
        // PUT /users/{id}/role
        [HttpPut("role/update")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateRequest roleUpdateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUserRole = await dbContext.UserRole.Where(p => p.UserId == roleUpdateRequest.UserId)
                .FirstOrDefaultAsync();

            if (newUserRole == null)
                return NotFound();

            newUserRole.Name = roleUpdateRequest.RoleName;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            var roleDto = new UserRoleDto()
            {
                Id = newUserRole.Id,
                Name = newUserRole.Name
            };

            return new RequestService().GetResponse("User role updated: ", roleDto);
        }

        //TODO: Нужно добавить лог при блокировки пользователя
        [HttpPut("block")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUnblockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var blockStatus = await dbContext.BlockStatus.FirstOrDefaultAsync(b => b.UserId == request.UserId);
            if (blockStatus == null)
                return NotFound();

            blockStatus.IsBlocked = true;
            blockStatus.PerformedById = request.PerformedById;
            blockStatus.Reason = request.Reason;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            var block = new BlockStatusDto()
            {
                PerformedById = blockStatus.PerformedById,
                Id = blockStatus.Id,
                IsBlocked = blockStatus.IsBlocked,
                Reason = blockStatus.Reason
            };
            return new RequestService().GetResponse("User blocked: ", block);
        }

        [HttpPut("unblock")]
        public async Task<IActionResult> UnblockUser([FromBody] BlockUnblockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var unblockStatus = await dbContext.BlockStatus.FirstOrDefaultAsync(b => b.UserId == request.UserId);

            if (unblockStatus == null)
                return NotFound();

            unblockStatus.IsBlocked = false;
            unblockStatus.PerformedById = request.PerformedById;
            unblockStatus.Reason = request.Reason;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            var block = new BlockStatusDto()
            {
                PerformedById = unblockStatus.PerformedById,
                Id = unblockStatus.Id,
                IsBlocked = unblockStatus.IsBlocked,
                Reason = unblockStatus.Reason
            };
            return new RequestService().GetResponse("User blocked: ", block);
        }


        [HttpPost("additem")]
        public async Task<IActionResult> AddItemToInventory([FromBody] AddRemoveItemRequest addRemoveItemRequest)
        {
            logger.LogWarning(
                $"Starting request handling in inventory {JsonConvert.SerializeObject(addRemoveItemRequest)}");
            var inventory = dbContext.UserInventory
                .Include(e => e.InventoryRecords)
                .FirstOrDefault(e => e.UserId == addRemoveItemRequest.UserId);
            if (inventory?.InventoryRecords is null)
            {
                return new RequestService().GetResponse("Не найден инвентарь", false);
            }

            var newItemRecord = new InventoryItemRecord()
            {
                ItemId = addRemoveItemRequest.ItemInventoryRecordId,
                UserInventoryId = inventory.Id,
                ItemRecordState = addRemoveItemRequest.ItemRecordState,
                IsItemActive = true,
            };

            inventory.InventoryRecords.Add(newItemRecord);

            try
            {
                await dbContext.SaveChangesAsync();
                var itemRecordDto = new GameResultDto()
                {
                    Items = new List<InventoryRecordDto>(),
                    UserId = inventory.UserId
                };
                var item = await casesCommService.GetItem(addRemoveItemRequest.ItemInventoryRecordId);
                itemRecordDto.Items.Add(new InventoryRecordDto()
                {
                    IsItemActive = newItemRecord.IsItemActive,
                    ItemState = newItemRecord.ItemRecordState,
                    Item = item.Result,
                    InventoryRecordId = newItemRecord.Id
                });

                logger.LogWarning($"Item successfully added: {newItemRecord.Id}");
                return new RequestService().GetResponse("Предмет успешно добавлен", itemRecordDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new RequestService().GetResponse("Ошибка при сохранении инвентаря", false);
            }
        }

        [HttpPost("add/items")]
        public async Task<IActionResult> AddMultipleItemsToInventory([FromBody] AddRemoveMultipleItemsRequest request)
        {
            var inventory = dbContext.UserInventory
                .Include(e => e.InventoryRecords)
                .FirstOrDefault(e => e.UserId == request.UserId);
            if (inventory == null)
            {
                return new RequestService().GetResponse("Не найден инвентарь", false);
            }


            var resultDto = new GameResultDto()
            {
                Items = new List<InventoryRecordDto>(),
                UserId = inventory.UserId
            };
            foreach (var item in request.Items)
            {
                var newEntry = new InventoryItemRecord()
                {
                    ItemId = item,
                    UserInventoryId = inventory.Id,
                    ItemRecordState = ItemRecordState.FromCase,
                    IsItemActive = true,
                };
                inventory?.InventoryRecords?.Add(newEntry);
                var itemDto = await casesCommService.GetItem(item);
                resultDto.Items.Add(new InventoryRecordDto()
                {
                    IsItemActive = newEntry.IsItemActive,
                    Item = itemDto.Result,
                    ItemState = newEntry.ItemRecordState,
                    InventoryRecordId = newEntry.Id
                });
            }

            try
            {
                await dbContext.SaveChangesAsync();
                return new RequestService().GetResponse("Предмет успешно добавлен", resultDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new RequestService().HandleError("Ошибка при сохранении инвентаря");
            }
        }

        [HttpDelete("sell-item")]
        public async Task<IActionResult> SellItem([FromQuery] string itemId)
        {
            var inventoryRecord = await dbContext.ItemsUser
                .Include(e => e.UserInventory)
                .FirstOrDefaultAsync(e => e.Id == itemId);

            if (inventoryRecord == null)
            {
                logger.LogError("No item found with this id");
                return NotFound();
            }

            if (inventoryRecord.UserInventory == null)
            {
                logger.LogError("No inventory found for this item");
                return NotFound();
            }

            logger.LogInformation($"item record userid {inventoryRecord.UserInventory.UserId}");

            if (inventoryRecord.ItemRecordState == ItemRecordState.Sold)
            {
                return BadRequest("Item already sold");
            }

            inventoryRecord.IsItemActive = false;
            inventoryRecord.ItemRecordState = ItemRecordState.Sold;
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

            var item = await casesCommService.GetItem(inventoryRecord.ItemId);

            logger.LogInformation($"Item data {JsonConvert.SerializeObject(item)}");
            var transaction = new TransactionParams()
            {
                Amount = item.Result.SellPrice,
                PaymentType = PTYPE.Bank,
                Type = TTYPE.ItemSell,
                UserId = inventoryRecord.UserInventory.UserId,
            };
            var result = await financialCommService.SellItem(transaction);

            if (result)
            {
                return new RequestService().GetResponse("Предмет успешно продан", true);
            }
            else
            {
                return new RequestService().GetResponse("Ошибка при продаже предмета", false);
            }
        }


        //DELETE /users/{id}
        [HttpDelete("{id}/{performedById}")]
        public async Task<IActionResult> SoftDeleteUser(string id, string performedById)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deleteUser = await dbContext.User.Where(p => p.Id == id)
                .Include(e => e.BlockStatus)
                .Include(e => e.UserStatistics)
                .Include(e => e.UserRole)
                .Include(e => e.UserInventory)
                .ThenInclude(e => e.InventoryRecords)
                .FirstOrDefaultAsync();

            if (deleteUser == null)
                return NotFound();

            deleteUser.IsDeleted = true;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return new RequestService().GetResponse("User deleted: ",
                await UserDtoFactory.CreateUserGetDto(deleteUser, casesCommService, casesCache, itemsCache));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> FullDeleteUser(string id)
        {
            var userToDelete = await dbContext.User.FirstOrDefaultAsync(e => e.Id == id);
            try
            {
                dbContext.User.Remove(userToDelete);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        private async Task SendDashboardUpdateMessage()
        {
            var newUsersData =
                dbContext.User.Where(e => DateTime.UtcNow - e.DateOfRegistration <= TimeSpan.FromDays(10)).ToList();


            var allUsersBalances =
                await financialCommService.GetUsersBalance(dbContext.User.Select(e => e.Id).ToList()!);
            var allUsersBonusBalances =
                await financialCommService.GetUsersBonusBalance(dbContext.User.Select(e => e.Id).ToList()!);


            var newUsersDtos = await Task.WhenAll(newUsersData.Select(e => Task.FromResult(new UserShortDto
            {
                Id = e.Id,
                Username = e.Email ?? e.UserName,
                Balance = allUsersBonusBalances.Result[e.Id],
                BonusBalance = allUsersBonusBalances.Result[e.Id],
            })));

            var allUsersDtos = await Task.WhenAll(dbContext.User.Select(e => Task.FromResult(new UserShortDto
            {
                Id = e.Id,
                Username = e.Email ?? e.UserName,
                Balance = allUsersBalances.Result[e.Id],
                BonusBalance = allUsersBonusBalances.Result[e.Id],
            })));
            var dashboardData = new UsersDashBoardData
            {
                NewUsersCount = newUsersData.Count,
                UsersCount = dbContext.User.Count(),
                NewUsersData = newUsersDtos.ToList(),
                UserData = allUsersDtos.ToList()
            };

            var log = new AdminLog<UsersDashBoardData>()
            {
                Message = "Dashboard updated",
                ActionPerformedBy = "",
                Payload = dashboardData
            };
            await adminLogRabbitMqService.SendAdminLog(log);

        }
        private Task<bool> UserExists(string id)
        {
            return dbContext.User.AnyAsync(e => e.Id == id);
        }
    }
}