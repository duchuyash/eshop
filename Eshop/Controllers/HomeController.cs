using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Principal;

namespace Eshop.Controllers
{
    public class HomeController : MyBaseController
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetString("Page-now", "Index");
            return View();
        }

        public IActionResult Contact()
        {
            HttpContext.Session.SetString("Page-now", "Contact");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}