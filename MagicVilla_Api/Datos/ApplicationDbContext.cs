using MagicVilla_Api.Modelos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Datos
{
    public class ApplicationDbContext: DbContext
    {
        // La base va a indicar al padre dbcontext
        // Se le mandara toda la configuraión, por medio de inyección de dependencias del servicio
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            

        }
        // Se creara una tabla en la bd
        public DbSet<Villa> Villas { get; set; }

        public DbSet<NumeroVilla> NumeroVillas { get; set; }

        // Datos alamacenados antes de empezar a agregar nuevos registros
        //Override de un metodo que existe enla clase dbcontext
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Detalle de la villa...",
                    ImagenUrl = "",
                    Ocupantes=5,
                    MetrosCuadrados=50,
                    Tarifa=200,
                    Amenidad="",
                    FechaCreacion= DateTime.Now,
                    FechaActualizacion= DateTime.Now
                },
                new Villa()
                {
                    Id = 2,
                    Nombre = "Premium Vista a la Piscina",
                    Detalle = "Detalle de la villa...",
                    ImagenUrl = "",
                    Ocupantes = 10,
                    MetrosCuadrados = 100,
                    Tarifa = 1500,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                }
             ); 
        }
    }
}
