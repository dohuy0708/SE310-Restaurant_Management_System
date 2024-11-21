namespace SE310_Restaurant_Management_System.Models
{
    public class MenuItemModel
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; } = null!;

        public IFormFile ImagePath { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? SubCategoryId { get; set; }
    }
}