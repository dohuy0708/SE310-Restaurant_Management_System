using System.ComponentModel.DataAnnotations;

namespace SE310_Restaurant_Management_System.Models
{
    public class MenuItemModel
    {
        public string Name { get; set; }

        public IFormFile? ImagePath { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? SubCategoryId { get; set; }
    }
}