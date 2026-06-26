using Front_ApiSoap_DentalFlow.Models.Tratamiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moduloClinicalStaff;
using servicioAuth;
using servicioSesionTratamiento;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class SesionTratamientoController : Controller
    {
       private readonly SesionTratamientoEndpoint _sesionTratamientoService;
        private readonly ClinicalStaffEndpoint _clinicalStaffService;
        private readonly AuthEndpoint _authEndpoint;
        public SesionTratamientoController(SesionTratamientoEndpoint sesionTratamientoService, ClinicalStaffEndpoint clinicalStaffService, AuthEndpoint authEndpoint)
        {
            _sesionTratamientoService = sesionTratamientoService;
            _clinicalStaffService = clinicalStaffService;
            _authEndpoint = authEndpoint;
        }

        private void AddSoapAuth()
        {
            var token = Request.Cookies["jwt_token"];

            if (string.IsNullOrEmpty(token))
                return;

            var httpRequest = new HttpRequestMessageProperty();
            httpRequest.Headers["Authorization"] = $"Bearer {token}";

            OperationContext.Current.OutgoingMessageProperties[
                HttpRequestMessageProperty.Name
            ] = httpRequest;
        }

        private IDisposable CreateScope()
        {
            var channel = (IClientChannel)((ClientBase<AuthEndpoint>)_authEndpoint).InnerChannel;
            return new OperationContextScope(channel);
        }
        [Authorize(Roles = "RECEPCIONISTA,ODONTOLOGO")]
        public async Task<ActionResult> Index(bool? asistencia,int idTratamiento=0)
        {
            try
            {
                if (idTratamiento !=0)
                {
                    using (CreateScope())
                    {
                        AddSoapAuth();
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

                        return View("Sesion", sesionesVm);
                    }
                }
                using (CreateScope())
                {
                    AddSoapAuth();

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

                    return View("Sesion", sesionesVM);
                }
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

        [Authorize(Roles = "ODONTOLOGO")]
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
                using (CreateScope())
                {
                    AddSoapAuth();

                    var rq = new registrarSesionRequest
                    {
                        request = new sesionTratamientoRegisterRequestDto
                        {
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
        [Authorize(Roles = "ODONTOLOGO")]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();
                    var response = await _sesionTratamientoService.getSesionAsync(new getSesionRequest { idSesion = id });

                    var responseVm = new FinalizarSesionVM
                    {
                        id = id,
                    };
                    return PartialView(responseVm);
                }
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
                using (CreateScope())
                {
                    AddSoapAuth();
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
            }
            catch(FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return PartialView(model);
            }
        }

        //metodo para marcar asistencia del paciente a la sesion de tratamiento
        [Authorize(Roles = "RECEPCIONISTA")]
        [HttpPost]
        public async Task<ActionResult> MarcarAsistencia(int idSesion)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();
                    var request = new marcarAsistenciaPacienteRequest
                    {
                        arg0 = idSesion,
                    };
                    var response = await _sesionTratamientoService.marcarAsistenciaPacienteAsync(request);
                    TempData["Success"] = response.@return ?? "Asistencia marcada correctamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        //metodo para cancelar sesion
        [Authorize(Roles = "ODONTOLOGO")]
        public async Task<ActionResult> CancelarSesion(int idSesion)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();
                    var request = new cancelarSesionRequest
                    {
                        idSesion = idSesion,
                    };
                    var response = await _sesionTratamientoService.cancelarSesionAsync(request);
                    TempData["Success"] = response.@return ?? "Sesión cancelada correctamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        }
}
