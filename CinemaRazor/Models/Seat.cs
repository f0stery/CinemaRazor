using System.ComponentModel.DataAnnotations;

namespace CinemaRazor.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Номер ряда")]
        [Range(1, 100)]
        public int RowNumber { get; set; }

        [Display(Name = "Номер места")]
        [Range(1, 200)]
        public int SeatNumber { get; set; }
    }
}
