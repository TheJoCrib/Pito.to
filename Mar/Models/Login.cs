using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Mar.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]

        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
        public bool Anonymous { get; set; }
    }
}
