namespace DataTransferLib.DataTransferObjects.Common;

/// <summary>Класс для получения корректных параметров запроса</summary>
public class DefaultRequest
{
    public const int DEFAULT_COUNT = 30;
    public const int DEFAULT_PAGE = 1;
    public const int MaximumCount = 9999;

    public int Page { get; set; } = DEFAULT_PAGE;
    public int Count { get; set; } = DEFAULT_COUNT;
    public string OrderBy { get; set; } = "name";
    public string OrderType { get; set; } = "asc";
    public string? FilterBy { get; set; }
    public string? FilterValue { get; set; }

    public string GetCacheKey(RequestType type)
    {
        return $"page_{Page}_count_{Count}_orderby{OrderBy}_ordertype{OrderType}_fliterby{FilterBy}_filterValue{FilterValue}";
    }

    public enum RequestType
    {
        User,
        Cases,
        Items,
        Financials,
    }

    public virtual string GetQueryParams()
    {
        var parameters = new List<string>();

        if (Page != DEFAULT_PAGE)
            parameters.Add($"page={Page}");

        if (Count != DEFAULT_COUNT)
            parameters.Add($"count={Count}");

        if (!string.IsNullOrWhiteSpace(OrderBy))
            parameters.Add($"orderby={OrderBy}");

        if (!string.IsNullOrWhiteSpace(OrderType))
            parameters.Add($"ordertype={OrderType}");

        if (!string.IsNullOrWhiteSpace(FilterBy))
            parameters.Add($"filterby={FilterBy}");

        if (!string.IsNullOrWhiteSpace(FilterValue))
            parameters.Add($"filtervalue={FilterValue}");

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : "";
    }

}

public class DateFilterRequest : DefaultRequest
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public override string GetQueryParams()
    {
        var parameters = new List<string>();

        var baseParams = base.GetQueryParams();
        if (!string.IsNullOrWhiteSpace(baseParams))
            parameters.Add(baseParams.TrimStart('?'));

        if (From != default)
            parameters.Add($"from={From:O}");

        if (To != default)
            parameters.Add($"to={To:O}");

        return parameters.Count > 0 ? "?" + string.Join("&", parameters) : "";
    }

}