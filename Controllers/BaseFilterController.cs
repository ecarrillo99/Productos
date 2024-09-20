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
        string sortBy = null,
        string sortDirection = "asc" // "asc" por defecto
    ) 
    {
        try
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var propertyInfo = typeof(T).GetProperty(filter.Key);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            query = query.Where(e => EF.Property<string>(e, filter.Key).Contains(filter.Value));
                        }
                        else if (propertyInfo.PropertyType == typeof(double))
                        {
                            if (double.TryParse(filter.Value, out double result))
                            {
                                query = query.Where(e => EF.Property<double>(e, filter.Key) == result);
                            }
                        }
                        else if (propertyInfo.PropertyType == typeof(DateTime))
                        {
                            var dateRange = filter.Value.Split(',');
                            if (dateRange.Length == 2 &&
                                DateTime.TryParse(dateRange[0], out DateTime startDate) &&
                                DateTime.TryParse(dateRange[1], out DateTime endDate))
                            {
                                query = query.Where(e => EF.Property<DateTime>(e, filter.Key) >= startDate && EF.Property<DateTime>(e, filter.Key) <= endDate);
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
                    query = query.OrderByDescending(e => EF.Property<object>(e, sortBy));
                }
                else
                {
                    query = query.OrderBy(e => EF.Property<object>(e, sortBy));
                }
            }

            var totalItems = await query.CountAsync();
            var entities = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var data = new ListData<List<T>>
            {
                Items = entities,
                TotalItems = totalItems,
                Page=page,
                PageSize=pageSize
            };

            return Ok(ApiResponse<ListData<List<T>>>.Ok(data, "Se ha listado correctamente", 0));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<T>>.Error($"Ha ocurrido un error al listar: {ex.Message}", 2));
        }
    }
}