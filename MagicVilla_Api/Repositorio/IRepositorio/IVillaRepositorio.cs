using MagicVilla_Api.Modelos;

namespace MagicVilla_Api.Repositorio.IRepositorio
{
    public interface IVillaRepositorio : IRepositorio<Villa>
    {
        Task<Villa> Actualizar(Villa entidad);
    }
}
