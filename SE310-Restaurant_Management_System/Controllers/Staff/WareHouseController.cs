using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
using SE310_Restaurant_Management_System.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using X.PagedList;

namespace SE310_Restaurant_Management_System.Controllers.Staff
{
    [Route("WareHouse")]
    public class WareHouseController : Controller
    {
        QlnhaHangContext db = new QlnhaHangContext();
        private readonly ILogger<WareHouseController> _logger;

        public WareHouseController(ILogger<WareHouseController> logger)
        {
            _logger = logger;
        }

        [Route("Home")]
        [HttpGet]
        public IActionResult Home(int? page, int? typeId, string searchTerm)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Start with all ingredients
            var ingredientsQuery = db.Ingredients.Include(i => i.Type).AsQueryable();

            // Default ViewBag name for "All"
            ViewBag.Name = "Tất cả";

            // Filter by type if specified
            if (typeId.HasValue)
            {
                ingredientsQuery = ingredientsQuery.Where(i => i.TypeId == typeId.Value);

                var selectedType = ingredientsQuery.FirstOrDefault()?.Type?.TypeName;
                if (selectedType != null)
                {
                    ViewBag.Name = selectedType;
                }
            }

            // Filter by search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ingredientsQuery = ingredientsQuery.Where(i => i.IngredientName.Contains(searchTerm));
            }

            var ingredients = ingredientsQuery.ToList();
            PagedList<Ingredient> list = new PagedList<Ingredient>(ingredients, pageNumber, pageSize);

            // Pass the search term back to the view to display it in the input field
            ViewBag.SearchTerm = searchTerm;

