using System.ComponentModel.DataAnnotations;

namespace H4G_Project.Models
{
    public class EventViewModel
    {
        public string Id { get; set; } = string.Empty;
        public int eventID { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 100 characters")]
        [Display(Name = "Event Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event details are required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Event details must be between 10 and 1000 characters")]
        [Display(Name = "Event Details")]
        public string Details { get; set; } = string.Empty;

        public string eventPhoto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event start date and time is required")]
        [Display(Name = "Start Date & Time")]
        public DateTime? Start { get; set; }

        [Display(Name = "End Date & Time")]
        public DateTime? End { get; set; }

        [Required(ErrorMessage = "Registration due date is required")]
        [Display(Name = "Registration Due Date")]
        public DateTime? RegistrationDueDate { get; set; }

        [Required(ErrorMessage = "Maximum participants is required")]
        [Range(1, 10000, ErrorMessage = "Maximum participants must be between 1 and 10,000")]
        [Display(Name = "Maximum Participants")]
        public int MaxParticipants { get; set; }

        public string QrCode { get; set; } = string.Empty;
    }
}