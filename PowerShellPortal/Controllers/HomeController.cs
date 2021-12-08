using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PowerShellPortal.Models;
using System.Diagnostics;
using System.Management.Automation;

namespace PowerShellPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<OutputHub> _hubContext;

        public HomeController(IWebHostEnvironment webHostEnvironment, IHubContext<OutputHub> hubContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string target)
        {
            // If user provides path, ensure all "\" are converted to "\\"
            target = target.Replace(@"\", @"\\");

            // Prepend with "~\Scripts\Powershell\" - required location of PowerShell scripts
            target = @"Scripts\" + target;

            string pscommand = Path.Combine(_webHostEnvironment.ContentRootPath, target);

            PowerShell shell = PowerShell.Create();

            // Setup powershell command and execute
            shell.AddCommand(pscommand);
            //    .AddParameter("target", target); //Add PowerShell script parameters here if you need

            PowerShellController powerShellController = new PowerShellController(_hubContext);

            powerShellController.RunScript(shell, true);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult Results()
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