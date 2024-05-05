using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pito.Models
{
    public class Library
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public string Image { get; set; }
    }
}
