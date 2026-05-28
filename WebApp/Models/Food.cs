using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Food
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }

    public int FoodCategoryId { get; set; }

    public virtual ICollection<FoodAllergen> FoodAllergens { get; set; } = new List<FoodAllergen>();

    public virtual FoodCategory FoodCategory { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
