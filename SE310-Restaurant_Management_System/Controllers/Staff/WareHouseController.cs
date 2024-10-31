using Microsoft.AspNetCore.Mvc;
using SE310_Restaurant_Management_System.Models;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace SE310_Restaurant_Management_System.Controllers.Staff
{
    public class WareHouseController : Controller
    {
        QlnhaHangContext db = new QlnhaHangContext();
        private readonly ILogger<WareHouseController> _logger;

        public WareHouseController(ILogger<WareHouseController> logger)
        {
            _logger = logger;
        }

        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Import(InventoryExit inventoryExit)
        {
            return View(inventoryExit);
        }

        [HttpGet]
        public IActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Export(InventoryExit inventoryExit)
        {
            return View(inventoryExit);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
