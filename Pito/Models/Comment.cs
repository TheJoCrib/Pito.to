using System;
using System.ComponentModel.DataAnnotations;

namespace Pito.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "The UserId field is required.")]
        public int UserId { get; set; } // Foreign key for User

        // Navigation property, using Login as User
        public virtual Login User { get; set; }

        [Required(ErrorMessage = "The PostId field is required.")]
        public int PostId { get; set; }

        // Navigation property
        public virtual Post Post { get; set; }

        [Required(ErrorMessage = "The Content field is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "The DatePosted field is required.")]
        public DateTime DatePosted { get; set; }
    }
}
