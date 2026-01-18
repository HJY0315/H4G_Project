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
using System.Net;
using System.Net.Mail;

namespace H4G_Project.Controllers
{
    public class ApplicationController : Controller
    {
        ApplicationDAL applicationContext = new ApplicationDAL();
        EventsDAL eventsContext = new EventsDAL();
        private readonly IConfiguration _config;

        public ApplicationController(IConfiguration config)
        {
            _config = config;
        }

        // Show list of applications with tabs for client applications and volunteer registrations
        public async Task<ActionResult> Index()
        {
            var applications = await applicationContext.GetAllApplications();

            // Sort applications: Pending first, then Declined, then Approved at bottom
            var sortedApplications = applications
                .OrderBy(a => a.Status == "Pending" ? 0 : (a.Status == "Declined" ? 1 : 2))
                .ToList();

            return View(sortedApplications);
        }

        // Show volunteer registrations page
        public async Task<ActionResult> VolunteerRegistrations()
        {
            var volunteerRegistrations = await eventsContext.GetVolunteerRegistrations();
            var allEvents = await eventsContext.GetAllEvents();

            // Sort volunteer registrations: Pending first, then Declined, then Approved at bottom
            var sortedVolunteerRegistrations = volunteerRegistrations
                .OrderBy(v => v.Status == "Pending" ? 0 : (v.Status == "Declined" ? 1 : 2))
                .ToList();

            ViewBag.AllEvents = allEvents;
            return View(sortedVolunteerRegistrations);
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

        // Approve application
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(string applicationId, string applicantName, string applicantEmail)
        {
            try
            {
                // Don't update status here - only redirect to user creation
                // Status will be updated to "Approved" only when user account is successfully created
                return RedirectToAction("AddUser", "Staff", new
                {
                    applicantName = applicantName,
                    applicantEmail = applicantEmail,
                    applicationId = applicationId
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error processing application: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Reject application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApplication(string applicationId, string applicantName, string applicantEmail)
        {
            try
            {
                bool success = await applicationContext.UpdateApplicationStatus(applicationId, "Declined");

                if (success)
                {
                                        var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(
                            _config["EmailSettings:SenderEmail"],
                            _config["EmailSettings:SenderPassword"]
                        ),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_config["EmailSettings:SenderEmail"]),
                        Subject = "Application Status Update",
                        Body = $"Dear {applicantName},<br/><br/>We regret to inform you that your application has been declined.<br/><br/>Regards,<br/>Support Team",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(applicantEmail);

                    await smtpClient.SendMailAsync(mailMessage);

                    TempData["SuccessMessage"] = $"Application for {applicantName} declined successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to decline application.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error declining application: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Approve volunteer registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVolunteerRegistration(string registrationId, string volunteerName, string volunteerEmail)
        {
            try
            {
                bool success = await eventsContext.UpdateVolunteerRegistrationStatus(registrationId, "Approved");

                if (success)
                {
                    TempData["SuccessMessage"] = $"Volunteer registration for {volunteerName} approved successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve volunteer registration.";
                }

                return RedirectToAction("VolunteerRegistrations");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving volunteer registration: {ex.Message}";
                return RedirectToAction("VolunteerRegistrations");
            }
        }

        // Reject volunteer registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVolunteerRegistration(string registrationId, string volunteerName, string volunteerEmail)
        {
            try
            {
                bool success = await eventsContext.UpdateVolunteerRegistrationStatus(registrationId, "Declined");

                if (success)
                {
                    TempData["SuccessMessage"] = $"Volunteer registration for {volunteerName} declined successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to decline volunteer registration.";
                }

                return RedirectToAction("VolunteerRegistrations");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error declining volunteer registration: {ex.Message}";
                return RedirectToAction("VolunteerRegistrations");
            }
        }
    }
}