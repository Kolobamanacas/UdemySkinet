using Core.Entities;

namespace Core.Specifications;

public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
{
    public ProductsWithFiltersForCountSpecification(ProductQueryParametersSpecification parameters)
        : base(product =>
            (string.IsNullOrEmpty(parameters.Search) || product.Name.ToLower().Contains(parameters.Search))
            && (!parameters.BrandId.HasValue || product.ProductBrandId == parameters.BrandId)
            && (!parameters.TypeId.HasValue || product.ProductTypeId == parameters.TypeId)
        )
    {
    }
}
