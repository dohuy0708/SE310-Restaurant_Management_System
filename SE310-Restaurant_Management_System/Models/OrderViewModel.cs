namespace SE310_Restaurant_Management_System.Models
{
    // Thêm các ViewModel để mapping dữ liệu
    public class OrderViewModel
    {
        public int TableId { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
