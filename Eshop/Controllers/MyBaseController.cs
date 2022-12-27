using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Controllers
{
    public class MyBaseController : Controller
    {
        protected EshopContext _context;

        protected readonly IWebHostEnvironment _environment;

        public MyBaseController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["Products_Cart"] = HttpContext.Session.GetInt32("sum-cart");

            ViewData["ProductTypes"] = _context.ProductTypes.Include(a=>a.Products);
            ViewData["Products_Count"] = _context.Products.Count();

            ViewData["ListTypes"] = from item in _context.Products.Include(a => a.ProductType).ToList()
                                    group item by item.ProductType into g
                                    orderby g.Key.Id
                                    select new { Id = g.Key.Id, Name = g.Key.Name, Num = g.Count() };


            int? id = HttpContext.Session.GetInt32("user-id");
            if (id != null)
            {
                ViewData["User"] = _context.Accounts.Find(id);
            }

            ViewData["Carts"] = GetListCart();

            base.OnActionExecuting(filterContext);
        }
        protected bool IsInCarts(int id)
        {
            return HttpContext.Session.GetInt32("cart-product-" + id.ToString()) != null;
        }
        protected int GetQuantity(int id)
        {
            return Convert.ToInt32(HttpContext.Session.GetInt32("quantity-" + id.ToString()));
        }
        protected List<Cart> GetListCart()
        {
            List<Cart> lstCart;
            lstCart = new List<Cart>();

            foreach (var item in _context.Products)
            {
                if (IsInCarts(item.Id))
                {
                    lstCart.Add(new Cart
                    {
                        ProductId = item.Id,
                        Quantity = GetQuantity(item.Id),
                        Product = _context.Products.Find(item.Id),
                    });
                }
            }

            return lstCart;
        }
    }
}
