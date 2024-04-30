using MagicVilla_Api.Modelos;

namespace MagicVilla_Api.Repositorio.IRepositorio
{
    public interface INumeroVillaRepositorio : IRepositorio<NumeroVilla>
    {
        Task<NumeroVilla> Actualizar(NumeroVilla entidad);
    }
}
