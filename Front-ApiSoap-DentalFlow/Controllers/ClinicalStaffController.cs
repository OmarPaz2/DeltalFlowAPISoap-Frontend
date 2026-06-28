using Front_ApiSoap_DentalFlow.Models;
using Front_ApiSoap_DentalFlow.Models.Specialty;
using Microsoft.AspNetCore.Mvc;
using moduloClinicalStaff;
using moduloEspecialidades;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class ClinicalStaffController : Controller
    {
        private readonly ClinicalStaffEndpoint _clinicalStaffService;
        private readonly SpecialtyEndpoint _specialtyService;

        public ClinicalStaffController(ClinicalStaffEndpoint clinicalStaffService, SpecialtyEndpoint specialtyService)
        {
            _clinicalStaffService = clinicalStaffService;
            _specialtyService = specialtyService;
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

        private IDisposable CreateClinicalStaffScope()
        {
            var channel = (IClientChannel)((ClientBase<ClinicalStaffEndpoint>)_clinicalStaffService).InnerChannel;
            return new OperationContextScope(channel);
        }

        private IDisposable CreateSpecialtyScope()
        {
            var channel = (IClientChannel)((ClientBase<SpecialtyEndpoint>)_specialtyService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<ActionResult> Index(string apellido, int idEspecialidad = 0)
        {
            try
            {
                ViewBag.Especialidades = await obtenerEspecialidades();

                if (!string.IsNullOrEmpty(apellido) || idEspecialidad > 0)
                {
                    using (CreateClinicalStaffScope())
                    {
                        AddSoapAuth();

                        var request = new getAllDentistsBySpecialtyAndLastNameRequest
                        {
                            arg0 = apellido,
                            arg1 = idEspecialidad
                        };

                        var clinicalStaffList = await _clinicalStaffService.getAllDentistsBySpecialtyAndLastNameAsync(request);

                        var clinicalStaffVmList = clinicalStaffList.@return.Select(cs => new ClinicalStaffViewModel
                        {
                            id = cs.id,
                            firstName = cs.firstName,
                            lastName = cs.lastName,
                            licenseNumber = cs.licenseNumber,
                            phone = cs.phone,
                            specialtyName = cs.specialty.name,
                            disponible = cs.disponible
                        }).ToList();

                        return View(clinicalStaffVmList);
                    }
                }

                return View(new List<ClinicalStaffViewModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error inesperado: " + ex.Message;
                return View("Index", new List<ClinicalStaffViewModel>());
            }
        }

        public async Task<ActionResult> Perfil()
        {
            ViewBag.Especialidades = await obtenerEspecialidades();
            return View(new ClinicalStaffRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Perfil(ClinicalStaffRequest model)
        {
            try
            {
                using (CreateClinicalStaffScope())
                {
                    
                    AddSoapAuth();

                    var request = new createDentistRequest
                    {
                        userId =int.TryParse(User.FindFirstValue("userId"), out var id) ? id : 0,
                        specialtyId = model.specialty,
                        licenseNumber = model.licenseNumber,
                        firstName = model.firstName,
                        lastName = model.lastName,
                        phone = model.phone
                    };

                    var response = await _clinicalStaffService.createDentistAsync(request);

                    var responseEntity = new ClinicalStaffViewModel
                    {
                        id = response.@return.id,
                        firstName = response.@return.firstName,
                        lastName = response.@return.lastName,
                        licenseNumber = response.@return.licenseNumber,
                        phone = response.@return.phone,
                        specialtyName = response.@return.specialty.name,
                        disponible = response.@return.disponible
                    };

                    ViewBag.objetoCreado = responseEntity;
                    ViewBag.SuccessMessage = "Perfil completado con exito";
                }

                return View(new ClinicalStaffRequest());
            }
            catch
            {
                ViewBag.ErrorMessage = "Error inesperado al crear el perfil.";
                return View(model);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                using (CreateClinicalStaffScope())
                {
                    AddSoapAuth();

                    var request = new getDentistByIdRequest
                    {
                        id = id
                    };

                    var rsp = await _clinicalStaffService.getDentistByIdAsync(request);

                    var respVM = new ClinicalStaffViewModel
                    {
                        id = rsp.@return.id,
                        firstName = rsp.@return.firstName,
                        lastName = rsp.@return.lastName,
                        licenseNumber = rsp.@return.licenseNumber,
                        phone = rsp.@return.phone,
                        specialtyName = rsp.@return.specialty.name,
                        disponible = rsp.@return.disponible
                    };

                    return View(respVM);
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error inesperado al obtener el perfil.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ClinicalStaffRequest model)
        {
            try
            {
                using (CreateClinicalStaffScope())
                {
                    AddSoapAuth();

                    var request = new updateDentistRequest
                    {
                        arg0 = id,
                        arg1 = new clinicalStaffDto
                        {
                            usuario = model.usuario,
                            firstName = model.firstName,
                            lastName = model.lastName,
                            licenseNumber = model.licenseNumber,
                            phone = model.phone,
                            specialty = model.specialty
                        }
                    };

                    var response = await _clinicalStaffService.updateDentistAsync(request);

                    var especialidades = await obtenerEspecialidades();
                    var especialidad = especialidades.FirstOrDefault(s => s.id == response.@return.specialty);

                    var vm = new ClinicalStaffViewModel
                    {
                        id = response.@return.id,
                        firstName = response.@return.firstName,
                        lastName = response.@return.lastName,
                        licenseNumber = response.@return.licenseNumber,
                        phone = response.@return.phone,
                        specialtyName = especialidad != null ? especialidad.name : "Sin especialidad"
                    };

                    ViewBag.datosActualizados = vm;

                    return View(new ClinicalStaffViewModel());
                }
            }
            catch
            {
                ViewBag.ErrorMessage = "Error inesperado al actualizar el perfil.";
                return View(model);
            }
        }

        private async Task<List<SpecialtyVM>> obtenerEspecialidades()
        {
            using (CreateSpecialtyScope())
            {
                
                AddSoapAuth();

                var especialidades = await _specialtyService.getAllSpecialtiesAsync(
                    new getAllSpecialtiesRequest()
                );

                if (especialidades == null || especialidades.@return == null || !especialidades.@return.Any())
                {
                    return new List<SpecialtyVM>();
                }

                return especialidades.@return.Select(e => new SpecialtyVM
                {
                    id = e.id,
                    name = e.name
                }).ToList();
            }
        }
    }
}