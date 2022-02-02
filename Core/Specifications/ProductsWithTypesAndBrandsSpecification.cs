using Core.Entities;
using System;

namespace Core.Specifications;

public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
{
    public ProductsWithTypesAndBrandsSpecification(ProductQueryParametersSpecification parameters)
        : base(product =>
            (string.IsNullOrEmpty(parameters.Search) || product.Name.ToLower().Contains(parameters.Search))
            && (!parameters.BrandId.HasValue || product.ProductBrandId == parameters.BrandId)
            && (!parameters.TypeId.HasValue || product.ProductTypeId == parameters.TypeId)
        )
    {
        AddInclude(p => p.ProductType);
        AddInclude(p => p.ProductBrand);
        AddOrderByAscending(p => p.Name);
        ApplyPaging(parameters.PageSize * (parameters.PageIndex - 1), parameters.PageSize);

        if (!string.IsNullOrEmpty(parameters.Sort))
        {
            switch (parameters.Sort)
            {
                case "priceAsc":
                    AddOrderByAscending(p => p.Price);
                    break;
                case "priceDesc":
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderByAscending(p => p.Name);
                    break;
            }
        }
    }

    public ProductsWithTypesAndBrandsSpecification(Guid id) : base(p => p.Id == id)
    {
        AddInclude(p => p.ProductType);
        AddInclude(p => p.ProductBrand);
    }
}
