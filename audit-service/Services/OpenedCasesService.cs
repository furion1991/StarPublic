using System.Net;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Users.Admin;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Services
{
    public class OpenedCasesService(ApplicationDbContext dbContext, CasesCommService casesCommService)
    {
        public async Task<ItemDto> GetMaxCostItem(string userId)
        {
            var openedCase = dbContext.OpenedCases
                .Where(u => u.UserId == userId)
                .OrderByDescending(c => c.Cost)
                .FirstOrDefault();


            var item = await casesCommService.GetItem(openedCase.ItemDroppedId);

            return item?.Result ?? new ItemDto() { Id = string.Empty };
        }


        public async Task<CaseDto?> GetFavouriteCaseForUser(string userId)
        {
            var favCaseId = dbContext.OpenedCases
                .GroupBy(c => c.CaseId)
                .Select(group => new
                {
                    CaseId = group.Key,
                    OpenCount = group.Count(),
                })
                .OrderByDescending(x => x.OpenCount)
                .FirstOrDefault();

            var caseDb = await casesCommService.GetCase(favCaseId.CaseId);

            if (caseDb != null && caseDb.StatusCode == HttpStatusCode.OK)
            {
                return caseDb.Result;
            }

            return null;
        }

        public async Task<List<DroppedItemDto>> GetLastTenDroppedItems()
        {
            var lastItemsDroppedQuery = await dbContext.OpenedCases
                .OrderByDescending(e => e.OpenedTimeStamp)
                .Take(10)
                .ToListAsync();

            List<DroppedItemDto> lastItemDroppedList = await CreateDroppedItemDtoList(lastItemsDroppedQuery);

            return lastItemDroppedList;
        }

        public async Task<List<DroppedItemDto>> GetLastTenItemsDroppedByCost()
        {
            var droppedItemsByCostQuery = await dbContext.OpenedCases
                .OrderByDescending(e => e.Cost)
                .Take(10)
                .ToListAsync();

            var lastItemDroppedListByCost = await CreateDroppedItemDtoList(droppedItemsByCostQuery);

            return lastItemDroppedListByCost;
        }

        private async Task<List<DroppedItemDto>> CreateDroppedItemDtoList(List<OpenedCase> openedCases)
        {
            var list = new List<DroppedItemDto>();

            var itemsList = await GetItemList(openedCases);
            foreach (var item in openedCases)
            {
                list.Add(new DroppedItemDto()
                {
                    Item = itemsList.FirstOrDefault(e => e.Id == item.ItemDroppedId),
                    CaseId = item.CaseId,
                    OpenedTimeStamp = item.OpenedTimeStamp,
                    UserId = item.UserId,
                });
            }

            return list;
        }

        public async Task<CasesOpenDto> GetOpenedCasesStatsForAdmin()
        {
            var openedCasesCount = await dbContext.OpenedCases.CountAsync();
            var openedCasesAmount = dbContext.OpenedCases.Sum(e => e.Cost);
            var data = new CasesOpenDto()
            {
                Amount = openedCasesAmount.Value,
                Count = openedCasesCount,
                DateTime = DateTime.UtcNow
            };
            return data;
        }


        private async Task<List<ItemDto>> GetItemList(List<OpenedCase> openedCases)
        {
            var result = await casesCommService.GetItemsList(openedCases.Select(e => e.ItemDroppedId).ToList());

            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return result.Result ?? new List<ItemDto>();
            }

            return new List<ItemDto>();
        }
    }
}