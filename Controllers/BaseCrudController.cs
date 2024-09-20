using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApiRest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseCrudController<TEntity> : BaseFilterController where TEntity : class
    {
        protected readonly ApplicationDbContext _context;

        public BaseCrudController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener lista de elementos con paginación, filtros y ordenación
        [HttpGet]
        public virtual async Task<ActionResult<ApiResponse<ListData<List<TEntity>>>>> GetEntities(
            int page = 1,
            int pageSize = 10,
            [FromQuery] Dictionary<string, string> filters = null,
            string sortBy = null,
            string sortDirection = "asc")
        {
            var query = _context.Set<TEntity>().AsQueryable();

            // Llamar al método de filtrado y ordenamiento del controlador filtro base
            var response = await GetEntities(query, page, pageSize, filters, sortBy, sortDirection);

            return response;
        }

        // Obtener un elemento por su ID
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> GetEntity(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return NotFound(ApiResponse<TEntity>.Error("No existe el elemento con el id especificado", 1));
            }

            return Ok(ApiResponse<TEntity>.Ok(entity, "Elemento obtenida correctamente", 0));
        }

        // Crear una nueva entidad
        [HttpPost]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> CreateEntity(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEntity), new { id = GetEntityId(entity) }, ApiResponse<TEntity>.Ok(entity, "Elemento creada correctamente", 0));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<TEntity>.Error($"No fue posible agregar el elemento: {ex.Message}", 2));
            }
        }

        // Actualizar un elemento por su ID
        [HttpPut("{id}")]
        public virtual async Task<ActionResult<ApiResponse<TEntity>>> UpdateEntity(int id, TEntity entity)
        {
            if (!EntityExists(id))
            {
                return NotFound(ApiResponse<TEntity>.Error("El elemento a modificar no existe", 1));
            }

            if (GetEntityId(entity) != id)
            {
                return BadRequest(ApiResponse<TEntity>.Error("El ID de el elemento no coincide con el ID proporcionado", 1));
            }

            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<TEntity>.Ok(entity, "Elemento actualizado correctamente", 0));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<TEntity>.Error($"Ha ocurrido un error al actualizar el elemento: {ex.Message}", 2));
            }
        }

        // Eliminar un elemento por su ID
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<ApiResponse<int>>> DeleteEntity(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<TEntity>.Error("El elemento a eliminar no existe", 1));
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<int>.Ok(id, "Elemento eliminado correctamente", 0));
        }

        protected abstract bool EntityExists(int id);
        protected abstract int GetEntityId(TEntity entity);
    }
}
