using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaRazor.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Дата и время начала")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Дата и время окончания")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Цена билета")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        [ValidateNever]
        public Movie Movie { get; set; }

        [ValidateNever]
        public ICollection<Ticket> Tickets { get; set; }
    }

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session Session { get; set; }

        [Required]
        public int SeatId { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat Seat { get; set; }

        [Display(Name = "Цена")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Дата покупки")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}
