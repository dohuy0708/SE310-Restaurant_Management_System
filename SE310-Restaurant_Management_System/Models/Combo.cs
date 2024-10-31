using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class Combo
{
    public int ComboId { get; set; }

    public string? Image { get; set; }

    public string ComboName { get; set; } = null!;

    public decimal ComboPrice { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
}
