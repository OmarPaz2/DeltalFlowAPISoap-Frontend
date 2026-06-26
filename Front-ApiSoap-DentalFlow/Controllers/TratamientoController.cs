using Front_ApiSoap_DentalFlow.Models.Tratamiento;
using Microsoft.AspNetCore.Mvc;
using servicioTratamiento;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class TratamientoController : Controller
    {
        private readonly TratamientoEndpoint _tratamientoService;

        public TratamientoController(TratamientoEndpoint tratamientoService)
        {
            _tratamientoService = tratamientoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TratamientoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var rq = new registrarTratamientoRequest
                {
                   request = new tratamientoRequestDto
                   {
                      
                       pacienteId = model.PacienteId,
                       odontologoId = model.OdontologoId,
                       diagnostico = model.Diagnostico,
                       tipoTratamiento = model.TipoTratamiento,
                       costoEstimado = model.CostoEstimado,
                       fechaInicio = model.FechaInicio.ToString("yyyy-MM-dd"),
                       cant_sesiones = model.CantSesiones
                       
                   }
                };

                var response = await _tratamientoService.registrarTratamientoAsync(rq);

                TempData["Success"] = response.@return ?? "Tratamiento registrado correctamente";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar tratamiento: " + ex.Message;
                return View(model);
            }
        }

        public IActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var response = await _tratamientoService.getTratamientoAsync(new getTratamientoRequest(id));
                var t = response.@return;

                var model = new TratamientoViewModel
                {
                    IdTratamiento = t.idTratamiento,
                    NombresPaciente = t.nombresPaciente,
                    ApellidosPaciente = t.apellidosPaciente,
                    Dni = t.dni,
                    NombresOdontologo = t.nombresOdontologo,
                    ApellidosOdontologo = t.apellidosOdontologo,
                    Diagnostico = t.diagnostico,
                    TipoTratamiento = t.tipoTratamiento,
                    CostoEstimado = t.costoEstimado,
                    FechaInicio = DateTime.TryParse(t.fechaInicio, out var f) ? f : DateTime.Now,
                    CantSesiones = t.cant_sesiones,
                    SesionesRestantes = t.sesionesRestantes,
                    MontoPagado = t.montoPagado,
                    Estado = t.estado,
                    Pagado = t.pagado
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar tratamiento: " + ex.Message;
                return View();
            }
        }

        public IActionResult EditEstado()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEstado(int idTratamiento, string estado)
        {
            try
            {
                await _tratamientoService.actualizarEstadoAsync(new actualizarEstadoRequest(idTratamiento, estado));

                TempData["Success"] = "Estado actualizado correctamente";
                return RedirectToAction(nameof(Details));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar estado: " + ex.Message;
                return View();
            }
        }
    }
}
