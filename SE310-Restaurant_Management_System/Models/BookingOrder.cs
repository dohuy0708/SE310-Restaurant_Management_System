using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class BookingOrder
{
    public int BookingOrderId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public DateTime BookingTime { get; set; }

    public int NumberOfPeople { get; set; }

    public decimal? DepositAmount { get; set; }

    public int? TableId { get; set; }

    public virtual RestaurantTable? Table { get; set; }
}
