using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

public class ProductsController : BaseApiController
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
    [Cached(600)]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
        [FromQuery] ProductQueryParametersSpecification parameters)
    {
        ProductsWithTypesAndBrandsSpecification specification = new ProductsWithTypesAndBrandsSpecification(parameters);
        ProductsWithFiltersForCountSpecification countSpecification = new ProductsWithFiltersForCountSpecification(parameters);
        int totalItems = await productsRepository.CountAsync(countSpecification);
        IReadOnlyList<Product> products = await productsRepository.GetManyAsync(specification);
        IReadOnlyList<ProductToReturnDto> productsToReturn =
            mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
        return Ok(new Pagination<ProductToReturnDto>(parameters.PageIndex, parameters.PageSize, totalItems, productsToReturn));
    }

    [HttpGet("{id}")]
    [Cached(600)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(Guid id)
    {
        ProductsWithTypesAndBrandsSpecification specification = new ProductsWithTypesAndBrandsSpecification(id);
        Product product = await productsRepository.GetOneAsync(specification);

        if (product == null)
        {
            return NotFound(new ApiResponse(404));
        }

        return mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("brands")]
    [Cached(600)]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
        return Ok(await brandsRepository.GetAllAsync());
    }

    [HttpGet("types")]
    [Cached(600)]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
        return Ok(await typesRepository.GetAllAsync());
    }
}
