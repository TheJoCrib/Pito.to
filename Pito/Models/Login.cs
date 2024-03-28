using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pito.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }

        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool Anonymous { get; set; }
    }
}
