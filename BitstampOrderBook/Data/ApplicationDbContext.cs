using BitstampOrderBook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BitstampOrderBook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<OrderBook> OrderBooks { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderBook>().HasKey(ob => ob.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);

            modelBuilder.Entity<OrderBook>()
                .HasMany(o => o.Bids)
                .WithOne()
                .HasForeignKey(o => o.OrderBookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderBook>()
                .HasMany(o => o.Asks)
                .WithOne()
                .HasForeignKey(o => o.OrderBookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderBook)
                .WithMany(ob => ob.Bids)
                .HasForeignKey(o => o.OrderBookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
