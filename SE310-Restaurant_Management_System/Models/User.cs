using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SE310_Restaurant_Management_System.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "*Vui lòng nhập mật khẩu.")]
    [MinLength(8, ErrorMessage = "*Mật khẩu phải có ít nhất 8 ký tự.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "*Vui lòng chọn vai trò.")]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "*Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "*Email không đúng định dạng.")]
     
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [Phone(ErrorMessage = "*Số điện thoại không hợp lệ.")]
    public string? PhoneNumber { get; set; }  

    public virtual Role Role { get; set; } = null!;
}
