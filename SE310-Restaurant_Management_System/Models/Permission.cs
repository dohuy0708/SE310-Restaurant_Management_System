using System;
using System.Collections.Generic;

namespace SE310_Restaurant_Management_System.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? PermissionGroup { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
