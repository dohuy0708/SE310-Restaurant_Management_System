using System.Drawing;

namespace SE310_Restaurant_Management_System.ViewModels
{
    public class MenuItemViewModel
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public bool IsSelected { get; set; } // Để tick chọn
    }
}