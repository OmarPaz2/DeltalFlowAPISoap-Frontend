using Front_ApiSoap_DentalFlow.Models.Tratamiento;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moduloClinicalStaff;
using servicioSesionTratamiento;
using System.Globalization;
using System.ServiceModel;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class SesionTratamientoController : Controller
    {
       private readonly SesionTratamientoEndpoint _sesionTratamientoService;
        private readonly ClinicalStaffEndpoint _clinicalStaffService;

        public SesionTratamientoController(SesionTratamientoEndpoint sesionTratamientoService, ClinicalStaffEndpoint clinicalStaffService)
        {
            _sesionTratamientoService = sesionTratamientoService;
            _clinicalStaffService = clinicalStaffService;
        }
        public async Task<ActionResult> Index(bool? asistencia=null,int idTratamiento=0)
        {
            try
            {
                if (idTratamiento !=0)
                {
                    var rq = new getAllSesionesByIdTratamientoRequest
                    {
                        idTratamiento = idTratamiento
                    };

                    var respo = await _sesionTratamientoService.getAllSesionesByIdTratamientoAsync(rq);

                    var sesionesVm = respo.@return.Select(s => new SesionTratameintoVM
                    {
                        idSesion = s.idSesion,
                        apellidoPaciente = s.apellidoPaciente,
                        costoParcial = s.costoParcial,
                        asistenciaPaciente = s.asistenciaPaciente,
                        dni = s.dni,
                        estado = s.estado,
                        fechaProgramada = DateTime.Parse(s.fechaProgramada),
                        fechaRealizada = DateTime.Parse(s.fechaRealizada),
                        nombrePaciente = s.nombrePaciente,
                        observaciones = s.observaciones
                    }).ToList();

                    return View("Sesion",sesionesVm);
                }

                var parametros = new getByIdUserRequest
                {
                    arg0 = 1
                };
                var perosnal = await _clinicalStaffService.getByIdUserAsync(parametros);


                var request = new sesionesParahoyRequest
                {
                    idOdontologo = perosnal.@return.id,
                    arg1 = asistencia
                };

                var response = await _sesionTratamientoService.sesionesParahoyAsync(request);

                var sesionesVM = response.@return.Select(s => new SesionTratameintoVM
                {
                    idSesion = s.idSesion,
                    apellidoPaciente = s.apellidoPaciente,
                    costoParcial = s.costoParcial,
                    asistenciaPaciente = s.asistenciaPaciente,
                    dni = s.dni,
                    estado = s.estado,
                    fechaProgramada = DateTime.Parse(s.fechaProgramada),
                    fechaRealizada = DateTime.Parse(s.fechaRealizada),
                    nombrePaciente = s.nombrePaciente,
                    observaciones = s.observaciones
                }).ToList();

                return View("Sesion",sesionesVM);
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return View("Sesion",new List<SesionTratameintoVM>());
            }
        }
        // GET: SesionTratamientoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SesionTratamientoController/Create
        public ActionResult Create()
        {
            return View(new SesionTratameintoVM());
        }

        // POST: SesionTratamientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SesionTratamientoRqVM sesion)
        {
            try
            {
                var rq = new registrarSesionRequest
                {
                    request = new sesionTratamientoRegisterRequestDto { 
                    costoParcial = sesion.costoParcial,
                        costoParcialSpecified = true,
                        tiempoDuracion = sesion.tiempoDuracion,
                        idTratamiento = sesion.idTratamiento,
                        fechaProgramada = sesion.fechaProgramada.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
                    }
                };

                var response = await _sesionTratamientoService.registrarSesionAsync(rq);
                var responseForClient = response.@return;
                TempData["Success"] = responseForClient;
                return RedirectToAction(nameof(Index));
            }
            catch (FaultException fault)
            {
                {
                    ViewBag.MensajeError = fault.Reason.ToString();
                    return View(sesion);
                }
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Ocurrio un error inesperado al crear la sesión de tratamiento";
                return View(sesion);
            }
            }


        /* public async Task<ActionResult> Edit(int id)
         {
             try
             {
                 var response = await _sesionTratamientoService.getSesionAsync(new getSesionRequest { idSesion = id });

                 var responseVm = new SesionTratameintoVM
                 {
                     idSesion = response.@return.idSesion,
                     costoParcial = response.@return.costoParcial,
                     apellidoPaciente = response.@return.apellidoPaciente,
                     asistenciaPaciente = response.@return.asistenciaPaciente,
                     dni = response.@return.dni,
                     estado = response.@return.estado,
                     fechaProgramada = DateTime.Parse(response.@return.fechaProgramada),
                     fechaRealizada = DateTime.Parse(response.@return.fechaRealizada),
                     nombrePaciente = response.@return.nombrePaciente,
                     observaciones = response.@return.observaciones
                 };
                 return View(responseVm);
             }
             catch (FaultException fault)
             {
                 TempData["ErrorMessage"] = fault.Reason.ToString();
                 return RedirectToAction(nameof(Index));
             }
         }
        */

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var response = await _sesionTratamientoService.getSesionAsync(new getSesionRequest { idSesion = id });

                var responseVm = new FinalizarSesionVM
                {
                 id = id,
                };
                return PartialView(responseVm);
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FinalizarSesionVM model)
        {
            try
            {
                var rq = new actualizarSesionRequest
                {
                    request = new sesionTratamientoUpdateRequestDto
                    {
                        fechaRealizada = model.fechaRealizada.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                        observaciones = model.observaciones
                    },
                    idSesion = model.id
                };

                var response = await _sesionTratamientoService.actualizarSesionAsync(rq);

                TempData["Success"] = response.@return;
                return RedirectToAction(nameof(Index));
            }
            catch(FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return View();
            }
        }

        //metodo para marcar asistencia del paciente a la sesion de tratamiento
        [HttpPost]
        public async Task<ActionResult> MarcarAsistencia(int idSesion)
        {
            try
            {
                var request = new marcarAsistenciaPacienteRequest
                {
                    arg0 = idSesion,
                };
                var response = await _sesionTratamientoService.marcarAsistenciaPacienteAsync(request);
                TempData["Success"] = response.@return ?? "Asistencia marcada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        //metodo para cancelar sesion
        public async Task<ActionResult> CancelarSesion(int idSesion)
        {
            try
            {
                var request = new cancelarSesionRequest
                {
                    idSesion = idSesion,
                };
                var response = await _sesionTratamientoService.cancelarSesionAsync(request);
                TempData["Success"] = response.@return ?? "Sesión cancelada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        }
}
