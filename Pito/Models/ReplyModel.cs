﻿using System.ComponentModel.DataAnnotations;

namespace Pito.Models
{
    public class ReplyModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Text is required.")]
        public string? Text { get; set; }
        public int ThreadId { get; set; }
        public int QuotedReplyId { get; set; }
        public DateTime? Date { get; set; }
        public bool isPinned { get; set; } = false;

        public string? AuthorName { get; set; } // assuming the username is stored in User.Identity.Name

    }
}
