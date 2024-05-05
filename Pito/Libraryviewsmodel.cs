using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pito
{
    public class Libraryviewsmodel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

        [NotMapped]
        public IFormFile photo { get; set; }
    }
}
