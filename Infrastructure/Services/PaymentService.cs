using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Order = Core.Entities.OrderAggregate.Order;
using Product = Core.Entities.Product;

namespace Infrastructure.Services;
public class PaymentService : IPaymentService
{
    private readonly IBasketRepository basketRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IConfiguration configuration;

    public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        this.basketRepository = basketRepository;
        this.unitOfWork = unitOfWork;
        this.configuration = configuration;
    }

    public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string? basketId)
    {
        StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
        CustomerBasket? basket = await basketRepository.GetBasketAsync(basketId);

        if (basket == null || !basket.DeliveryMethodId.HasValue || basket.DeliveryMethodId == Guid.Empty)
        {
            return null;
        }

        decimal shippingPrice = 0;
        DeliveryMethod? deliveryMethod = await unitOfWork.Repository<DeliveryMethod>()
            .GetByIdAsync(basket.DeliveryMethodId);
        shippingPrice = deliveryMethod?.Price ?? 0m;

        foreach (BasketItem item in basket.Items)
        {
            Product? productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

        PaymentIntentService service = new PaymentIntentService();
        PaymentIntent paymentIntent;

        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            PaymentIntentCreateOptions options = new PaymentIntentCreateOptions
            {
                Amount = (long)(basket.Items.Sum(item => item.Quantity * item.Price * 100) + (shippingPrice * 100)),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            paymentIntent = await service.CreateAsync(options);
            basket.PaymentIntentId = paymentIntent.Id;
            basket.ClientSecret = paymentIntent.ClientSecret;
        }
        else
        {
            PaymentIntentUpdateOptions options = new PaymentIntentUpdateOptions
            {
                Amount = (long)(basket.Items.Sum(item => item.Quantity * item.Price * 100) + (shippingPrice * 100))
            };

            await service.UpdateAsync(basket.PaymentIntentId, options);
        }

        await basketRepository.UpdateBasketAsync(basket);
        return basket;
    }

    public async Task<Order?> UpdateOrderPaymentSucceeded(string paymentIntentId)
    {
        OrderByPaymentIntentIdSpecification specification = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        Order order = await unitOfWork.Repository<Order>().GetOneAsync(specification);

        if (order == null)
        {
            return null;
        }

        order.Status = OrderStatus.PaymentRecevied;
        unitOfWork.Repository<Order>().Update(order);
        await unitOfWork.Complete();
        return order;
    }

    public async Task<Order?> UpdateOrderPaymentFailed(string paymentIntentId)
    {
        OrderByPaymentIntentIdSpecification specification = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        Order order = await unitOfWork.Repository<Order>().GetOneAsync(specification);

        if (order == null)
        {
            return null;
        }

        order.Status = OrderStatus.PaymentFailed;
        await unitOfWork.Complete();
        return order;
    }
}
