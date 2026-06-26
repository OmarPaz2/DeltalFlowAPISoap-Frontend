using Front_ApiSoap_DentalFlow.Models.Appointment;
using Microsoft.AspNetCore.Mvc;
using moduloTipoCita;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class AppointmentTypeController : Controller
    {
        private readonly AppointmentTypeEndpoint _appointmentTypeService;

        public AppointmentTypeController(AppointmentTypeEndpoint appointmentTypeService)
        {
            _appointmentTypeService = appointmentTypeService;
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
            var channel = (IClientChannel)((ClientBase<AppointmentTypeEndpoint>)_appointmentTypeService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _appointmentTypeService.getAllAppointmentTypesAsync(
                        new getAllAppointmentTypesRequest()
                    );

                    if (response == null || response.@return == null)
                    {
                        return View(new List<AppointmentTypeVM>());
                    }

                    List<AppointmentTypeVM> tipos = response.@return.Select(x => new AppointmentTypeVM
                    {
                        id = x.id,
                        minutos = x.durationMinutes,
                        name = x.name,
                        price = x.price
                    }).ToList();

                    return View(tipos);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener los tipos de citas. Intente nuevamente mas tarde";
                return View(new List<AppointmentTypeVM>());
            }
        }

        public ActionResult Create()
        {
            return PartialView(new AppointmentTypeVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AppointmentTypeVM rq)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var request = new createAppointmentTypeRequest
                    {
                        name = rq.name,
                        durationMinutes = rq.minutos,
                        price = rq.price
                    };

                    await _appointmentTypeService.createAppointmentTypeAsync(request);
                }

                TempData["MensajeSuccess"] = "Tipo de cita creada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (FaultException fal)
            {
                TempData["Error"] = fal.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el tipo de cita. Intente nuevamente mas tarde";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var tipo = await _appointmentTypeService.getAllAppointmentTypesAsync(
                        new getAllAppointmentTypesRequest()
                    );

                    var seleccion = tipo.@return.FirstOrDefault(x => x.id == id);

                    if (seleccion == null)
                    {
                        TempData["Error"] = "Tipo de cita no encontrado";
                        return RedirectToAction(nameof(Index));
                    }

                    var vm = new AppointmentTypeVM
                    {
                        id = seleccion.id,
                        name = seleccion.name,
                        minutos = seleccion.durationMinutes,
                        price = seleccion.price
                    };

                    return PartialView(vm);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar el tipo de cita";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AppointmentTypeVM model)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var parametros = new updateTypeRequest
                    {
                        arg0 = model.id,
                        arg1 = new appointmentType
                        {
                            name = model.name,
                            durationMinutes = model.minutos,
                            price = model.price,
                            durationMinutesSpecified = true
                        }
                    };

                    var resultado = await _appointmentTypeService.updateTypeAsync(parametros);

                    TempData["Success"] = resultado.@return;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FaultException soapEx)
            {
                TempData["Error"] = soapEx.Reason.ToString();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el tipo de cita";
                return View(model);
            }
        }
    }
}