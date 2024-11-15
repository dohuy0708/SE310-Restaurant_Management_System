using System.ComponentModel.DataAnnotations;

namespace SE310_Restaurant_Management_System.ViewModels
{
    public class AccountViewModel
    {
        [Required (ErrorMessage ="*Vui lòng nhập  Email.")]
         public string Email { set; get; }
        [Required (ErrorMessage ="*Vui lòng nhập mật khẩu.")]
        public string Password { set; get; }
    }
}
