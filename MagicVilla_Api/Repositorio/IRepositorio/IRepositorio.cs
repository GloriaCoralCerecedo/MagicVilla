using System.Linq.Expressions;

namespace MagicVilla_Api.Repositorio.IRepositorio
{
    // Interfaz de repositorio generico
    // <T> where T : class - Generico, recibe cualquier tipo de entidad
    public interface IRepositorio<T> where T : class
    {
        Task Crear(T entidad);

        // Si no envia un filtro, devolvera toda la lista
        Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null);

        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true);

        Task Remover(T entidad);

        Task Grabar();

    }


}
