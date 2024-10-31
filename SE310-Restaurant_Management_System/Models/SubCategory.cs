using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class SubCategory
{
    public int SubCategoryId { get; set; }

    public int? CategoryId { get; set; }

    public string SubCategoryName { get; set; } = null!;

    public virtual FoodCategory? Category { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
