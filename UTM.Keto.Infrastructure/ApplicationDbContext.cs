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
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }

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
                
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);
                
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);
                
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
                
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);
                
            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
                
            modelBuilder.Entity<Review>()
                .HasRequired(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
                
            modelBuilder.Entity<SupportTicket>()
                .HasRequired(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId);
                
            modelBuilder.Entity<TicketMessage>()
                .HasRequired(tm => tm.Ticket)
                .WithMany(st => st.Messages)
                .HasForeignKey(tm => tm.TicketId)
                .WillCascadeOnDelete(false);
                
            modelBuilder.Entity<TicketMessage>()
                .HasRequired(tm => tm.User)
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}