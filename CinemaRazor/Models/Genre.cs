using System.ComponentModel.DataAnnotations;

namespace CinemaRazor.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Жанр")]
        public string Name { get; set; }
    }
}
