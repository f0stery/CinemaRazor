using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Должность")]
        public string Title { get; set; } = string.Empty;

        [Precision(10, 2)]
        [Range(0, 100000)]
        [Display(Name = "Оклад")]
        public decimal Salary { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Обязанности")]
        public string Responsibilities { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Требования")]
        public string Requirements { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; }
    }
}
