using Eshop.Controllers;
using Eshop.Data;
using Eshop.Migrations;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : MyBaseController
    {
        public AccountsController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {

        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("Page-now", "Index");
            var accounts = _context.Accounts.Where(a => a.IsAdmin == true).ToList();
            return View(accounts);
        }
    }
}
