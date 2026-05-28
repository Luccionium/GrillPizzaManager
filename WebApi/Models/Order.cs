using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal TotalPrice { get; set; }

    public int UserId { get; set; }

    public string? Note { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
