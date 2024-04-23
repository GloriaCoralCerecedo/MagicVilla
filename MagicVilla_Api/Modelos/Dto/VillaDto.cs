using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Modelos.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }
        // Data Annotation
        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public double MetrosCuadrados { get; set; }
        public string ImagenUrl { get; set; }
        public string Amenidad { get; set; }
    }
}
