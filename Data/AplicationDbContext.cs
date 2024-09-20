using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Models;

namespace ProductsApiRest.Data
{
     // Constructor para la configuración de opciones en la BD
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } //Denifición de la coleccion de productos en la BD
        public DbSet<Warehouse> Warehouse { get; set; } //Denifición de la coleccion de productos en la BD

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany()
                .HasForeignKey(p => p.IdWarehouse)
                .IsRequired(false); // Relación opcional
        }
    }
}
