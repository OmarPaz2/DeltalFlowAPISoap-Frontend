using Front_ApiSoap_DentalFlow.Models.Appointment;
using Microsoft.AspNetCore.Mvc;
using moduloCitas;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentEndpoint _appointmentService;

        public AppointmentController(AppointmentEndpoint appointmentService)
        {
            _appointmentService = appointmentService;
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
            var channel = (IClientChannel)((ClientBase<AppointmentEndpoint>)_appointmentService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _appointmentService.getAllAppointmentsAsync(new getAllAppointmentsRequest());

                    var lista = response.@return.Select(c => new AppointmentViewModel
                    {
                        Id = c.id,

                        PatientId = c.patient != null ? c.patient.id : 0,
                        DentistId = c.dentist != null ? c.dentist.id : 0,

                        NombrePaciente = c.patient != null
                            ? $"{c.patient.firstName} {c.patient.lastName}"
                            : "Sin paciente",

                        NombreOdontologo = c.dentist != null
                            ? $"{c.dentist.firstName} {c.dentist.lastName}"
                            : "Sin odontólogo",

                        TipoCita = c.appointmentType != null
                            ? c.appointmentType.name
                            : "Sin tipo",

                        Monto = c.amount,

                        Motivo = c.reason,
                        Estado = c.status.ToString()
                    }).ToList();

                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al listar citas: " + ex.Message;
                return View(new List<AppointmentViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    long appointmentTypeId = 1;

                    var rq = new createAppointmentRequest
                    {
                        appointmentTypeId = appointmentTypeId,
                        date = model.Fecha.ToString("yyyy-MM-dd"),
                        dentistId = model.DentistId,
                        patientId = model.PatientId,
                        reason = model.Motivo,
                        startTime = model.Hora
                    };

                    await _appointmentService.createAppointmentAsync(rq);
                }

                TempData["Success"] = "Cita registrada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar cita: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _appointmentService.getAppointmentByIdAsync(
                        new getAppointmentByIdRequest { id = id }
                    );

                    var c = response.@return;

                    var model = new AppointmentViewModel
                    {
                        Id = c.id,

                        PatientId = c.patient != null ? c.patient.id : 0,
                        DentistId = c.dentist != null ? c.dentist.id : 0,

                        NombrePaciente = c.patient != null
                            ? $"{c.patient.firstName} {c.patient.lastName}"
                            : "Sin paciente",

                        NombreOdontologo = c.dentist != null
                            ? $"{c.dentist.firstName} {c.dentist.lastName}"
                            : "Sin odontólogo",

                        TipoCita = c.appointmentType != null
                            ? c.appointmentType.name
                            : "Sin tipo",

                        Monto = c.amount,

                        Motivo = c.reason,
                        Estado = c.status.ToString()
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar cita: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    await _appointmentService.cancelAppointmentAsync(
                        new cancelAppointmentRequest { appointmentId = id }
                    );
                }

                TempData["Success"] = "Cita cancelada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cancelar cita: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}