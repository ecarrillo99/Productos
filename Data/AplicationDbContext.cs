using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Models;

namespace ProductsApiRest.Data
{
     // Constructor para la configuración de opciones en la BD
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } //Denifición de la coleccion de clientes en la BD
    }
}
