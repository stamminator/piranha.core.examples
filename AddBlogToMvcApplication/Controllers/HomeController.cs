using Microsoft.AspNetCore.Mvc;
using System;

namespace AddBlogToMvcApplication.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// This is the home page.
        /// </summary>
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }
    }
}
