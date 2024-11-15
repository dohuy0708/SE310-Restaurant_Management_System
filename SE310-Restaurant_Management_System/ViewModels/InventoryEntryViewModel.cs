using System.Web.Mvc.Routing.Constraints;

namespace SE310_Restaurant_Management_System.ViewModels
{
    public class InventoryEntryViewModel
    {
        public int EntryId { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalPrice { get; set; }
        public List<EntryDetailViewModel> EntryDetails { get; set; } = new List<EntryDetailViewModel>();
    }

    public class EntryDetailViewModel
    {
      /*  public int DetailId { get; set; }*/
        public int IngredientId { get; set; }
        public int DetailId { get; set; }   
        public string IngredientName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }
}
