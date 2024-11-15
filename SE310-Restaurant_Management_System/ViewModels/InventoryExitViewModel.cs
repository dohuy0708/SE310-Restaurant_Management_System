namespace SE310_Restaurant_Management_System.ViewModels
{
    public class InventoryExitViewModel
    {
        public int ExitId { get; set; }
        public DateTime ExitDate { get; set; }
        public string Description { get; set; }
        public string RecipientName { get; set; }
        public List<ExitDetailViewModel> ExitDetails { get; set; } = new List<ExitDetailViewModel>();
    }

    public class ExitDetailViewModel
    {
        public int DetailId { get; set; }
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
    }
}
