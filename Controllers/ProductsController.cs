using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;

namespace ProductsApiRest.Controllers
{
    public class ProductsController : BaseCrudController<Product>
    {
        public ProductsController(ApplicationDbContext context) : base(context) { }
        // Obtener lista de productos con sus almacenes
        public override async Task<ActionResult<ApiResponse<ListData<List<Product>>>>> GetEntities(
            int page = 1,
            int pageSize = 10,
            [FromQuery] Dictionary<string, string> filters = null,
            string sortBy = "Id",
            string sortDirection = "asc")
        {
            var query = _context.Products.Include(p => p.Warehouse).AsQueryable();

            // Llamar al método de filtrado y ordenamiento del controlador base
            var response = await base.GetEntities(query, page, pageSize, filters, sortBy, sortDirection);

            return response;
        }

        protected override bool EntityExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        protected override int GetEntityId(Product entity)
        {
            return entity.Id;
        }

        // Puedes sobrescribir métodos o agregar otros específicos si es necesario
    }
}
