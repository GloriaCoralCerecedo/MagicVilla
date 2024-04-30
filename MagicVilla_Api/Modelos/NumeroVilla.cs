using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_Api.Modelos
{
    //Relación de uno a muchos, donde villa sera el padre y NumeroVilla todo los numeros de villa que tendra la villa como tal 
    public class NumeroVilla
    {
        // Llave primaria, donde se ingresa manualmente
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        // Relación con la tabla villa
        [Required]
        public int VillaId { get; set; }
        // Relación con Villa utilizando el VillaId
        [ForeignKey("VillaId")]   
        public Villa Villa { get; set; }
        public string DetalleEspecial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
