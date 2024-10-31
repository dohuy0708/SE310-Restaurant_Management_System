using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public int Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public DateOnly ImportDate { get; set; }

    public int TypeId { get; set; }

    public virtual ICollection<EntryDetail> EntryDetails { get; set; } = new List<EntryDetail>();

    public virtual ICollection<ExitDetail> ExitDetails { get; set; } = new List<ExitDetail>();

    public virtual IngredientType Type { get; set; } = null!;
}
