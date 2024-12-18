namespace SE310_Restaurant_Management_System.ViewModels
{
    public class ComboViewModel
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; }
        public decimal ComboPrice { get; set; }
        public IFormFile? ImagePath { get; set; }
        public List<int>? SelectedMenuItems { get; set; }
    }
}