using Core.Entities.OrderAggregate;

namespace Core.Specifications;
public class OrderByPaymentIntentIdSpecification : BaseSpecification<Order>
{
    public OrderByPaymentIntentIdSpecification(string? paymentIntentId)
        : base((Order order) => order.PaymentIntentId == paymentIntentId)
    {
    }
}
