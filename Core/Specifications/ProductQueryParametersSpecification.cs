using System;

namespace Core.Specifications;

public class ProductQueryParametersSpecification
{
    public int PageIndex { get; set; } = 1;
    public int PageSize
    {
        get => pageSize;
        set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public Guid? BrandId { get; set; }
    public Guid? TypeId { get; set; }
    public string? Sort { get; set; }
    public string? Search
    {
        get => search;
        set => search = value.ToLower();
    }

    private const int MaxPageSize = 50;
    private int pageSize = 6;
    private string? search = "";
}
