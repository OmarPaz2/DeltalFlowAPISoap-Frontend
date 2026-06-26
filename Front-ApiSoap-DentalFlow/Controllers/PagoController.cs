using Front_ApiSoap_DentalFlow.Models.Pago;
using Microsoft.AspNetCore.Mvc;
using moduloPago;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class PagoController : Controller
    {
        private readonly PagoEndpoint _pagoService;

        public PagoController(PagoEndpoint pagoService)
        {
            _pagoService = pagoService;
        }

        private void AddSoapAuth()
        {
            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token)) return;

            var httpRequest = new HttpRequestMessageProperty();
            httpRequest.Headers["Authorization"] = $"Bearer {token}";

            OperationContext.Current.OutgoingMessageProperties[
                HttpRequestMessageProperty.Name
            ] = httpRequest;
        }

        private IDisposable CreateScope()
        {
            var channel = (IClientChannel)((ClientBase<PagoEndpoint>)_pagoService).InnerChannel;
            return new OperationContextScope(channel);
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
                using (CreateScope())
                {
                    AddSoapAuth();

                    var pago = new registerPagoCitaRequest
                    {
                        idCita = model.IdReferencia,
                        pago = new pagoRequestDto
                        {
                            metodoPago = model.MetodoPago,
                            monto = model.Monto,
                            montoSpecified = true
                        }
                    };

                    await _pagoService.registerPagoCitaAsync(pago);
                }

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
                using (CreateScope())
                {
                    AddSoapAuth();

                    var pago = new registerPagoTratamientoRequest
                    {
                        idTratamiento = model.IdReferencia,
                        pago = new pagoRequestDto
                        {
                            metodoPago = model.MetodoPago,
                            monto = model.Monto,
                            montoSpecified = true
                        }
                    };

                    await _pagoService.registerPagoTratamientoAsync(pago);
                }

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
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _pagoService.findPagoByIdAsync(
                        new findPagoByIdRequest { idPago = idPago }
                    );

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
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar pago: " + ex.Message;
                return View();
            }
        }
    }
}