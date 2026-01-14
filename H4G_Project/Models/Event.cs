using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace H4G_Project.Models
{
    [FirestoreData]
    public class Event
    {
        // Firestore document ID (not stored as a field)
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, ErrorMessage = "Event name cannot exceed 100 characters")]
        public string Name { get; set; }

        [FirestoreProperty("start")]
        [Required]
        public DateTime Start { get; set; }

        [FirestoreProperty("end")]
        public DateTime? End { get; set; }
    }
}
