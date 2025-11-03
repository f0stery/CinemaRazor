using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaRazor.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Название фильма")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата выхода")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Длительность (минуты)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Жанр")]
        [Required(ErrorMessage = "Выберите жанр")]
        public int GenreId { get; set; }

        [Display(Name = "Жанр")]
        [ValidateNever] // <-- вот это важно
        public Genre Genre { get; set; }
    }
}
