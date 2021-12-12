using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericRepository<Product> productsRepository;
    private readonly IGenericRepository<ProductBrand> brandsRepository;
    private readonly IGenericRepository<ProductType> typesRepository;
    private readonly IMapper mapper;

    public ProductsController(
        IGenericRepository<Product> productsRepository,
        IGenericRepository<ProductBrand> brandsRepository,
        IGenericRepository<ProductType> typesRepository,
        IMapper mapper)
    {
        this.productsRepository = productsRepository;
        this.brandsRepository = brandsRepository;
        this.typesRepository = typesRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
    {
        ProductsWithTypesAndBrandsSpecification specification = new ProductsWithTypesAndBrandsSpecification();
        IReadOnlyList<Product> products = await productsRepository.GetManyAsync(specification);
        return Ok(mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(Guid id)
    {
        ProductsWithTypesAndBrandsSpecification specification = new ProductsWithTypesAndBrandsSpecification(id);
        Product product = await productsRepository.GetOneAsync(specification);
        return mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
        return Ok(await brandsRepository.GetAllAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
        return Ok(await typesRepository.GetAllAsync());
    }
}
