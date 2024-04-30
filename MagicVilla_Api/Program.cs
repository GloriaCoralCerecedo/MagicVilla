using MagicVilla_Api;
using MagicVilla_Api.Datos;
using MagicVilla_Api.Repositorio;
using MagicVilla_Api.Repositorio.IRepositorio;
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

// Servicio de interfaz con implementación, se agrega para inyectar en el controlador
// AddScoped - son servicios con alcance que se crean una vez por solicitud y luego se destruyen 
// AddSingleton - son servicios que se crean la primera vez que se solicitan y luego cada solicitud
// posterior utilizara la misma instancia
// AddTransient - son servicios transitorios, se crean cada vez que se solicitan, esta vida se utiliza mejor para servicios
// livianos y sin estado
builder.Services.AddScoped<IVillaRepositorio, VillaRepositorio>();
builder.Services.AddScoped<INumeroVillaRepositorio, NumeroVillaRepositorio>();

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
