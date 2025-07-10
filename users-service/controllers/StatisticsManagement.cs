using DataTransferLib.CommunicationsServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersService.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UsersService.controllers
{
    [Route("stats")]
    [ApiController]
    public class StatisticsManagement : ControllerBase
    {
        private readonly ApplicationDbContext _dbcontext;

        public StatisticsManagement(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }








        // GET stats/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserStat(string id)
        {
            var userStat = await _dbcontext.UserStatistics.Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (userStat == null)
                return NotFound();

            return Ok(userStat);
        }

        // PUT stats/placeorder
        [HttpPut("placeorder")]
        public async Task<IActionResult> AddPlaceorder(string id)
        {
            var userPlaceorder = await _dbcontext.UserStatistics.Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (userPlaceorder == null)
                return NotFound();

            userPlaceorder.OrdersPlaced += 1;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return Ok("Количеству размещенных заказов +1");
        }

        // PUT stats/crushrocketplayed
        [HttpPut("crushrocketplayed")]
        public async Task<IActionResult> AddCrushRocketplayed(string id)
        {
            var userCrushRocketplayed =
                await _dbcontext.UserStatistics.Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (userCrushRocketplayed == null)
                return NotFound();

            userCrushRocketplayed.CrashRocketsPlayed += 1;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return Ok("Количеству сыгранных краш рокет +1");
        }

        // PUT stats/promocodeused
        [HttpPut("promocodeused")]
        public async Task<IActionResult> AddPromocodeused(string id)
        {
            var userPromocodeused = await _dbcontext.UserStatistics.Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (userPromocodeused == null)
                return NotFound();

            userPromocodeused.PromocodesUsed += 1;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new RequestService().HandleError(e);
            }

            return Ok("Количеству использованных промокодов +1");
        }
    }
}