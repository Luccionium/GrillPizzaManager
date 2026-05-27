using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class FoodAllergen
{
    public int Id { get; set; }

    public int FoodId { get; set; }

    public int AllergenId { get; set; }

    public virtual Allergen Allergen { get; set; } = null!;

    public virtual Food Food { get; set; } = null!;
}
