using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Eshop.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : MyBaseController
    {
        public ProductsController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }
        public IActionResult Index()
        {
            var user = _context.Accounts.Find(HttpContext.Session.GetInt32("user-id"));
            if (user == null || !user.IsAdmin)
                return RedirectToAction("Index", "Home");

            HttpContext.Session.SetString("Page-now", "ProductsIndex");
            var lstProducts = _context.Products.Where(a=>a.Status == true).Include(a => a.ProductType).OrderBy(a=>a.Id).ToList();

            return View(lstProducts);
        }
        public IActionResult Details(int? id)
        {
            var product = _context.Products.Include(a => a.ProductType).FirstOrDefault(a => a.Id == id);
            if (id == null || product ==null)
                return NotFound();

            ViewData["begin"] = _context.Products.OrderBy(a => a.Id).First().Id;
            ViewData["end"] = _context.Products.OrderBy(a=>a.Id).Last().Id;
            return View(product);
        }
        public IActionResult Delete(int? id)
        {
            var product = _context.Products.Find(id);
            if (id == null || product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Products", new {area ="Admin"} );
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _context.Products.Include(a => a.ProductType).FirstOrDefault(a => a.Id == id);
            if(product == null)
                return NotFound();

            ViewBag.TypeSelect = new SelectList(_context.ProductTypes, "Id", "Name");
  
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit([Bind("Id,Image,ImageFile,Name,Description,Price,Stock,ProductTypeId,SKU")] Product product)
        {
            if (product.ImageFile != null)
            {
                var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "img", "product");
                var filePath = Path.Combine(uploadPath, fileName);
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    product.ImageFile.CopyTo(fs);
                    fs.Flush();
                }
                product.Image = fileName;

            }
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
