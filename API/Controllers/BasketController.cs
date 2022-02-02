using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class BasketController : BaseApiController
{
    private readonly IBasketRepository basketRepository;
    private readonly IMapper mapper;

    public BasketController(IBasketRepository basketRepository, IMapper mapper)
    {
        this.basketRepository = basketRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
    {
        CustomerBasket basket = await basketRepository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
    {
        CustomerBasket updatedBasket = await basketRepository.UpdateBasketAsync(mapper.Map<CustomerBasketDto, CustomerBasket>(basket));
        return Ok(updatedBasket);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await basketRepository.DeleteBasketAsync(id);
    }
}
