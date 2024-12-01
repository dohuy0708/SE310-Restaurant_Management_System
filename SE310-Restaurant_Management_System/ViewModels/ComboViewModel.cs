namespace SE310_Restaurant_Management_System.ViewModels
{
    public class ComboViewModel
    {
        public string TenCombo { get; set; }
        public decimal GiaCombo { get; set; }
        public List<MenuItemViewModel> SelectedMenuItems { get; set; }
        public List<MonAnViewModel> MonAns { get; set; } = new List<MonAnViewModel>();
    }

    public class MonAnViewModel
    {
        public int Id { get; set; }
        public string TenMonAn { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
    }
}