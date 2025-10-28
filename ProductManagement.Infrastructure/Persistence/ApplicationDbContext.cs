using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using System.Reflection;

namespace ProductManagement.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}


