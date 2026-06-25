using Front_ApiSoap_DentalFlow.Models.Specialty;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moduloEspecialidades;
using moduloPaciente;
using System.ServiceModel;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class SpecialtyController : Controller
    {
        private readonly SpecialtyService _specialtyService;

        public SpecialtyController(SpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var response = await _specialtyService.getAllSpecialtiesAsync(new getAllSpecialtiesRequest());

                if (response == null || response.@return == null)
                {
                   
                    return View(new List<SpecialtyVM>());
                }

                List<SpecialtyVM>specialties =response.@return.Select(x => new SpecialtyVM
                {
                    id = x.id,
                    name = x.name
                }).ToList();

                return View(specialties);
            }
            catch (Exception ex) {
                TempData["Error"] = "Error al obtener las especialidades. Intente nuevamente mas tarde";
                return View(new List<SpecialtyVM>());
            }

            
        }


        
        public ActionResult Create()
        {
            //partialView trae el contenido hmtl de la pagina y lo coloca en la vista actual, sin recargar toda la página y sin asignarle el layaut
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

                    var request = new createSpecialtyRequest
                    {
                        arg0 = name
                    };
                    var response = await _specialtyService.createSpecialtyAsync(request);

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
            var especilidad = await _specialtyService.getAllSpecialtiesAsync(new getAllSpecialtiesRequest());

            var seleccion = especilidad.@return.FirstOrDefault(x => x.id == id);

            var vm = new SpecialtyVM
            {
                id = seleccion.id,
                name = seleccion.name
            };

            return PartialView(vm);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SpecialtyVM model)
        {
            try
            {
                var parametros = new updateSpecialtyRequest
                {
                    arg0 = model.id,
                    arg1 = model.name
                };

                var resultado = await _specialtyService.updateSpecialtyAsync(parametros);
                TempData["Success"] = resultado.@return;
                return RedirectToAction(nameof(Index));
            }
            catch(FaultException soapEx)
            {
                ViewBag.MensajeError = soapEx.Reason.ToString();
                return View(model);
            }
            
        }

  
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleteRq = new deleteSpecialtyRequest
                {
                    arg0 = id
                };
                var response = await _specialtyService.deleteSpecialtyAsync(deleteRq);


                TempData["MensajeSuccess"] = response.@return;
                return RedirectToAction(nameof(Index));
            }
            catch (FaultException soapEx)
            {
                TempData["MensajeError"] = soapEx.Reason.ToString();
                return View("Index");
            }
        }
    }
}
