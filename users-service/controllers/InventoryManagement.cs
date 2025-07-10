using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users;
using UsersService.HttpClientContext;
using UsersService.Models;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;


namespace UsersService.controllers
{
    [Route("inventory")]
    [ApiController]
    public class InventoryManagement(ApplicationDbContext dbContext) : ControllerBase
    {


        [HttpPut("set_item_state")]
        public async Task<IActionResult> SetItemStateAfterAction(ChangeStateRequest request)
        {
            if (request.InventoryRecordsIds.Count == 0)
            {
                return BadRequest();
            }

            foreach (var id in request.InventoryRecordsIds)
            {
                var inventoryRecord = await dbContext.ItemsUser
                    .FirstOrDefaultAsync(ir => ir.Id == id);

                if (inventoryRecord == null)
                {
                    return NotFound();
                }
                inventoryRecord.IsItemActive = false;
                inventoryRecord.ItemRecordState = request.ItemRecordState;
            }

            try
            {
                await dbContext.SaveChangesAsync();
                return Ok("State changed");
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);
            }
        }





    }
}