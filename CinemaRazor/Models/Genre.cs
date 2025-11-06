using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaRazor.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Жанр")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        // 🔗 Добавлено: один жанр может содержать несколько фильмов
        [ValidateNever]
        public ICollection<Movie>? Movies { get; set; }
    }
}
