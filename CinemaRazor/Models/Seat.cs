using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "Сеанс")]
        public int SessionId { get; set; }

        public Session Session { get; set; }

        [Display(Name = "Занято")] 
        public bool IsOccupied { get; set; }
    }
}
