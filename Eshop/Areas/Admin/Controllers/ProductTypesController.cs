using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Eshop.Controllers;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : MyBaseController
    {
        public ProductTypesController(EshopContext context, IWebHostEnvironment environment)
            : base(context, environment)
        {
        }

        
    }
}
