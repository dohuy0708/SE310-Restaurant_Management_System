namespace SE310_Restaurant_Management_System.Models.ItemModels
{
    public class Item
    {
        public int MenuItemId { get; set; }

        public string Name { get; set; } = null!;

        public string? Image { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
    }
}