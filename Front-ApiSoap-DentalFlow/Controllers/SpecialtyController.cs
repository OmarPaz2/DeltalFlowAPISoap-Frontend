using Front_ApiSoap_DentalFlow.Models.Specialty;
using Microsoft.AspNetCore.Mvc;
using moduloEspecialidades;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class SpecialtyController : Controller
    {
        private readonly SpecialtyEndpoint _specialtyService;

        public SpecialtyController(SpecialtyEndpoint specialtyService)
        {
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

        private IDisposable CreateScope()
        {
            var channel = (IClientChannel)((ClientBase<SpecialtyEndpoint>)_specialtyService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _specialtyService.getAllSpecialtiesAsync(
                        new getAllSpecialtiesRequest()
                    );

                    if (response == null || response.@return == null)
                    {
                        return View(new List<SpecialtyVM>());
                    }

                    List<SpecialtyVM> specialties = response.@return.Select(x => new SpecialtyVM
                    {
                        id = x.id,
                        name = x.name
                    }).ToList();

                    return View(specialties);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al obtener las especialidades. Intente nuevamente mas tarde";
                return View(new List<SpecialtyVM>());
            }
        }

        public ActionResult Create()
        {
            return PartialView(new SpecialtyVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    using (CreateScope())
                    {
                        AddSoapAuth();

                        var request = new createSpecialtyRequest
                        {
                            name = name
                        };

                        await _specialtyService.createSpecialtyAsync(request);
                    }

                    TempData["MensajeSuccess"] = "Especialidad creada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (FaultException fal)
                {
                    TempData["Error"] = fal.Reason.ToString();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la especialidad. Intente nuevamente mas tarde";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.ErrorMessage = "El nombre de la especialidad no puede estar vacío.";
            return PartialView();
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var especilidad = await _specialtyService.getAllSpecialtiesAsync(
                        new getAllSpecialtiesRequest()
                    );

                    var seleccion = especilidad.@return.FirstOrDefault(x => x.id == id);

                    if (seleccion == null)
                    {
                        TempData["Error"] = "Especialidad no encontrada";
                        return RedirectToAction(nameof(Index));
                    }

                    var vm = new SpecialtyVM
                    {
                        id = seleccion.id,
                        name = seleccion.name
                    };

                    return PartialView(vm);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar la especialidad";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SpecialtyVM model)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var parametros = new updateSpecialtyRequest
                    {
                        id = model.id,
                        name = model.name
                    };

                    var resultado = await _specialtyService.updateSpecialtyAsync(parametros);

                    TempData["Success"] = resultado.@return;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (FaultException soapEx)
            {
                ViewBag.MensajeError = soapEx.Reason.ToString();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Error inesperado al actualizar la especialidad";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var deleteRq = new deleteSpecialtyRequest
                    {
                        arg0 = id
                    };

                    var response = await _specialtyService.deleteSpecialtyAsync(deleteRq);

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
    }
}