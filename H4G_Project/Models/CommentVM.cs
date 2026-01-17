using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace H4G_Project.Models
{
    public class CommentVM
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Comment { get; set; }
        public string ParentCommentId { get; set; }
        public Timestamp Timestamp { get; set; }

        public List<CommentVM> Replies { get; set; } = new List<CommentVM>();
    }
}
