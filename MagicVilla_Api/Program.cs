using MagicVilla_Api;
using MagicVilla_Api.Datos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Servicio, relación de la clase DbContext, con la cadena de conexión y con el motor de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Servicio para usar el mapper en la inyección de dependencias
builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
