using Microsoft.AspNetCore.Authorization; //used for the [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using MyCRM.Models;
using System.Diagnostics;

namespace MyCRM.Controllers
{
    //[Authorize] //Comment out this attribute, if you want to start from home page
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}