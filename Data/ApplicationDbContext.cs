using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;
namespace ProductManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    //This property represents the Products table in the database.
    //It allows you to query and save instances of the Product entity.
    public DbSet<Product> Products { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.UnitPrice).HasPrecision(18, 2);
    }
}
