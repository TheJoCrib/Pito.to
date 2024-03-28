using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Pito.Models
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
