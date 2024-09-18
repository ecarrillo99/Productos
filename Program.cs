using Microsoft.EntityFrameworkCore;
using ProductsApiRest.Data;

var builder = WebApplication.CreateBuilder(args);

// Enviar la cadena de conexión a la clase del contexto de la BD
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();