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

        [Required]
        [StringLength(1000)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Фирма производитель")]
        public string ProducerCompany { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Страна производитель")]
        public string ProductionCountry { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Актёры")]
        public string Actors { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Возрастные ограничения")]
        public int? AgeRating { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Дата выхода")]
        public DateTime ReleaseDate { get; set; }

        [Range(1, 600)]
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
