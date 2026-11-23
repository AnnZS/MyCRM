using Microsoft.AspNetCore.Mvc;
using MyCRM.Models;
using MyCRM.ViewModels;
using System.Diagnostics;

namespace MyCRM.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Analytics()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Leads() => View("CrmList", new CrmListViewModel(
            "Leads", "No leads yet.", ["Name", "Company", "Email", "Lead status", "Owner"]));

        [HttpGet]
        public IActionResult Contacts() => View("CrmList", new CrmListViewModel(
            "Contacts", "No contacts yet.", ["Name", "Account", "Email", "Phone", "Job title"]));

        [HttpGet]
        public IActionResult Accounts() => View("CrmList", new CrmListViewModel(
            "Accounts", "No accounts yet.", ["Account name", "Industry", "Phone", "Website", "Owner"]));

        [HttpGet]
        public IActionResult Opportunities() => View("CrmList", new CrmListViewModel(
            "Opportunities", "No opportunities yet.", ["Opportunity name", "Account", "Stage", "Amount", "Close date"]));

        [HttpGet]
        public IActionResult Products() => View("CrmList", new CrmListViewModel(
            "Products", "No products yet.", ["Product name", "Product code", "Product family", "Status"]));

        [HttpGet]
        public IActionResult PriceBooks() => View("CrmList", new CrmListViewModel(
            "Price Books", "No price books yet.", ["Price book name", "Description", "Status"]));

        [HttpGet]
        public IActionResult Calendar() => View("CrmList", new CrmListViewModel(
            "Calendar", "No events yet.", ["Event", "Start", "End", "Location", "Related to"]));

        [HttpGet]
        public IActionResult Cases() => View("CrmList", new CrmListViewModel(
            "Cases", "No cases yet.", ["Case number", "Subject", "Account", "Status", "Priority"]));

        [HttpGet]
        public IActionResult Campaigns() => View("CrmList", new CrmListViewModel(
            "Campaigns", "No campaigns yet.", ["Campaign name", "Type", "Status", "Start date", "End date"]));

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
