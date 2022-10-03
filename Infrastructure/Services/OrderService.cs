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
    private readonly IPaymentService paymentService;

    public OrderService(
        IUnitOfWork unitOfWork,
        IBasketRepository basketRepository,
        IPaymentService paymentService)
    {
        this.unitOfWork = unitOfWork;
        this.basketRepository = basketRepository;
        this.paymentService = paymentService;
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

        // Check to see if order exists.
        OrderByPaymentIntentIdSpecification specification =
            new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
        Order? existingOrder = await unitOfWork.Repository<Order>().GetOneAsync(specification);

        if (existingOrder != null)
        {
            unitOfWork.Repository<Order>()?.Delete(existingOrder);
            await paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
        }

        // Create order.
        Order order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, basket.PaymentIntentId);

        // Save order to DB.
        unitOfWork.Repository<Order>().Add(order);
        int resultCode = await unitOfWork.Complete();

        if (resultCode <= 0)
        {
            return null;
        }

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
