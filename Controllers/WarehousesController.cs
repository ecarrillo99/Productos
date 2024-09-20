using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;

namespace ProductsApiRest.Controllers
{
    public class WarehouseController : BaseCrudController<Warehouse>
    {
        public WarehouseController(ApplicationDbContext context) : base(context) { }

        protected override bool EntityExists(int id)
        {
            return _context.Warehouse.Any(e => e.Id == id);
        }

        protected override int GetEntityId(Warehouse entity)
        {
            return entity.Id;
        }

    }
}
