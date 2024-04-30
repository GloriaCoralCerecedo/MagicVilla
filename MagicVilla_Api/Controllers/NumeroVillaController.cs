using AutoMapper;
using MagicVilla_Api.Datos;
using MagicVilla_Api.Modelos;
using MagicVilla_Api.Modelos.Dto;
using MagicVilla_Api.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class NumeroVillaController : ControllerBase
    {
        // Logger
        private readonly ILogger<NumeroVillaController> _logger;
        ////DbContext
        //private readonly ApplicationDbContext _db;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo;
        // Auto Mapper
        private readonly IMapper _mapper;
        protected APIResponse _response;
        // Constructor
        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo, INumeroVillaRepositorio numeroRepo, IMapper mapper)
        {
            _logger = logger;
           _villaRepo = villaRepo;
            _numeroRepo = numeroRepo;
            //_db = db;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener Números villas");
                IEnumerable<NumeroVilla> numerovillaList = await _numeroRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numerovillaList);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {

                _response.IsExitoso = false;
                _response.ErrorsMessages =  new List<string>() { ex.ToString() };   
            }
            return _response;
        }
       
        [HttpGet("id:int", Name = "GetNumeroVilla")] // Tipo de dato integer, nombre de la ruta
        // Retorna un solo objeto en base al id

        // Documentar todos los códigos de estado
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer el Número Villa con id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso=false;
                    return BadRequest(_response); // Mala solicitud, status 400
                }
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);
                if (numeroVilla == null)
                {
                    _response.statusCode=HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response); // No encontro registro relacionado con el id, status 404
                }
                _response.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
                _response.statusCode = HttpStatusCode.OK;
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
         public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
        {
            try
            {
                // Si el modelo no es valido
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _numeroRepo.Obtener(v => v.VillaNo == createDto.VillaNo) != null)
                {
                    // "Nombre de la validación", "Mensaje que quiero mostrar"
                    ModelState.AddModelError("NombreExiste", "El número villa ya existe");
                    return BadRequest(ModelState);
                }
                // Que el id exista en la base de datos
                if (await _villaRepo.Obtener(v=>v.Id==createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El Id de la  villa NO existe");
                    return BadRequest(ModelState);
                }
                if (createDto == null)
                {
                    return BadRequest(); // 400
                }
                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;   
                await _numeroRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response); // 201
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
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);  // 400
                }
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);
                if (numeroVilla == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response); // 404
                }
                await _numeroRepo.Remover(numeroVilla);
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
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto )
        {
            if (updateDto == null || id!= updateDto.VillaNo)
            {
                _response.IsExitoso=false;
                _response.statusCode = HttpStatusCode.BadRequest;   
                return BadRequest(_response); // 400
            }
            if (await _villaRepo.Obtener(v => v.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ClaveForanea", "El Id de la  villa NO existe");
                return BadRequest(ModelState);
            }
            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);   
            await _numeroRepo.Actualizar(modelo);
            _response.statusCode=HttpStatusCode.NoContent;
            return Ok(_response);
        }

    }
}
