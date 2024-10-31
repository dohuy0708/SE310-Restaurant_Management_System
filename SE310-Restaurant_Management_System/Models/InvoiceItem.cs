using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class InvoiceItem
{
    public int InvoiceItemId { get; set; }

    public int? InvoiceId { get; set; }

    public int? MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual MenuItem? MenuItem { get; set; }
}
