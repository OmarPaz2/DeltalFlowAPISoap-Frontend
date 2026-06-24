using Front_ApiSoap_DentalFlow.Models.Pago;
using Microsoft.AspNetCore.Mvc;
using moduloPago;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class PagoController : Controller
    {
        private readonly PagoServiceImplClient _pagoService;

        public PagoController(PagoServiceImplClient pagoService)
        {
            _pagoService = pagoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePagoCita()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePagoCita(PagoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var pago = new pagoRequestDto
                {
                    metodoPago = model.MetodoPago,
                    monto = model.Monto,
                    montoSpecified = true
                };

                await _pagoService.registerPagoCitaAsync(pago, model.IdReferencia);

                TempData["Success"] = "Pago de cita registrado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar pago de cita: " + ex.Message;
                return View(model);
            }
        }

        public IActionResult CreatePagoTratamiento()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePagoTratamiento(PagoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var pago = new pagoRequestDto
                {
                    metodoPago = model.MetodoPago,
                    monto = model.Monto,
                    montoSpecified = true
                };

                await _pagoService.registerPagoTratamientoAsync(pago, model.IdReferencia);

                TempData["Success"] = "Pago de tratamiento registrado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar pago de tratamiento: " + ex.Message;
                return View(model);
            }
        }

        public IActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Details(int idPago)
        {
            try
            {
                var response = await _pagoService.findPagoByIdAsync(idPago);
                var p = response.@return;

                var model = new PagoViewModel
                {
                    IdPago = p.idPago,
                    Razon = p.razon,
                    NombresPaciente = p.nombresPaciente,
                    ApellidosPaciente = p.apellidosPaciente,
                    NombreEspecialidad = p.nombreEspecialidad,
                    Monto = p.monto,
                    Fecha = p.fecha,
                    MetodoPago = p.metodoPago
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar pago: " + ex.Message;
                return View();
            }
        }
    }
}
