using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Eshop.Controllers;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : MyBaseController
    {
        public HomeController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }
        public IActionResult Index()
        {
            var user = _context.Accounts.Find(HttpContext.Session.GetInt32("user-id"));
            if(user == null || !user.IsAdmin)
                return RedirectToAction("Index", "Home");
            return View();
        }
    }
}

       