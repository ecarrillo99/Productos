using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;

namespace ProductsApiRest.Controllers
{
    [Route("api/[controller]")] //Definir rutas, en este caso se usa el nombre del controlador
    [ApiController]
    public class ProductsController(ApplicationDbContext context) : BaseController
    {
        private readonly ApplicationDbContext _context = context; //Crear instancia del contexto de la BD

        //obtener lista de productos GET: api/Products
        /*[HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                if (products.Count > 0) //Se verifica que la lista de productos no esté vacia
                {
                    var data = new ListData<List<Product>>
                    {
                        Items = products.ToList(),
                        TotalItems = products.Count,
                    };
                    return Ok(ApiResponse<ListData<List<Product>>>.Ok(data,"Se ha listado correctamente", 0)); //Mensaje de OK cuando se tienen Datos;
                }
                else
                {
                    return NotFound(ApiResponse<List<Product>>.Error("No existen productos", 1)); //Mensaje cuando no existen datos
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Product>>.Error("Ha ocurrido un error al listar: {ex.Message}", 2)); //Mensaje de error al suceder excepciones
            }
        }*/

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ListData<List<Product>>>>> GetProducts(
            int page = 1,
            int pageSize = 10,
            [FromQuery] Dictionary<string, string> filters = null,
            string sortBy = null,
            string sortDirection = "asc")
        {
            var productsQuery = _context.Products.AsQueryable();
            return await GetEntities(productsQuery, page, pageSize, filters, sortBy, sortDirection);
        }

        //Obtener un producto por su id GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProducto(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null) //Se verifica que exista el producto
                {
                    return NotFound(ApiResponse<Product>.Error("No existe producto con el id especificado", 1)); //Respuesta si el producto con el id ingresado no existe
                }
                return Ok(ApiResponse<Product>.Ok(product, "Producto obtenido correctamente", 0)); //Respuesta si el producto con el id ingresado existe
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Product>.Error($"Ha ocurrido un error al obtener producto: {ex.Message}", 2));  //Respuesta para errores
            }
        }

        //Agregar un nuevo producto POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Product>>> PostProducto(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                var response = ApiResponse<Product>.Ok(product, "Se ha agregado el producto correctamente", 0);

                return CreatedAtAction(nameof(GetProducto), new { id = product.Id }, response); //Retorna respuesta junto al producto que se añadió
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Product>.Error($"No fue posible agregar el producto: {ex.Message}", 2));  //Respuesta para errores
            }
        }

        //Actualizar producto por ID PUT: api/Productos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> PutProducto(int id, Product product)
        {
            // Verifica si el producto existe antes de intentar actualizar
            if (!ProductExists(id))
            {
                return NotFound(ApiResponse<Product>.Error("El producto a modificar no existe", 1));
            }

            if (id != product.Id)// Verifica si el ID proporcionado coincide con el del producto
            {
                return BadRequest(ApiResponse<Product>.Error("El ID del producto no coincide con el ID proporcionado", 1));
            }

            try
            {
                // Marca el producto como modificado
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<Product>.Ok(product, "Producto actualizado correctamente", 0)); //Mensaje de actualizacion correcta
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Product>.Error($"Ha ocurrido un error al actualizar producto: {ex.Message}", 2)); //Mensaje para excepciones
            }
        }

        //Eliminar producto por ID DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> DeleteProducto(int id)
        {
            try
            {
                if (!ProductExists(id)) // Verifica si el producto existe 
                {
                    return NotFound(ApiResponse<Product>.Error("El producto a eliminar no existe", 1));
                }

                var product = new Product { Id = id }; //Crear instancia de producto con el id a eliminar

                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<int>.Ok(id, "Producto eliminado correctamente", 0)); //Mensaje de eliminación correcta
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.Error($"Ha ocurrido un error al eliminar el producto: {ex.Message}", 2)); //Respuesta para excepciones
            }
        }

        //Función para verificar si un producto existe
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}