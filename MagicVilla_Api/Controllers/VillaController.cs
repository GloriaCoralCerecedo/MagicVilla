using AutoMapper;
using MagicVilla_Api.Datos;
using MagicVilla_Api.Modelos;
using MagicVilla_Api.Modelos.Dto;
using MagicVilla_Api.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Net;

// Ver errores en el navegador https://localhost:7201/api/Villa

namespace MagicVilla_Api.Controllers
{

    [Route("api/[controller]")] // api/Villa
    [ApiController]
    public class VillaController : ControllerBase
    {
        // Logger
        private readonly ILogger<VillaController> _logger;
        ////DbContext
        //private readonly ApplicationDbContext _db;
        private readonly IVillaRepositorio _villaRepo;
        // Auto Mapper
        private readonly IMapper _mapper;
        protected APIResponse _response;
        // Constructor
        //public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
           _villaRepo = villaRepo;
            //_db = db;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        // IEnumerable porque retorna una Lista, de tipo Villa o VillaDto (Modelo)
        // ActionResult, podemos utilizar cualquier tipo de retorno un objeto(status), como por ejemplo ok
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas() 
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");
                //IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
                //return Ok(VillaStore.villaList); // status 200 
            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorsMessages =  new List<string>() { ex.ToString() };   
            }
            return _response;
        }
       
        [HttpGet("id:int", Name = "GetVilla")] // Tipo de dato integer, nombre de la ruta
        // Retorna un solo objeto en base al id

        // Documentar todos los códigos de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<VillaDto>> GetVilla(int id)
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso=false;
                    return BadRequest(_response); // Mala solicitud, status 400
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                //var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);
                if (villa == null)
                {
                    _response.statusCode=HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response); // No encontro registro relacionado con el id, status 404
                }
                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response.statusCode = HttpStatusCode.OK;
                // Retorna un objeto de villaDto, se obtiene los datos de villa
                return Ok(_response);  // status 200 
            }
            catch (Exception ex)
            {

                _response.IsExitoso=false;  
                _response.ErrorsMessages = new List<string> { ex.ToString() };  
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Cuando se crea un nuevo registro
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Error interno, ejemplo con ingresar un tipo de dato diferente
        // FromBody, indica que va a recibir datos dependiendo el modelo
        //public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
         public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                // Si el modelo no es valido
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // Model State Personalizado
                // Si el primer registro que encuentre es igual a lo que estoy recibiendo, si es diferente a null
                // if (VillaStore.villaList.FirstOrDefault(v=>v.Nombre.ToLower() == villaDto.Nombre.ToLower()) !=null) 
                //if(await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    // "Nombre de la validación", "Mensaje que quiero mostrar"
                    ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                    return BadRequest(ModelState);
                }
                if (createDto == null)
                {
                    return BadRequest(); // 400
                }
                //if (villaDto.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError); // 500
                //}
                Villa modelo = _mapper.Map<Villa>(createDto);
                //Villa modelo = new()
                //{
                //    Nombre = villaDto.Nombre,
                //    Detalle = villaDto.Detalle,
                //    ImagenUrl = villaDto.ImagenUrl,
                //    Ocupantes = villaDto.Ocupantes,
                //    Tarifa = villaDto.Tarifa,
                //    MetrosCuadrados = villaDto.MetrosCuadrados,
                //    Amenidad = villaDto.Amenidad,
                //};
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;   
                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                //await _db.Villas.AddAsync(modelo);
                //await _db.SaveChangesAsync();
                //villaDto.Id=VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
                //VillaStore.villaList.Add(villaDto);

                //// Correcto
                //return Ok(villaDto); // 200

                // Cuando en una api se crea un nuevo recurso, se debe indicar la url del recurso creado.
                // Se debe utilizar el enpoint get, el que retorna un solo registro

                // Se manda el nombre de la ruta get, el id, y todo el modelo
                // Id del modelo, modelo nuevo
                //return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo); // 201
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response); // 201
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // IActionResult, porque no requerimos el modelo
        // Se retorna un NoContent
        //public async Task<IActionResult> DeleteVilla(int id) 
        // Las interfaces no pueden llevar un tipo 
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);  // 400
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);
                //var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);
                if (villa == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response); // 404
                }
                //VillaStore.villaList.Remove(villa);
                // No es un metodo asincrono remove
                await _villaRepo.Remover(villa);
                //_db.Villas.Remove(villa);
                //await _db.SaveChangesAsync();
                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response); // 204
            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorsMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        // Actualiza todo el modelo 
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto )
        {
            if (updateDto == null || id!= updateDto.Id)
            {
                _response.IsExitoso=false;
                _response.statusCode = HttpStatusCode.BadRequest;   
                return BadRequest(_response); // 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            Villa modelo = _mapper.Map<Villa>(updateDto);   
            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad,
            //};
            // No es un metodo asincrono update
            await _villaRepo.Actualizar(modelo);
            _response.statusCode=HttpStatusCode.NoContent;
            //_db.Villas.Update(modelo);
            //await _db.SaveChangesAsync();
            return Ok(_response);
        }

        // Actualizar solo una propiedad del modelo
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest(); // 400
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            // Registro actual
            // AsNoTracking(), nos permite consultar un registro, ya sea del DbContext, pero que no se trackee
            // y que no de el problema de que quede abierto
            //var villa =await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false);
            // Antes de que se actualice el registro, se pondra temporalmente en un modelo VillaDto
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);
            //VillaUpdateDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad,
            //};
            if (villa == null) return BadRequest();
            // Id y Modelo
            patchDto.ApplyTo(villaDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Mismas propiedad del villaDto, pero luego de pasar el pactchDto.ApplyTo, que ya contiene lo unico que se va a modificar
            // Se lo devielve al modelo del tipo villa
            // Este modelo se envia al metodo DbContext update
            Villa modelo = _mapper.Map<Villa>(villaDto);
            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad,
            //};
            // No es un metodo asincrono update
            await _villaRepo.Actualizar(modelo);
            _response.statusCode =HttpStatusCode.NoContent; 
            //_db.Villas.Update(modelo);
            //await _db.SaveChangesAsync();

            return Ok(_response);
        }

    }
}
