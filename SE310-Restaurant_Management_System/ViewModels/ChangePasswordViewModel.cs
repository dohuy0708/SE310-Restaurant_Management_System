using System.ComponentModel.DataAnnotations;

namespace SE310_Restaurant_Management_System.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "*Vui lòng nhập mật khẩu hiện tại.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "*Vui lòng nhập mật khẩu mới.")]
        [MinLength(8, ErrorMessage = "*Mật khẩu mới phải có ít nhất 8 ký tự.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "*Vui lòng xác nhận mật khẩu mới.")]
        [Compare("NewPassword", ErrorMessage = "*Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
