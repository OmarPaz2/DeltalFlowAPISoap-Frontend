using Front_ApiSoap_DentalFlow.Models;
using Microsoft.AspNetCore.Mvc;
using moduloPaciente;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class PacienteController : Controller
    {
        private readonly PatientEndpoint _patientService;

        public PacienteController(PatientEndpoint patientService)
        {
            _patientService = patientService;
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
            var channel = (IClientChannel)((ClientBase<PatientEndpoint>)_patientService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<ActionResult> Index(string nombre, string dni, string apellido)
        {
            try
            {
                if (dni != null || nombre != null || apellido != null)
                {
                    using (CreateScope())
                    {
                        AddSoapAuth();

                        var parametros = new searchPatientRequest
                        {
                            nombre = nombre,
                            apellido = apellido,
                            dni = dni
                        };

                        searchPatientResponse paciente = await _patientService.searchPatientAsync(parametros);

                        var resultadoView = mappearPacienteResposeSearch(paciente);
                        return View(resultadoView);
                    }
                }
                else
                {
                    using (CreateScope())
                    {
                        AddSoapAuth();

                        getAllPatientsResponse paciente = await _patientService.getAllPatientsAsync(
                            new getAllPatientsRequest()
                        );

                        var pacienteRpList = mappearPacienteRespose(paciente);

                        Console.WriteLine("pacientes cant MAPEADOS: " + pacienteRpList.ToList().Count);

                        return View("Index", pacienteRpList);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al conectar con el servicio SOAP: {ex.Message}");
                return View(new List<PacienteResponse>());
            }
        }

        public ActionResult Create()
        {
            return View(new PacienteRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PacienteRequest requestClient)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    Console.WriteLine(requestClient.birthDate.ToString());

                    string fechaFormateada = requestClient.birthDate?.ToString("yyyy-MM-dd") ?? string.Empty;

                    createPatientRequest requestService = new createPatientRequest
                    {
                        dni = requestClient.dni,
                        address = requestClient.address,
                        birthDate = fechaFormateada,
                        email = requestClient.email,
                        firstName = requestClient.firstName,
                        gender = requestClient.gender,
                        lastName = requestClient.lastName,
                        phone = requestClient.phone
                    };

                    createPatientResponse response = await _patientService.createPatientAsync(requestService);

                    PacienteResponse pacienteCreado = new PacienteResponse
                    {
                        id = response.@return.id,
                        dni = response.@return.dni,
                        firstName = response.@return.firstName,
                        lastName = response.@return.lastName,
                        birthDate = DateOnly.Parse(response.@return.birthDate),
                        gender = response.@return.gender,
                        phone = response.@return.phone,
                        address = response.@return.address,
                        email = response.@return.email
                    };

                    ViewBag.responseService = pacienteCreado;
                    return View(new PacienteRequest());
                }
            }
            catch (FaultException soapEx)
            {
                ViewBag.MensajeError = soapEx.Reason.ToString();
                return View(requestClient);
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Ocurrio un error inesperado al hacer la solicitud";
                return View(requestClient);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var respose = await _patientService.getPatientByIdAsync(
                        new getPatientByIdRequest { id = id }
                    );

                    var paciente = new PacienteResponse
                    {
                        id = respose.@return.id,
                        dni = respose.@return.dni,
                        firstName = respose.@return.firstName,
                        lastName = respose.@return.lastName,
                        birthDate = DateOnly.Parse(respose.@return.birthDate),
                        address = respose.@return.address,
                        email = respose.@return.email,
                        gender = respose.@return.gender,
                        phone = respose.@return.phone
                    };

                    return View(paciente);
                }
            }
            catch (FaultException soapEx)
            {
                TempData["MensajeError"] = soapEx.Reason.ToString();
                return View(new PacienteResponse());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PacienteRequest rq)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var repsonse = await _patientService.updatePatientAsync(new updatePatientRequest
                    {
                        dni = rq.dni,
                        firstName = rq.firstName,
                        lastName = rq.lastName,
                        birthDate = rq.birthDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                        address = rq.address,
                        email = rq.email,
                        gender = rq.gender,
                        phone = rq.phone,
                        id = id
                    });

                    var pacienteActualizado = new PacienteResponse
                    {
                        id = repsonse.@return.id,
                        dni = repsonse.@return.dni,
                        firstName = repsonse.@return.firstName,
                        lastName = repsonse.@return.lastName,
                        birthDate = DateOnly.Parse(repsonse.@return.birthDate),
                        address = repsonse.@return.address,
                        phone = repsonse.@return.phone,
                        gender = repsonse.@return.gender,
                        email = repsonse.@return.email
                    };

                    ViewBag.EditExitoso = true;
                    return View(pacienteActualizado);
                }
            }
            catch (FaultException soapEx)
            {
                ViewBag.MensajeError = soapEx.Reason.ToString();
                return View(rq);
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Ocurrio un error inesperado al editar el paciente";
                return View(rq);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var deleteRq = new deletePatientRequest
                    {
                        id = id
                    };

                    var response = await _patientService.deletePatientAsync(deleteRq);

                    TempData["MensajeSuccess"] = response.@return;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FaultException soapEx)
            {
                TempData["MensajeError"] = soapEx.Reason.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        private List<PacienteResponse> mappearPacienteResposeSearch(searchPatientResponse pacientes)
        {
            return pacientes.@return.Select(p => new PacienteResponse
            {
                id = p.id,
                dni = p.dni,
                firstName = p.firstName,
                lastName = p.lastName,
                birthDate = DateOnly.Parse(p.birthDate),
                gender = p.gender,
                phone = p.phone,
                address = p.address,
                email = p.email
            }).ToList();
        }

        private List<PacienteResponse> mappearPacienteRespose(getAllPatientsResponse allPacientes)
        {
            return allPacientes.@return.Select(p =>
            {
                if (!DateOnly.TryParseExact(p.birthDate, "yyyy-MM-dd", out DateOnly fechaParseada))
                {
                    fechaParseada = DateOnly.MinValue;
                }

                return new PacienteResponse
                {
                    id = p.id,
                    dni = p.dni,
                    firstName = p.firstName,
                    lastName = p.lastName,
                    birthDate = fechaParseada,
                    gender = p.gender,
                    phone = p.phone,
                    address = p.address,
                    email = p.email
                };
            }).ToList();
        }
    }
}