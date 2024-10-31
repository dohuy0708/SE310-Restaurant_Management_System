using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class RestaurantTable
{
    public int TableId { get; set; }

    public int TableNumber { get; set; }

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<BookingOrder> BookingOrders { get; set; } = new List<BookingOrder>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual TableStatus Status { get; set; } = null!;
}
