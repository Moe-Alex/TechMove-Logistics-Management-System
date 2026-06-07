using Microsoft.EntityFrameworkCore;
using TechMove.Models;

namespace TechMove.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Contract)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostUSD)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.ExchangeRateUsed)
                .HasPrecision(18, 6);

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.LocalCostZAR)
                .HasPrecision(18, 2);
        }
    }
}