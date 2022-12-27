using Eshop.Data;
using Eshop.Migrations;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Controllers
{
    public class AccountsController : MyBaseController
    {
        public AccountsController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {

        }
        
        public IActionResult Index()
        {
            HttpContext.Session.SetString("Page-now", "Index");
            return View();
        }
        public IActionResult MyProflie()
        {
            return View();
        }
        //[HttpGet]
        //public IActionResult Signin()
        //{
        //    return View();
        //}
        [HttpPost]
        public IActionResult Signin(string Username, string Password)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Username == Username && a.Password == Password);
            if (account != null)
            {
                HttpContext.Session.SetInt32("user-id", account.Id);
                //if (account.IsAdmin)
                //{
                //    return RedirectToAction("Index", "Home", new { area = "Admin" });
                //}
                return RedirectToAction("LoadCart", "Carts");
            }
            else
            {
                ViewBag.ErrorMsg = "Login failed!";
                return NotFound();
            }
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup([Bind("AvatarFile, Username, Password, Email, Phone, Address, FullName")] Account account)
        {
           
                _context.Accounts.Add(account);
                _context.SaveChanges();

                if (account.AvatarFile != null)
                {
                    var fileName = account.Id.ToString() + Path.GetExtension(account.AvatarFile.FileName);
                    var uploadPath = Path.Combine(_environment.WebRootPath, "img");
                    var filePath = Path.Combine(uploadPath, fileName);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        account.AvatarFile.CopyTo(fs);
                        fs.Flush();
                    }
                    account.Avatar = fileName;
                }
                else
                {
                    account.Avatar = "defaultavatar.png";
                }
                account.IsAdmin = false;
                _context.Accounts.Update(account);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("user-id", account.Id);
               
           
            return RedirectToAction("Index", "Home");

        }
        public IActionResult Signout()
        {
            CreateCart();

            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Append("MySession", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });

            return RedirectToAction("Index", "Home");

        }
        private void CreateCart()
        {
            int? id = HttpContext.Session.GetInt32("user-id");
            if (id != null)
            {
                List<Cart> lstMyCart = _context.Carts
                  .Where(a => a.AccountId == Convert.ToInt32(id)).ToList();

                foreach (var item in GetListCart())
                {
                    var cart = lstMyCart.FirstOrDefault(a => a.ProductId == item.ProductId);
                    if (cart != null)
                    {
                        cart.Quantity = item.Quantity;
                    }
                    else
                    {
                        _context.Carts.Add(new Cart
                        {
                            AccountId = Convert.ToInt32(id),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                    }
                }
                _context.SaveChanges();
                foreach (var item in lstMyCart)
                {
                    var cart = GetListCart().FirstOrDefault(a => a.ProductId == item.ProductId);
                    if (cart == null)
                    {
                        _context.Carts.Remove(item);
                    }
                }
                _context.SaveChanges();

            }
        }

    }
}
