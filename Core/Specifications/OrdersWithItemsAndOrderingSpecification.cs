using Core.Entities.OrderAggregate;
using System;

namespace Core.Specifications;
public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
{
    public OrdersWithItemsAndOrderingSpecification(string email) : base(order => order.BuyerEmail == email)
    {
        AddInclude(order => order.OrderItems);
        AddInclude(order => order.DeliveryMethod);
        AddOrderByDescending(order => order.OrderDate);
    }

    public OrdersWithItemsAndOrderingSpecification(Guid orderId, string email)
        : base(order => order.Id == orderId && order.BuyerEmail == email)
    {
        AddInclude(order => order.OrderItems);
        AddInclude(order => order.DeliveryMethod);
    }
}
