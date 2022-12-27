using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Principal;
using Eshop.Migrations;

namespace Eshop.Controllers
{
    public class CartsController : MyBaseController
    {
        public CartsController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }
        public IActionResult ShoppingCart()
        {
            HttpContext.Session.SetString("Page-now", "ShoppingCart");
            return View();
        }
        public IActionResult Delete_ProductCart(int id)
        {
            HttpContext.Session.Remove("cart-product-" + id.ToString());
            HttpContext.Session.Remove("quantity-" + id.ToString());
            HttpContext.Session.SetInt32("sum-cart", GetListCart().Count);

            return RedirectToAction("ShoppingCart", "Carts");
        }
        public IActionResult Checkout()
        {
            HttpContext.Session.SetString("Page-now", "Checkout");
            return View();
        }
          
        [HttpPost]
        public IActionResult Checkout([Bind("AccountId, ShippingPhone, ShippingAddress")] Invoice invoice)
        {
            //add hóa đơn
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            foreach (var item in ViewBag.Carts)
            {
                invoice.Total += item.Quantity * item.Product.Price;
            }
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            
            //add chi tiết hóa đơn
            var id =_context.Invoices.Max(x => x.Id);

            foreach (var item in ViewBag.Carts)
            {
                invoiceDetails.Add(new InvoiceDetail
                {
                    InvoiceId = id,
                    ProductId = item.ProductId,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                });
            }
            _context.InvoiceDetails.AddRange(invoiceDetails);

            //xóa giỏ hàng
            _context.Carts.RemoveRange(
                _context.Carts.Where(a => a.AccountId == invoice.AccountId).ToList()
            );

            _context.SaveChanges();


            List<Cart> lstMyCart = _context.Carts
                   .Where(a => a.AccountId == Convert.ToInt32(id)).ToList();

            foreach (var item in GetListCart())
            {
                HttpContext.Session.Remove("cart-product-" + item.ProductId.ToString());
                HttpContext.Session.Remove("quantity-" + item.ProductId.ToString());
            }
            HttpContext.Session.SetInt32("sum-cart", GetListCart().Count);

            return RedirectToAction("LoadCart") ;
        }
        public IActionResult LoadCart()
        {
            int? id = HttpContext.Session.GetInt32("user-id");
            if (id != null)
            {
                List<Cart> lstMyCart = _context.Carts
                    .Where(a => a.AccountId == Convert.ToInt32(id)).ToList();

                if (lstMyCart.Count != 0)
                {
                    foreach (var item in lstMyCart)
                    {
                        if (IsInCarts(item.ProductId))
                        {
                            HttpContext.Session.SetInt32("quantity-" + item.ProductId.ToString(), (item.Quantity + GetQuantity(item.ProductId)));
                        }
                        else
                        {
                            HttpContext.Session.SetInt32("cart-product-" + item.ProductId.ToString(), item.ProductId);
                            HttpContext.Session.SetInt32("quantity-" + item.ProductId.ToString(), item.Quantity);
                        }
                    }
                }

                HttpContext.Session.SetInt32("sum-cart", GetListCart().Count);
            }
            var account = _context.Accounts.Find(id);
            if (account.IsAdmin)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                switch (HttpContext.Session.GetString("Page-now"))
                {
                    case "ShopList":
                        {
                            return RedirectToAction("ShopList", "Products");
                        }
                        break;
                    case "ShoppingCart":
                        {
                            return RedirectToAction("ShoppingCart");
                        }
                        break;

                    case "Checkout":
                        {
                            return RedirectToAction("MyProfile", "Accounts");
                        }
                        break;
                    case "Contact":
                        {
                            return RedirectToAction("Contact", "Home");
                        }
                        break;

                    default:
                        {

                            return RedirectToAction("Index", "Home");
                        }
                        break;
                }
            }
        }
    }
}
