using Front_ApiSoap_DentalFlow.Models.Appointment;
using Microsoft.AspNetCore.Mvc;
using moduloCitas;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentServiceClient _appointmentService;

        public AppointmentController(AppointmentServiceClient appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _appointmentService.getAllAppointmentsAsync();

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
                long appointmentTypeId = 1;

                await _appointmentService.createAppointmentAsync(
                    model.PatientId,
                    model.DentistId,
                    appointmentTypeId,
                    model.Fecha.ToString("yyyy-MM-dd"),
                    model.Hora,
                    model.Motivo
                );

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
                var response = await _appointmentService.getAppointmentByIdAsync(id);
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
                await _appointmentService.cancelAppointmentAsync(id);

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