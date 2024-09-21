using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BaseFilterController : ControllerBase
{
    public async Task<ActionResult<ApiResponse<ListData<List<T>>>>> GetEntities<T>(
    IQueryable<T> query,
    int page = 1,
    int pageSize = 10,
    Dictionary<string, string> filters = null,
    string sortBy = "Id",
    string sortDirection = "asc"
)
    {
        try
        {
            if (filters != null)
            {
                foreach (var filter in filters) //Recorre los filtros que llegan en la url
                {
                    var propertyInfo = typeof(T).GetProperty(filter.Key);
                    if (propertyInfo != null) //Comprueba las propiedades del elemento
                    {
                        if (propertyInfo.PropertyType == typeof(string)) //Acción si es string
                        {
                            query = query.Where(e => EF.Property<string>(e, filter.Key).ToLower().Contains(filter.Value.ToLower())); //realición de filtrado por contiene
                        }
                        else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(int)) //Acción si es número
                        {
                            var numberRange = filter.Value.Split(','); //Comprueba si se envia un rango (Ejm: Stock=1,2)

                            if (numberRange.Length == 1) //Acción si no hay rango
                            {
                                if (propertyInfo.PropertyType == typeof(double) && double.TryParse(numberRange[0], out double singleValue)) //Acción si el valor es double
                                {
                                    query = query.Where(e => EF.Property<double>(e, filter.Key) >= singleValue); //Obtiene los valores iguales o mayores
                                }
                                else if (propertyInfo.PropertyType == typeof(int) && int.TryParse(numberRange[0], out int singleIntValue)) //Acción si el valor es int
                                {
                                    query = query.Where(e => EF.Property<int>(e, filter.Key) >= singleIntValue); //Obtiene los valores iguales o mayores
                                }
                            }
                            else if (numberRange.Length == 2) //Acción si existe un rango
                            {
                                if (propertyInfo.PropertyType == typeof(double) &&
                                    double.TryParse(numberRange[0], out double minValue) &&
                                    double.TryParse(numberRange[1], out double maxValue))
                                {
                                    query = query.Where(e => EF.Property<double>(e, filter.Key) >= minValue && EF.Property<double>(e, filter.Key) <= maxValue); //Obtiene los valores entre el rango para double
                                }
                                else if (propertyInfo.PropertyType == typeof(int) &&
                                         int.TryParse(numberRange[0], out int minIntValue) &&
                                         int.TryParse(numberRange[1], out int maxIntValue))
                                {
                                    query = query.Where(e => EF.Property<int>(e, filter.Key) >= minIntValue && EF.Property<int>(e, filter.Key) <= maxIntValue); //Obtiene los valores entre el rango para int
                                }
                            }
                        }

                        else if (propertyInfo.PropertyType == typeof(DateTime)) //Acciones para fechas
                        {
                            var dateRange = filter.Value.Split(','); //Comprueba si hay un rangp de fechas (Ejm. Dates=2024-09-01, 2024-09-10)
                            if (dateRange.Length == 1 && DateTime.TryParse(dateRange[0], out DateTime singleDate))
                            {
                                // Convertir a UTC
                                query = query.Where(e => EF.Property<DateTime>(e, filter.Key) <= singleDate.ToUniversalTime()); //Obtiene los elementos con fechas menores
                            }
                            else if (dateRange.Length == 2 &&
                                     DateTime.TryParse(dateRange[0], out DateTime startDate) &&
                                     DateTime.TryParse(dateRange[1], out DateTime endDate))
                            {
                                // Convertir a UTC
                                query = query.Where(e => EF.Property<DateTime>(e, filter.Key) >= startDate.ToUniversalTime() && EF.Property<DateTime>(e, filter.Key) <= endDate.ToUniversalTime()); //Obtiene elementos entre las fechas
                            }
                        }

                    }
                }
            }

            // Aplicar ordenamiento
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortDirection.ToLower() == "desc")
                {
                    query = query.OrderByDescending(e => EF.Property<object>(e, sortBy)); //Ordenda de manera decendente 
                }
                else
                {
                    query = query.OrderBy(e => EF.Property<object>(e, sortBy)); //Ordena ascendente
                }
            }

            var totalItems = await query.CountAsync(); //Obtiene el total de elementos
            var entities = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(); //Obtiene los elementos

            var data = new ListData<List<T>>
            {
                Items = entities,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return Ok(ApiResponse<ListData<List<T>>>.Ok(data, "Se ha listado correctamente", 0)); //Respuesta exitosa
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<T>>.Error($"Ha ocurrido un error al listar: {ex.Message}", 2)); //Respuesta en caso de error
        }
    }

}