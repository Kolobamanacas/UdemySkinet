using API.Dtos;
using API.Errors;
using API.Extensionos;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService orderService;
    private readonly IMapper mapper;

    public OrdersController(IOrderService orderService, IMapper mapper)
    {
        this.orderService = orderService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        string email = HttpContext.User.RetrieveEmailFromPrincipal();
        IReadOnlyList<Order> orders = await orderService.GetOrdersForUserAsync(email);
        return Ok(mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(Guid id)
    {
        string email = HttpContext.User.RetrieveEmailFromPrincipal();
        Order order = await orderService.GetOrderByIdAsync(id, email);

        if (order == null)
        {
            return NotFound(new ApiResponse(404));
        }

        return Ok(mapper.Map<Order, OrderToReturnDto>(order));
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await orderService.GetDeliveryMethodsAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
    {
        string email = HttpContext.User.RetrieveEmailFromPrincipal();
        Address address = mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);
        Order order = await orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

        if (order == null)
        {
            return BadRequest(new ApiResponse(400, "Problem creating order."));
        }

        return Ok(order);
    }
}
