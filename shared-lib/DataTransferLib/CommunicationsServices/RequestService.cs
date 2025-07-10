using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

namespace DataTransferLib.CommunicationsServices;

/// <summary>Сервис для обслуживания работы с запросами</summary>
public class RequestService : ControllerBase
{
    public ActionResult GetResponse<T>(string message, T result, int? page = null, int? count = null)
    {
        if (page == null)
            return Ok(
                new DefaultResponse<T>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = message,
                    Result = result,
                });

        return Ok(
            new ExtendedResponse<T>
            {
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Result = result,
                Page = page,
                Count = count
            });
    }


    public ActionResult HandleError<T>(Exception e, string message = "Ошибка обработки запроса")
    {
        return new ContentResult()
        {
            StatusCode = 500,
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(new ErrorResponse<T>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message,
                ErrorDetails = e.Message,
            })
        };
    }

    public ActionResult HandleError<T>(T errorObject, string message = "Ошибка обработки запроса")
    {
        return new ContentResult()
        {
            StatusCode = 500,
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(new ErrorResponse<T>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message,
                Result = errorObject
            })
        };
    }

    public static ActionResult InternalServerError(string message = "Ошибка обработки запроса")
    {
        return new ContentResult()
        {
            StatusCode = 500,
            ContentType = "application/json",
            Content = message
        };
    }

    public static void CheckPaginationParams(ref DefaultRequest defaultRequest)
    {
        switch (defaultRequest.Count)
        {
            case -1:
                defaultRequest.Count = DataTransferObjects.Common.DefaultRequest.MaximumCount;
                return;
            case <= 0:
                defaultRequest.Count = DataTransferObjects.Common.DefaultRequest.DEFAULT_COUNT;
                return;
        }

        if (defaultRequest.Page <= 0)
            defaultRequest.Page = DataTransferObjects.Common.DefaultRequest.DEFAULT_PAGE;
    }

    /// <summary>Для получения корректных границ из Request.FilterValue</summary>
    public static T[]? GetValuesFromFilter<T>(string values)
    {
        T[]? result = JsonSerializer.Deserialize<T[]>(values);
        if (result != null)
        {
            if (Comparer<T>.Default.Compare(result[0], result[1]) > 0)
            {
                (result[0], result[1]) = (result[1], result[0]);
            }
        }

        return result;
    }
}