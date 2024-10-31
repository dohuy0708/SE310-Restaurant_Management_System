using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class IngredientType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
