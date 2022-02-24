using System;

namespace API.Dtos;

public class OrderItemDto
{
    public Guid ProudctId { get; set; }
    public string ProductName { get; set; }
    public string PictureUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
