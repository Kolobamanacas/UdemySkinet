using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IOrderService
{
    Task<Order?> CreateOrderAsync(string buyerEmail, Guid deliveryMethod, string basketId, Address shippingAddress);
    Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
    Task<Order> GetOrderByIdAsync(Guid orderId, string buyerEmail);
    Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
}
