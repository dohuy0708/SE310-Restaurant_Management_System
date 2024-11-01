using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE310_Restaurant_Management_System.Models;
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
        public IActionResult Home(int ? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var ingredients = db.Ingredients
                      .Include(i => i.Type) // Join voi bang IngredientTypes
                      .ToList();
            PagedList<Ingredient> list = new PagedList<Ingredient>(ingredients, pageNumber, pageSize);

            return View(list);
        }



        [Route("Import")]
        [HttpGet]
        public IActionResult Import( int ? page )
        {
            int pageSize = 8;
            int pageNumber = page ==null || page < 0 ? 1 : page.Value;

            var imports = db.InventoryEntries.ToList();

            PagedList<InventoryEntry> list = new PagedList<InventoryEntry>(imports, pageNumber, pageSize);


            return View(list);
        }


      
        [Route("Export")]
        [HttpGet]
        public IActionResult Export(int? page)
        {
            int pageSize = 8;
            int pageNumber = page ==null || page < 0 ? 1 : page.Value;

            var exports = db.InventoryExits.ToList();

            PagedList<InventoryExit> list = new PagedList<InventoryExit>(exports, pageNumber, pageSize);


            return View(list);
           
        }
 




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
