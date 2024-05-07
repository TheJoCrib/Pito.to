using System.ComponentModel.DataAnnotations;

namespace Mar.Models
{
    public class ThreadModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public string? Text { get; set; }

        public int TopicId { get; set; }
        public DateTime? Date { get; set; }
        public bool isPinned { get; set; } = false;
        public string? AuthorName { get; set; } // assuming the username is stored in User.Identity.Name

        public int ViewCount { get; set; }
    }
}
