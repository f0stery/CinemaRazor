using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaRazor.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [Range(16, 100)]
        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [StringLength(10)]
        [Display(Name = "Пол")]
        public string? Gender { get; set; }

        [StringLength(150)]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Required]
        [Display(Name = "Должность")]
        public int PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        [Display(Name = "Должность")]
        public Position? Position { get; set; }   // ← вот тут добавили "?" и убрали обязательность
    }
}
