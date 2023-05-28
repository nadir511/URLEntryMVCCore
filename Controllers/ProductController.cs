using Microsoft.AspNetCore.Mvc;

namespace URLEntryMVC.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
