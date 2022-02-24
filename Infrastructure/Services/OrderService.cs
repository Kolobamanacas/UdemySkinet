using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;
public class OrderService : IOrderService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IBasketRepository basketRepository;

    public OrderService(
        IUnitOfWork unitOfWork,
        IBasketRepository basketRepository)
    {
        this.unitOfWork = unitOfWork;
        this.basketRepository = basketRepository;
    }

    public async Task<Order?> CreateOrderAsync(string buyerEmail, Guid deliveryMethodId, string basketId, Address shippingAddress)
    {
        // Get basket from the repo.
        CustomerBasket basket = await basketRepository.GetBasketAsync(basketId);
        // Get items from the Product repo.
        List<OrderItem> items = new List<OrderItem>();

        foreach (BasketItem item in basket.Items)
        {
            Product productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
            ProductItemOrdered itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
            OrderItem orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
            items.Add(orderItem);
        }

        // Get delivery method from the repo.
        DeliveryMethod deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
        // Calculate subtotal.
        decimal subtotal = items.Sum(item => item.Price * item.Quantity);
        // Create order.
        Order order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
        // Save order to DB.
        unitOfWork.Repository<Order>().Add(order);
        int resultCode = await unitOfWork.Complete();

        if (resultCode <= 0)
        {
            return null;
        }

        // Delete basket.
        await basketRepository.DeleteBasketAsync(basketId);
        // Return order.
        return order;
    }

    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
        return await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
    }

    public async Task<Order> GetOrderByIdAsync(Guid orderId, string buyerEmail)
    {
        OrdersWithItemsAndOrderingSpecification specification = new OrdersWithItemsAndOrderingSpecification(orderId, buyerEmail);
        return await unitOfWork.Repository<Order>().GetOneAsync(specification);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    {
        OrdersWithItemsAndOrderingSpecification specification = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
        return await unitOfWork.Repository<Order>().GetManyAsync(specification);
    }
}
