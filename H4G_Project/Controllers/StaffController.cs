using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using H4G_Project.DAL;
using H4G_Project.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DateTime = System.DateTime;
using Google.Cloud.Firestore;


namespace H4G_Project.Controllers
{
    public class StaffController : Controller
    {
        StaffDAL staffContext = new StaffDAL();
        EventsDAL eventsDAL = new EventsDAL();

        public async Task<IActionResult> Index()
        {
            string memberEmail = HttpContext.Session.GetString("email");
            Staff staff = await staffContext.GetStaffByEmail(memberEmail);
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> AddNewStaff()
        {
            Staff staff = new Staff();
            return View(staff);
        }

        [HttpPost]
        public async Task<ActionResult> NewStaff(IFormCollection form)
        {
            Staff staff = new Staff
            {
                Username = form["Username"],
                Email = form["Email"],
                Password = form["Password"]
            };

            bool addUserResult = await staffContext.AddStaff(staff);

            if (addUserResult)
            {
                return RedirectToAction("Index", "Staff");
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> LogInUser()
        {
            Staff staff = new Staff();
            return View(staff);
        }

        [HttpPost]
        public async Task<ActionResult> LogInUser(IFormCollection form)
        {
            Staff staff = await staffContext.GetStaffByEmail(form["Email"]);

            if (staff != null && staff.Password == form["Password"])
            {
                HttpContext.Session.SetString("Username", staff.Username);
                TempData["Username"] = staff.Username;
                TempData.Keep("Username");
                HttpContext.Session.SetString("UserEmail", staff.Email);
                return RedirectToAction("Index", "Staff");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ======================================================
        // 🔹 EVENT MANAGEMENT (ADDED)
        // ======================================================

        // Show create event page
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View(new Event());
        }

        // Handle create event
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
    string Name,
    DateTime Start,
    DateTime? End)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ModelState.AddModelError("Name", "Event name is required");
                return View();
            }

            Event ev = new Event
            {
                Name = Name,
                Start = Timestamp.FromDateTime(Start.ToUniversalTime()),
                End = End.HasValue
                    ? Timestamp.FromDateTime(End.Value.ToUniversalTime())
                    : null
            };

            bool success = await eventsDAL.AddEvent(ev);

            if (success)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(CreateEvent));
            }

            ModelState.AddModelError("", "Failed to create event.");
            return View();
        }



        // Return events for FullCalendar
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await eventsDAL.GetAllEvents();

            var calendarEvents = events.Select(e => new
            {
                id = e.Id,
                title = e.Name,
                start = e.Start.ToDateTime(),
                end = e.End?.ToDateTime()
            });

            return Json(calendarEvents);
        }

    }
}
