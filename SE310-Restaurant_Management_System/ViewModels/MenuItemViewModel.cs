using System.Drawing;

namespace SE310_Restaurant_Management_System.ViewModels
{
    public class MenuItemViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int SubCategory { get; set; }
        public string ImagePath { get; set; }
        public IFormFile Image { get; set; }
    }
}