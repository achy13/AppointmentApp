using System.Diagnostics;
using AppointmentApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string userRole = "Anonymous";

            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    userRole = "Admin";
                else if (User.IsInRole("ServiceProvider"))
                    userRole = "ServiceProvider";
                else if (User.IsInRole("Client"))
                    userRole = "Client";
            }

            ViewBag.UserRole = userRole;

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
