namespace SE310_Restaurant_Management_System.Models
{
    public class InvoiceItemViewModel
    {
        public string MenuItemName { get; set; } // Tên món ăn
        public decimal Price { get; set; } // Giá món
        public int Quantity { get; set; } // Tổng số lượng
        public decimal TotalPrice { get; set; } // Tổng giá tiền (Số lượng * Giá)
    }

}
