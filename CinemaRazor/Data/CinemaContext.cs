using Microsoft.EntityFrameworkCore;
using CinemaRazor.Models;

namespace CinemaRazor.Data
{
    public class CinemaContext : DbContext
    {
        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🎟 Ticket → Seat (каскадное удаление)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🎞 Ticket → Session (с правильной обратной связью!)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Session)
                .WithMany(s => s.Tickets) // ✅ добавлено сюда
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🎬 Session → Movie
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Movie)
                .WithMany()
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // 💰 Настройка точности цен
            modelBuilder.Entity<Session>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Seat>()
                .HasOne(seat => seat.Session)
                .WithMany(session => session.Seats)
                .HasForeignKey(seat => seat.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
