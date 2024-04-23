using MagicVilla_Api.Datos;
using MagicVilla_Api.Modelos;
using MagicVilla_Api.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// Ver errores en el navegador https://localhost:7201/api/Villa

namespace MagicVilla_Api.Controllers
{

    [Route("api/[controller]")] // api/Villa
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        //DbContext
        private readonly ApplicationDbContext _db;
        // Constructor
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        // IEnumerable porque retorna una Lista, de tipo Villa o VillaDto (Modelo)
        // ActionResult, podemos utilizar cualquier tipo de retorno un objeto(status), como por ejemplo ok
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas() 
        {
            _logger.LogInformation("Obtener las villas");
            return Ok(_db.Villas.ToList());
            //return Ok(VillaStore.villaList); // status 200 
        }
       
        [HttpGet("id:int", Name = "GetVilla")] // Tipo de dato integer, nombre de la ruta
        // Retorna un solo objeto en base al id

        // Documentar todos los códigos de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con id " + id);
                return BadRequest(); // Mala solicitud, status 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return NotFound(); // No encontro registro relacionado con el id, status 404
            }
            return Ok(villa);  // status 200 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Cuando se crea un nuevo registro
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error interno, ejemplo con ingresar un tipo de dato diferente
        // FromBody, indica que va a recibir datos dependiendo el modelo
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {
            // Si el modelo no es valido
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Model State Personalizado
            // Si el primer registro que encuentre es igual a lo que estoy recibiendo, si es diferente a null
           // if (VillaStore.villaList.FirstOrDefault(v=>v.Nombre.ToLower() == villaDto.Nombre.ToLower()) !=null) 
           if(_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                // "Nombre de la validación", "Mensaje que quiero mostrar"
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }
            if(villaDto == null)
            {
                return BadRequest(); // 400
            }
            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // 500
            }
            Villa modelo = new()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };

            _db.Villas.Add(modelo);
            _db.SaveChanges();
            //villaDto.Id=VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto);

            //// Correcto
            //return Ok(villaDto); // 200

            // Cuando en una api se crea un nuevo recurso, se debe indicar la url del recurso creado.
            // Se debe utilizar el enpoint get, el que retorna un solo registro

            // Se manda el nombre de la ruta get, el id, y todo el modelo
            return CreatedAtRoute("GetVilla", new {id=villaDto.Id}, villaDto); // 201
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // IActionResult, porque no requerimos el modelo
        // Se retorna un NoContent
        public IActionResult DeleteVilla(int id) 
        {
            if (id == 0)
            {
                return BadRequest();  // 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);
            var villa =_db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound(); // 404
            }
            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent(); // 204
        }

        // Actualiza todo el modelo 
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto )
        {
            if (villaDto == null || id!= villaDto.Id)
            {
                return BadRequest(); // 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();
            return NoContent();
        }

        // Actualizar solo una propiedad del modelo
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> pactchDto)
        {
            if (pactchDto == null || id == 0)
            {
                return BadRequest(); // 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            // Registro actual
            // AsNoTracking(), nos permite consultar un registro, ya sea del DbContext, pero que no se trackee
            // y que no de el problema de que quede abierto
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            // Antes de que se actualice el registro, se pondra temporalmente en un modelo VillaDto
            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad,
            };
            if (villa == null) return BadRequest();
            // Id y Modelo
            pactchDto.ApplyTo(villaDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Mismas propiedad del villaDto, pero luego de pasar el pactchDto.ApplyTo, que ya contiene lo unico que se va a modificar
            // Se lo devielve al modelo del tipo villa
            // Este modelo se envia al metodo DbContext update
            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad,
            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
