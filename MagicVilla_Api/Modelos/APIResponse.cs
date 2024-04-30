using System.Net;

namespace MagicVilla_Api.Modelos
{
    public class APIResponse
    {
        // Propiedad código de estado
        public HttpStatusCode statusCode {  get; set; }
        public bool IsExitoso { get; set; } = true;
        // Lista de todos los errores
        public List<string> ErrorsMessages { get; set; }
        // Almacenar cualquier tipo de objeto
        public object Resultado { get; set; }   
    }
}
