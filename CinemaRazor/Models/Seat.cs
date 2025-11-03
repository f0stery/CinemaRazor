using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaRazor.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Номер ряда")]
        public int RowNumber { get; set; }

        [Display(Name = "Номер места")]
        public int SeatNumber { get; set; }
    }
}
