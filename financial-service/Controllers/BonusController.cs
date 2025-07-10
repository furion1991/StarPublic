using DataTransferLib.CommunicationsServices;
using DtoClassLibrary.DataTransferObjects.Bonus;
using Microsoft.AspNetCore.Mvc;
using FinancialService.Database;
using FinancialService.Database.Models.Bonuses;
using FinancialService.Services;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Controllers;

[Route("bonus")]
[ApiController]
public class BonusController(ILogger<BonusController> logger, ApplicationDbContext dbContext, BonusService bonusService)
    : ControllerBase
{
    [HttpGet("get/all")]
    public async Task<IActionResult> GetAllBonuses()
    {
        var list = await dbContext.Bonuses
            .Where(b => !b.IsDeleted)
            .Select(e => e.GetBonusDto()).ToListAsync();

        return new RequestService().GetResponse("List of all bonuses", list);
    }

    [HttpGet("latest/{userId}")]
    public async Task<IActionResult> GetLatestUserBonus(string userId)
    {
        var lastBonus = await bonusService.GetLatestUserBonus(userId);

        if (lastBonus == null)
        {
            return NotFound();
        }


        try
        {
            var bonusRecordDto = new UserBonusRecordDto()
            {
                Bonus = lastBonus.Bonus.GetBonusDto(),
                BonusId = lastBonus.BonusId,
                FinDataId = lastBonus.FinDataId,
                Id = lastBonus.Id,
                TimeGotBonus = lastBonus.TimeGotBonus
            };
            return new RequestService().GetResponse("Latest user bonus is: ", bonusRecordDto);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(500, e.Message);
        }
    }





    [HttpDelete("delete/{bonusId}")]
    public async Task<IActionResult> DeleteBonus(string bonusId)
    {
        var bonusToDelete = await dbContext.Bonuses.FirstOrDefaultAsync(b => b.Id == bonusId);
        if (bonusToDelete != null)
        {
            try
            {
                bonusToDelete.IsDeleted = true;
                await dbContext.SaveChangesAsync();
                return Ok("Bonus deleted safely");
            }
            catch (Exception e)
            {
                logger.LogError("Error, while deleting bonus");
                logger.LogError(e.Message);
                return StatusCode(500);
            }
        }

        return NotFound();
    }

    [HttpDelete("delete_full/{bonusId}")]
    public async Task<IActionResult> FullDeleteBonus(string bonusId)
    {
        var bonusToDelete = await dbContext.Bonuses.FirstOrDefaultAsync(b => b.Id == bonusId);

        if (bonusToDelete == null)
        {
            return NotFound();
        }
        var userBonusRecordsWithThisBonus = await dbContext.UserBonusRecords
            .Include(b => b.Bonus)
            .Where(b => b.Bonus.Id == bonusId)
            .ToListAsync();

        try
        {
            foreach (var record in userBonusRecordsWithThisBonus)
            {
                record.IsUsed = true;
                record.BonusId = string.Empty;
                record.Bonus = null;
            }

            dbContext.Bonuses.Remove(bonusToDelete);
            await dbContext.SaveChangesAsync();
            return new RequestService().GetResponse("Bonus fully deleted", bonusToDelete.Id);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return NotFound();
    }


    [HttpPost("wheel/add/{userId}")]
    public async Task<IActionResult> AddRandomBonusFromWheel(string userId)
    {
        var bonuses = dbContext.Bonuses.ToList();
        var totalChance = bonuses.Sum(b => b.DropChance);
        var random = new Random();

        var randomNumber = random.NextDouble() * (double)totalChance;

        var finRecord = await dbContext.FinancialDatas.FirstOrDefaultAsync(f => f.UserId == userId);
        if (finRecord == null)
        {
            return NotFound();
        }

        Bonus randomBonus = null;

        foreach (var bonus in bonuses)
        {
            randomNumber -= (double)bonus.DropChance;
            if (randomNumber <= 0)
            {
                randomBonus = bonus;
                break;
            }
        }

        if (randomBonus == null)
            return new RequestService().HandleError(new InvalidOperationException("No bonus chosen, try again"));

        var newBonusRecord = new UserBonusRecord()
        {
            Bonus = randomBonus,
            BonusId = randomBonus.Id,
            FinDataId = finRecord.Id,
            FinancialData = finRecord,
            IsUsed = false,
            IsWheelBonus = true,
            TimeGotBonus = DateTime.UtcNow,
        };
        try
        {
            await dbContext.UserBonusRecords
                .Include(e => e.FinancialData)
                .Include(e => e.Bonus)
                .Where(b => b.FinancialData.UserId == userId)
                .Where(b => !b.IsUsed && b.IsWheelBonus)
                .ForEachAsync(b => b.IsUsed = true);
            await dbContext.UserBonusRecords.AddAsync(newBonusRecord);
            await dbContext.SaveChangesAsync();

            var bonusRecordDto = new UserBonusRecordDto()
            {
                Bonus = newBonusRecord.Bonus.GetBonusDto(),
                BonusId = newBonusRecord.BonusId,
                FinDataId = newBonusRecord.FinDataId,
                Id = newBonusRecord.Id,
                TimeGotBonus = newBonusRecord.TimeGotBonus
            };

            return new RequestService().GetResponse("Added bonus from wheel", bonusRecordDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding bonus from wheel");
            logger.LogError(e.Message);
            return new RequestService().HandleError(e);
        }
    }


    [HttpPost("add/balance/bonus")]
    public async Task<IActionResult> AddBalanceBonus([FromBody] BalanceBonusDto balanceBonusDto)
    {
        var bonus = new BalanceBonus()
        {
            Name = balanceBonusDto.Name,
            Amount = balanceBonusDto.Amount,
            BonusImage = balanceBonusDto.BonusImage,
            BonusType = BonusType.BalanceBonus,
            Description = balanceBonusDto.Description,
            ImageForDepositView = balanceBonusDto.ImageForDepositView,
            DropChance = balanceBonusDto.DropChance
        };

        try
        {
            dbContext.Add(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();
            balanceBonusDto.Id = bonus.Id;
            balanceBonusDto.BonusImage = bonus.BonusImage;
            balanceBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            return new RequestService().GetResponse("Added balance bonus", balanceBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest();
        }
    }


    [HttpPost("add/cashback/bonus")]
    public async Task<IActionResult> AddCashbackBonus([FromBody] CashbackBonusDto cashbackBonusDto)
    {
        var bonus = new CashbackBonus()
        {
            Name = cashbackBonusDto.Name,
            BonusType = BonusType.CashbackBonus,
            CashbackPercentage = cashbackBonusDto.CashbackPercentage,
            Description = cashbackBonusDto.Description,
            Duration = cashbackBonusDto.Duration,
            DropChance = cashbackBonusDto.DropChance
        };

        try
        {
            await dbContext.AddAsync(bonus);

            SetBonusImageUrls(bonus);

            await dbContext.SaveChangesAsync();

            cashbackBonusDto.BonusImage = bonus.BonusImage;
            cashbackBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            cashbackBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added cashback service", cashbackBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding cashback bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/deposit/bonus")]
    public async Task<IActionResult> AddDepositBonus([FromBody] DepositBonusDto desBonusDto)
    {
        var bonus = new DepositBonus()
        {
            Name = desBonusDto.Name,
            Description = desBonusDto.Description,
            BonusType = BonusType.DepositBonus,
            DepositCap = desBonusDto.DepositCap,
            Mtype = desBonusDto.Mtype,
            BonusMultiplier = desBonusDto.BonusMultiplier,
            DropChance = desBonusDto.DropChance
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            desBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            desBonusDto.BonusImage = bonus.ImageForDepositView;
            desBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added deposit bonus", desBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding deposit bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/discount/bonus")]
    public async Task<IActionResult> AddDiscountBonus([FromBody] DiscountBonusDto discountBonusDto)
    {
        var bonus = new DiscountBonus()
        {
            Name = discountBonusDto.Name,
            Description = discountBonusDto.Description,
            BonusType = BonusType.DiscountBonus,
            DiscountPercentage = discountBonusDto.DiscountPercentage,
            DropChance = discountBonusDto.DropChance
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            discountBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            discountBonusDto.BonusImage = bonus.ImageForDepositView;
            discountBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added discount bonus", discountBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in discount adding");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/freecase/bonus")]
    public async Task<IActionResult> AddFreeCaseBonus([FromBody] FreeCaseBonusDto freeCaseBonusDto)
    {
        var bonus = new FreeCaseBonus()
        {
            Name = freeCaseBonusDto.Name,
            Description = freeCaseBonusDto.Description,
            BonusType = BonusType.FreeCaseBonus,
            CaseCount = freeCaseBonusDto.CaseCount,
            MinimumDeposit = freeCaseBonusDto.MinimumDeposit,
            DropChance = freeCaseBonusDto.DropChance
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            freeCaseBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            freeCaseBonusDto.BonusImage = bonus.ImageForDepositView;
            freeCaseBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added free case bonus", freeCaseBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding freecase bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/item/bonus")]
    public async Task<IActionResult> AddItemBonusDto([FromBody] ItemBonusDto itemBonusDto)
    {
        var bonus = new ItemBonus()
        {
            Name = itemBonusDto.Name,
            BonusType = BonusType.ItemBonus,
            Description = itemBonusDto.Description,
            ItemCount = itemBonusDto.ItemCount,
            MinimumDeposit = itemBonusDto.MinimumDeposit,
            DropChance = itemBonusDto.DropChance,
            ItemMaximalCost = itemBonusDto.MaximalItemCost,
            ItemMinimalCost = itemBonusDto.MinimalItemCost
        };


        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            itemBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            itemBonusDto.BonusImage = bonus.ImageForDepositView;
            itemBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added item bonus", itemBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding item bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }


    [HttpPost("add/randomcase/bonus")]
    public async Task<IActionResult> AddRandomCaseBonus([FromBody] RandomCaseBonusDto randomCaseBonusDto)
    {
        var bonus = new RandomCaseBonus()
        {
            Name = randomCaseBonusDto.Name,
            BonusType = BonusType.RandomCaseBonus,
            Description = randomCaseBonusDto.Description,
            MinimumDeposit = randomCaseBonusDto.MinimumDeposit,
            DropChance = randomCaseBonusDto.DropChance
        };


        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            randomCaseBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            randomCaseBonusDto.BonusImage = bonus.ImageForDepositView;
            randomCaseBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added item bonus", randomCaseBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding item bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/wheelspin/bonus")]
    public async Task<IActionResult> AddWheelSpinBonus([FromBody] WheelSpinBonusDto wheelSpinBonusDto)
    {
        var bonus = new WheelSpinBonus()
        {
            Name = wheelSpinBonusDto.Name,
            BonusType = BonusType.FreeSpinBonus,
            Description = wheelSpinBonusDto.Description,
            ExtraSpins = wheelSpinBonusDto.ExtraSpins,
            DropChance = wheelSpinBonusDto.DropChance
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            wheelSpinBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            wheelSpinBonusDto.BonusImage = bonus.ImageForDepositView;
            wheelSpinBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added item bonus", wheelSpinBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding item bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("add/letter/bonus")]
    public async Task<IActionResult> AddLetterBonus([FromBody] LetterBonusDto letterBonusDto)
    {
        var bonus = new LetterBonus()
        {
            Name = letterBonusDto.Name,
            BonusType = BonusType.LetterBonus,
            Description = letterBonusDto.Description,
            DropChance = letterBonusDto.DropChance,
            Letter = letterBonusDto.Letter
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            letterBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            letterBonusDto.BonusImage = bonus.ImageForDepositView;
            letterBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added letter bonus", letterBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding item bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }


    [HttpPost("add/fivek/bonus")]
    public async Task<IActionResult> AddFiveKBonus([FromBody] FiveKBonusDto fiveKBonusDto)
    {
        var bonus = new FiveKBonus()
        {
            Name = fiveKBonusDto.Name,
            BonusType = BonusType.FiveKBonus,
            Description = fiveKBonusDto.Description,
            DropChance = fiveKBonusDto.DropChance,
        };

        try
        {
            await dbContext.AddAsync(bonus);
            SetBonusImageUrls(bonus);
            await dbContext.SaveChangesAsync();

            fiveKBonusDto.ImageForDepositView = bonus.ImageForDepositView;
            fiveKBonusDto.BonusImage = bonus.ImageForDepositView;
            fiveKBonusDto.Id = bonus.Id;
            return new RequestService().GetResponse("Added letter bonus", fiveKBonusDto);
        }
        catch (Exception e)
        {
            logger.LogError("Error in adding item bonus");
            logger.LogError(e.Message);
            return StatusCode(500);
        }
    }


    private void SetBonusImageUrls(Bonus bonus)
    {
        bonus.ImageForDepositView =
            $"https://{Environment.GetEnvironmentVariable("ENV")}.24cases.ru/v1/bonus/image/{bonus.Id}";
        bonus.BonusImage = $"https://{Environment.GetEnvironmentVariable("ENV")}.24cases.ru/v1/bonus/image/{bonus.Id}";
    }
}