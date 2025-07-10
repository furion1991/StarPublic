using System.Net;
using CasesService.Database;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common;
using Microsoft.EntityFrameworkCore;

namespace CasesService.Services;

public class CategoryService(ApplicationDbContext dbContext)
{

    public async Task<CaseCategory?> GetCategoryById(string id)
    {
        return await dbContext.CaseCategories.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<CaseCategory?> GetCategoryByName(string name)
    {
        return await dbContext.CaseCategories.FirstOrDefaultAsync(c => c.Name == name.ToLower());
    }

    public async Task<IResponse<bool>> UpdateCategory(string id, string name)
    {
        var existingCategory = await dbContext.CaseCategories.FirstOrDefaultAsync(c => c.Id == id);

        if (existingCategory == null)
        {
            return new ErrorResponse<bool>()
            {
                Message = "Category with this ID not found",
                Result = false,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        existingCategory.Name = name.ToLower();
        existingCategory.NormilizedName = name.ToUpper();
        await dbContext.SaveChangesAsync();
        return new DefaultResponse<bool>()
        {
            Message = "Category updated successfully",
            Result = true,
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task<IResponse<bool>> DeleteCategory(string id)
    {
        var existingCategory = await dbContext.CaseCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (existingCategory == null)
        {
            return new ErrorResponse<bool>()
            {
                Message = "Category with this ID not found",
                Result = false,
                StatusCode = HttpStatusCode.NotFound
            };
        }
        dbContext.CaseCategories.Remove(existingCategory);
        await dbContext.SaveChangesAsync();
        return new DefaultResponse<bool>()
        {
            Message = "Category deleted successfully",
            Result = true,
            StatusCode = HttpStatusCode.OK
        };
    }

    public async Task<IResponse<List<CaseCategory>>> GetAllCategories()
    {
        try
        {
            var categories = await dbContext.CaseCategories.ToListAsync();
            return new DefaultResponse<List<CaseCategory>>()
            {
                Message = "Categories retrieved successfully",
                Result = categories,
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception e)
        {
            return new ErrorResponse<List<CaseCategory>>()
            {
                Message = e.Message,
                Result = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<IResponse<string>> CreateCategory(string name)
    {
        var existingCategory = await dbContext.CaseCategories.FirstOrDefaultAsync(c => c.Name == name.ToLower());

        if (existingCategory != null)
        {
            return new ErrorResponse<string>()
            {
                Message = "User with this name already exists.",
                Result = "",
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var newCategory = new CaseCategory
        {
            Name = name.ToLower(),
            ImageUrl = "",
            NormilizedName = name.ToUpper(),
            Id = Guid.NewGuid().ToString()
        };
        try
        {

            await dbContext.CaseCategories.AddAsync(newCategory);

            await dbContext.SaveChangesAsync();

            return new DefaultResponse<string>()
            {
                Message = "Category created successfully",
                Result = newCategory.Id,
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception e)
        {
            return new ErrorResponse<string>()
            {
                Message = e.Message,
                Result = "",
                StatusCode = HttpStatusCode.InternalServerError
            };

        }
    }
}