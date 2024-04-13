using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.View.Areas.Merchant.Controllers
{
    [AllowAnonymous]
    [Area("Merchant")]
    public class MerchantController : Controller
    {
                
        public IActionResult Index()
        {
            return View();
        }
    }
}
