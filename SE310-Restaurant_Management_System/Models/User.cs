using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string? Email { get; set; }

    public virtual Role Role { get; set; } = null!;
}
