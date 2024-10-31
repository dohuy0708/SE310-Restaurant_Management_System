using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? SubCategoryId { get; set; }

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual SubCategory? SubCategory { get; set; }
}
