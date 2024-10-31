using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class InventoryEntry
{
    public int EntryId { get; set; }

    public DateOnly EntryDate { get; set; }

    public string? Description { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<EntryDetail> EntryDetails { get; set; } = new List<EntryDetail>();
}
