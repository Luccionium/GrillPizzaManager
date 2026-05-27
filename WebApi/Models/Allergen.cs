using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Allergen
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<FoodAllergen> FoodAllergens { get; set; } = new List<FoodAllergen>();
}
