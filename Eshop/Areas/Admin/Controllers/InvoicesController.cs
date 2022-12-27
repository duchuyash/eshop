using Microsoft.AspNetCore.Mvc;
using Eshop.Controllers;
using Eshop.Data;
using Microsoft.EntityFrameworkCore;
using Eshop.Models;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InvoicesController : MyBaseController
    {
        public InvoicesController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {

        }
        public IActionResult Index()
        {
            var user = _context.Accounts.Find(HttpContext.Session.GetInt32("user-id"));
            if (user == null || !user.IsAdmin)
                return RedirectToAction("Index", "Home");

            HttpContext.Session.SetString("Page-now", "InvoicesIndex");
            var invoices = _context.Invoices.Where(a => a.Status == false).ToList();
  
            return View(invoices);
        }
        public IActionResult Details(int? id)
        {
            List<InvoiceDetail> lstDetails = _context.InvoiceDetails.Where(a=>a.InvoiceId==id).Include(a=>a.Product).ToList();
            if (id == null || lstDetails.Count < 1)
                return NotFound();

            ViewData["Invoice"] = _context.Invoices.Where(a => a.Id == id).Include(a => a.Account).First();
            return View(lstDetails);
        }
    }
}
