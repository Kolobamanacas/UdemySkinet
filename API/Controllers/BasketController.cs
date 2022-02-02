using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class BasketController : BaseApiController
{
    private readonly IBasketRepository basketRepository;

    public BasketController(IBasketRepository basketRepository)
    {
        this.basketRepository = basketRepository;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
    {
        CustomerBasket basket = await basketRepository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
    {
        CustomerBasket updatedBasket = await basketRepository.UpdateBasketAsync(basket);
        return Ok(updatedBasket);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await basketRepository.DeleteBasketAsync(id);
    }
}
