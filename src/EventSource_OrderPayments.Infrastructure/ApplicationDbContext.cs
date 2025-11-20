using EventSource_OrderPayments.Infrastructure.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace EventSource_OrderPayments.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<OrderReadModel> Orders { get; set; }
        public DbSet<OrderItemReadModel> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemReadModel>()
                .HasOne<OrderReadModel>()
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}