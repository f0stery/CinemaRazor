using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaRazor.Data
{
    public static class SeedData
    {
        private const int DefaultRows = 3;
        private const int DefaultSeatsPerRow = 5;

        public static async Task EnsureSeatLayoutAsync(CinemaContext context)
        {
            var expectedSeats = new HashSet<(int Row, int Seat)>();

            for (var row = 1; row <= DefaultRows; row++)
            {
                for (var seat = 1; seat <= DefaultSeatsPerRow; seat++)
                {
                    expectedSeats.Add((row, seat));
                }
            }

            var existingSeats = await context.Seats
                .AsNoTracking()
                .ToListAsync();

            // Add missing seats
            foreach (var (row, seatNumber) in expectedSeats)
            {
                if (!existingSeats.Any(s => s.RowNumber == row && s.SeatNumber == seatNumber))
                {
                    context.Seats.Add(new Seat
                    {
                        RowNumber = row,
                        SeatNumber = seatNumber
                    });
                }
            }

            // Remove seats that fall outside the desired layout (if they are not used)
            var extraSeats = existingSeats
                .Where(s => !expectedSeats.Contains((s.RowNumber, s.SeatNumber)))
                .ToList();

            if (extraSeats.Any())
            {
                var extraSeatIds = extraSeats.Select(s => s.Id).ToList();

                var lockedSeatIds = await context.Tickets
                    .Where(t => extraSeatIds.Contains(t.SeatId))
                    .Select(t => t.SeatId)
                    .Distinct()
                    .ToListAsync();

                var removableSeats = extraSeats
                    .Where(s => !lockedSeatIds.Contains(s.Id))
                    .ToList();

                if (removableSeats.Any())
                {
                    context.Seats.RemoveRange(removableSeats);
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
