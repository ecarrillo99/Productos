using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;

namespace ProductsApiRest.Controllers
{
    [Route("api/[controller]")] //Definir rutas, en este caso se usa el nombre del controlador
    [ApiController]
    public class ProductsController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context; //Crear instancia del contexto de la BD

        //obtener lista de productos GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                if (products.Count > 0) //Se verifica que la lista de productos no esté vacia
                {
                    return Ok(new ApiResponse<ListData<List<Product>>> //Mensaje de OK cuando se tienen Datos
                    {
                        Code = 0,
                        Status = true,
                        Message = "Se ha listado correctamente",
                        Data = new ListData<List<Product>>
                        {
                            Items = products.ToList(),
                            TotalItems = products.Count,
                        }
                    });
                }
                else
                {
                    return NotFound(new ApiResponse<List<Product>> //Mensaje cuando no existen datos
                    {
                        Code = 1,
                        Status = false,
                        Message = "No existen productos",
                    });
                }
            }
            catch (Exception ex)
            {
                //Mensaje de error al suceder excepciones
                return StatusCode(500, new ApiResponse<List<Product>>
                {
                    Code = 2,
                    Status = false,
                    Message = $"Ha ocurrido un error al listar: {ex.Message}",
                });
            }
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
                    return NotFound(new ApiResponse<Product>//Respuesta si el producto con el id ingresado no existe
                    {
                        Code = 1,
                        Status = false,
                        Message = "No existe producto con el id especificado",
                    });
                }

                return Ok(new ApiResponse<Product> //Respuesta si el producto con el id ingresado existe
                {
                    Code = 0,
                    Status = true,
                    Message = "Producto obtenido correctamente",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Product> //Respuesta para errores
                {
                    Code = 2,
                    Status = false,
                    Message = $"Ha ocurrido un error al obtener producto: {ex.Message}",
                });
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

                var response = new ApiResponse<Product> //Respuesta al guardarse correctamente
                {
                    Code = 0,
                    Status = true,
                    Message = "Se ha agregado el producto correctamente",
                    Data = product
                };

                return CreatedAtAction(nameof(GetProducto), new { id = product.Id }, response); //Retorna respuesta junto al producto que se añadió
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<Product> //Respuesta para errores
                {
                    Code = 2,
                    Status = false,
                    Message = $"No fue posible agregar el producto: {ex.Message}",
                };

                return StatusCode(500, response);
            }
        }

        //Actualizar producto por ID PUT: api/Productos/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> PutProducto(int id, Product product)
        {
            // Verifica si el producto existe antes de intentar actualizar
            if (!ProductExists(id))
            {
                return NotFound(new ApiResponse<Product>
                {
                    Code = 1,
                    Status = false,
                    Message = "El producto a modificar no existe",
                    Data = null
                });
            }

            if (id != product.Id)// Verifica si el ID proporcionado coincide con el del producto
            {
                return BadRequest(new ApiResponse<Product>
                {
                    Code = 1,
                    Status = false,
                    Message = "El ID del producto no coincide con el ID proporcionado",
                });
            }

            try
            {
                // Marca el producto como modificado
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Product> // Respuesta para actualización correcta
                {
                    Code = 0,
                    Status = true,
                    Message = "Producto actualizado correctamente",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Product> // Respuesta para errores
                {
                    Code = 2,
                    Status = false,
                    Message = $"Ha ocurrido un error al actualizar producto: {ex.Message}",
                });
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
                    return NotFound(new ApiResponse<Product>
                    {
                        Code = 1,
                        Status = false,
                        Message = "No existe el producto a eliminar",
                    });
                }

                var product = new Product { Id = id }; //Crear instancia de producto con el id a eliminar

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<int> //Respuesta para eliminación correcta
                {
                    Code = 0,
                    Status = true,
                    Message = "Producto eliminado correctamente",
                    Data=id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Product> //Resueta para errores
                {
                    Code = 2,
                    Status = false,
                    Message = $"Ha ocurrido un error al eliminar el producto: {ex.Message}",
                });
            }
        }

        //Función para verificar si un producto existe
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
