namespace DotNg.Application.Models;

public class ListResponse<T> where T : class
{
    public int TotalCount { get; set; }
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool IsFirst { get; set; }
    public bool IsLast { get; set; }

    public ListResponse()
    {
        TotalCount = PageNumber = PageSize = 0;
        IsFirst = IsLast = true;
        Data = [];
    }

    public ListResponse(List<T> data)
    {
        IsFirst = IsLast = true;
        Data = data;
        TotalCount = data.Count;
        PageNumber = 1;
        PageSize = data.Count;
    }

    public ListResponse(List<T> data, int totalCount, int? pageNumber, int? pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber ?? 1;
        PageSize = pageSize ?? totalCount;

        IsFirst = PageNumber == 1;
        IsLast = pageNumber * pageSize >= TotalCount;
    }

    public ListResponse(List<T> data, int totalCount, int pageNumber, int pageSize)
    {
        IsFirst = pageNumber == 1;
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        IsLast = pageNumber * pageSize >= totalCount;
    }

    public ListResponse(List<T> data, int totalCount, int pageNumber, int pageSize, bool isFirst, bool isLast)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        IsFirst = isFirst;
        IsLast = isLast;
        Data = data;
    }
}