            return View(list);
        }



        [Route("Import")]
        [HttpGet]
        public IActionResult Import(int? page, DateTime? selectedDate)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Retrieve all entries and convert DateOnly to DateTime for comparison
            var imports = db.InventoryEntries
                .Select(entry => new InventoryEntryViewModel
                {
                    EntryId = entry.EntryId,
                    // Convert DateOnly to DateTime explicitly here
                    EntryDate = entry.EntryDate.ToDateTime(TimeOnly.MinValue),  // Convert DateOnly to DateTime
                    Description = entry.Description,
                    TotalPrice = entry.TotalPrice,
                    EntryDetails = entry.EntryDetails.Select(detail => new EntryDetailViewModel
                    {
                        DetailId = detail.DetailId,
                        IngredientId = detail.IngredientId,
                        Unit = detail.Unit,
                        IngredientName = db.Ingredients
                             .Where(i => i.IngredientId == detail.IngredientId)
                             .Select(i => i.IngredientName)
                             .FirstOrDefault(),
                        Quantity = detail.Quantity,
                        ImportPrice = detail.ImportPrice
                    }).ToList()
                })
                .ToList();  // Bring the data into memory

            // Apply filter on the client side if selectedDate is provided
            if (selectedDate.HasValue)
            {
                imports = imports.Where(i => i.EntryDate.Date == selectedDate.Value.Date).ToList();
            }

            // Paging
            var importsList = imports.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            PagedList<InventoryEntryViewModel> list = new PagedList<InventoryEntryViewModel>(importsList, pageNumber, pageSize);

            // Store selected date in ViewBag for use in the view
            ViewBag.SelectedDate = selectedDate;

            return View(list);
        }





        [Route("CreateImport")]
        [HttpGet]
        public IActionResult CreateImport()
        {
            var lis = db.Ingredients.ToList();

            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the ID as "yyyyMMddHHmmss" (e.g., 20231103120000 for November 3, 2024, 12:00:00 PM)
            ViewBag.ID = currentDateTime.ToString("yyyyMMddHHmmss");

            // Format the Date as "yyyy-MM-dd" (e.g., 2024-11-03)
            ViewBag.Date = currentDateTime.ToString("yyyy-MM-dd");

            return View(lis);
        }
        [Route("CreateImport")]
        [HttpPost]
        public IActionResult CreateImport([FromBody] InventoryEntry model)
        {
            if (model == null )
            {
                Console.WriteLine("dữ liệu k hợp lệ model");
                return BadRequest("Dữ liệu nhập vào không hợp lệ.");
            }
            if (model.EntryDetails == null )
            {
                Console.WriteLine("dữ liệu k hợp lệ modle2");
                return BadRequest("Dữ liệu nhập vào không hợp lệ.");
            }
            if (!model.EntryDetails.Any())
            {
                Console.WriteLine("dữ liệu k hợp lệ modle3");
                return BadRequest("Dữ liệu nhập vào không hợp lệ.");
            }

            Console.WriteLine("Received model: ");
            Console.WriteLine($"EntryId: {model.EntryId}");
            Console.WriteLine($"EntryDate: {model.EntryDate}");
            Console.WriteLine($"Description: {model.Description}");
            Console.WriteLine($"EntryDetail: {model.EntryDetails.Count()}");


            if (model == null || model.EntryDetails == null || !model.EntryDetails.Any())
            {
                Console.WriteLine("dữ liệu k hợp lệ");
                return BadRequest("Dữ liệu nhập vào không hợp lệ.");
            }
            // Tính toán tổng giá cho đơn nhập kho từ các EntryDetail
            model.TotalPrice = model.EntryDetails.Sum(detail => detail.ImportPrice * detail.Quantity);
            model.EntryDate = DateOnly.FromDateTime(DateTime.Now); // Gán ngày nhập hiện tại

            try
            {
                // Lưu InventoryEntry vào cơ sở dữ liệu trước để có thể tạo ra EntryId
                db.InventoryEntries.Add(model);
                db.SaveChanges();

                // Lặp qua từng EntryDetail để cập nhật Ingredient và lưu EntryDetail
                foreach (var detail in model.EntryDetails)
                {
                    // Cập nhật số lượng của Ingredient
                    var ingredient = db.Ingredients.FirstOrDefault(i => i.IngredientId == detail.IngredientId);
                    if (ingredient != null)
                    {
                        ingredient.Quantity += detail.Quantity; // Cộng số lượng nhập vào
                        ingredient.ImportDate = DateOnly.FromDateTime(DateTime.Now); // Cập nhật ngày nhập
                    }

                    // Thiết lập EntryId cho từng EntryDetail để tham chiếu đến InventoryEntry
                    detail.EntryId = model.EntryId;

                    // Thêm EntryDetail vào cơ sở dữ liệu
                    db.EntryDetails.Add(detail);
                }

                // Lưu tất cả các thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return Ok("Đơn nhập kho đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần)
                Console.WriteLine("Lỗi khi tạo đơn nhập kho:", ex);

                // Trả về lỗi cho client
                return StatusCode(500, "Đã xảy ra lỗi khi tạo đơn nhập kho.");
            }
        }



        [Route("Export")]
        [HttpGet]
        public IActionResult Export(int? page, DateTime? selectedDate)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Join InventoryExits and Ingredients to get IngredientName
            var exports = db.InventoryExits.Select(e => new InventoryExitViewModel
            {
                ExitId = e.ExitId,
                ExitDate = e.ExitDate.ToDateTime(TimeOnly.MinValue),
                Description = e.Description,
                RecipientName = e.RecipientName,
                ExitDetails = e.ExitDetails.Select(d => new ExitDetailViewModel
                {
                    DetailId = d.DetailId,
                    IngredientId = d.IngredientId,
                    IngredientName = db.Ingredients
                             .Where(i=> i.IngredientId == d.IngredientId)
                             .Select(i => i.IngredientName)
                             .FirstOrDefault(),
                    Unit = d.Unit,
                    Quantity = d.Quantity
                   
                }).ToList()
            }).ToList();


            if (selectedDate.HasValue)
            {
                exports = exports.Where(i => i.ExitDate.Date == selectedDate.Value.Date).ToList();
            }

            // Paging
            var exportsList = exports.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PagedList<InventoryExitViewModel> list = new PagedList<InventoryExitViewModel>(exportsList, pageNumber, pageSize);

            // Store selected date in ViewBag for use in the view
            ViewBag.SelectedDate = selectedDate;
 

            return View(list);
        }



        [Route("CreateExport")]
        [HttpGet]
        public IActionResult CreateExport()
        {
          

            return View();
        }

        



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
