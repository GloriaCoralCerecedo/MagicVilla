using MagicVilla_Api.Datos;
using MagicVilla_Api.Modelos;
using MagicVilla_Api.Repositorio.IRepositorio;

namespace MagicVilla_Api.Repositorio
{
    public class NumeroVillaRepositorio: Repositorio<NumeroVilla>, INumeroVillaRepositorio
    {
        private readonly ApplicationDbContext _db;

        // Le pasamos el dbContext al padre (Repositorio)
        public NumeroVillaRepositorio(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }

        public async Task<NumeroVilla> Actualizar(NumeroVilla entidad)
        {
            entidad.FechaActualizacion = DateTime.Now;
            _db.NumeroVillas.Update(entidad);
            await _db.SaveChangesAsync();
            return entidad; 
        }
    }
}
