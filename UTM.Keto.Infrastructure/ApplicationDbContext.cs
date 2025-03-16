using System;
using System.Data.Entity;
using UTM.Keto.Domain;

namespace UTM.Keto.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("KetoDbConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // настройка связей
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);
        }
    }
}