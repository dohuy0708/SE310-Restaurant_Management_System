using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class ExitDetail
{
    public int DetailId { get; set; }

    public int ExitId { get; set; }

    public int IngredientId { get; set; }

    public string Unit { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual InventoryExit Exit { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
