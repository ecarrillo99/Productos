using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;
using ProductsApiRest.Models;

namespace ProductsApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        //obtener lista de productos GET: api/Products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                if (products.Count > 0) //Se verifica que la lista de productos no esté vacia
                {
                    return Ok(new ApiResponse<List<Product>> //Mensaje de OK cuando se tienen Datos
                    {
                        Code = 0,
                        Status = true,
                        Message = "Se ha listado correctamente",
                        Data = products
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

        //Obtener un producto por su id GET: api/Productos/{id}
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
    }
}
