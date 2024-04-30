using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Modelos.Dto
{
    public class NumeroVillaUpdateDto
    {
        [Required]
        public int VillaNo { get; set; }
        // Relación con la tabla villa
        [Required]
        public int VillaId { get; set; }
        public string DetalleEspecial { get; set; }
    }
}
