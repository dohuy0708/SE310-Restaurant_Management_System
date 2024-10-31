using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class EntryDetail
{
    public int DetailId { get; set; }

    public int EntryId { get; set; }

    public int IngredientId { get; set; }

    public string Unit { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal ImportPrice { get; set; }

    public virtual InventoryEntry Entry { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
