using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Eshop.Controllers
{
    public class ProductsController : MyBaseController
    {
        public ProductsController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }

        public IActionResult ShopList(int p = 1)
        {
            var products = _context.Products.Skip((p - 1) * 10).Take(10).ToList();
            HttpContext.Session.SetString("Page-now", "ShopList");


            
            return View(products);
        }
        public IActionResult ShopDetails(int? id)
        {
            if (id == null)
                return NotFound();
            var product = _context.Products.Include(a=>a.ProductType).ToList().Find(a => a.Id == id);
            
            if (product == null)
                return NotFound();

            HttpContext.Session.SetString("Page-now", "ShopDetails");

            ViewData["Products"] = _context.Products
                .Where(a => a.ProductTypeId == product.ProductTypeId && a.Id != product.Id);

            
            return View(product);
        }
        public IActionResult AddToCart(int id, int? like, int quantity = 1)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            if (IsInCarts(id))
            {
                int q = GetQuantity(id);
                HttpContext.Session.SetInt32("quantity-" + id.ToString(), q + quantity);
            }
            else
            {
                HttpContext.Session.SetInt32("cart-product-" + id.ToString(), id);
                HttpContext.Session.SetInt32("quantity-" + id.ToString(), quantity);
            }

            HttpContext.Session.SetInt32("sum-cart", GetListCart().Count);

            switch(HttpContext.Session.GetString("Page-now"))
            {
                case "Index":
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    break;
                case "ShopList":
                    {
                        return RedirectToAction("ShopList", "Products");
                    }
                    break;
                default:
                    {
                        if (like != null)
                        {
                            return RedirectToAction("ShopDetails", "Products", new { id = like });
                        }
                        return RedirectToAction("ShopDetails", "Products", new { id = id });
                    }
                    break;
            }    
            
        }
        public IActionResult AddToCartAPI(int id, int? like, int quantity = 1)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            if (IsInCarts(id))
            {
                int q = GetQuantity(id);
                HttpContext.Session.SetInt32("quantity-" + id.ToString(), q + quantity);
            }
            else
            {
                HttpContext.Session.SetInt32("cart-product-" + id.ToString(), id);
                HttpContext.Session.SetInt32("quantity-" + id.ToString(), quantity);
            }

            HttpContext.Session.SetInt32("sum-cart", GetListCart().Count);

            List<int> lstint = new List<int>();
            lstint.Add(GetListCart().Count);

           return new JsonResult(lstint);

        }
        public IActionResult FilterTypes(int id, int p = 1)
        {
            var lstproduct = _context.Products.Include(a=>a.ProductType).Where(a => a.ProductTypeId == id).Skip((p - 1) * 10).Take(10).ToList();
            HttpContext.Session.SetString("Page-now", "ShopList");
            
            ViewData["Type"] = _context.ProductTypes.Find(id).Name;
            return View("ShopList", lstproduct);
        }
        public IActionResult FilterAll(int p = 1)
        {
            var lstproduct = _context.Products.Skip((p - 1) * 10).Take(10).ToList();

            return View("ShopList", lstproduct);
        }
        public IActionResult FilterWorld (int p = 1)
        {
            var lstproduct = _context.Products.Skip((p - 1) * 10).Take(10).ToList();

            return View("ShopList", lstproduct);
        }
    }
}
