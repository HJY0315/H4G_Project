using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using H4G_Project.DAL;
using H4G_Project.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Firebase.Storage;
using Google.Cloud.Firestore.V1;
using System.Dynamic;


namespace H4G_Project.Controllers
{
    public class ApplicationController : Controller
    {
        ApplicationDAL applicationContext = new ApplicationDAL();

        // Show list of applications
        public async Task<ActionResult> Index()
        {
            var applications = await applicationContext.GetAllApplications();
            return View(applications);
        }

        // Show the form (GET)
        [HttpGet]
        public IActionResult NewApplication()
        {
            return View();
        }

        // Handle form submission (POST)
        [HttpPost]
        public async Task<IActionResult> NewApplication(Application application, IFormFile medicalReport)
        {
            bool success = await applicationContext.AddApplication(application, medicalReport);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }
    }
}