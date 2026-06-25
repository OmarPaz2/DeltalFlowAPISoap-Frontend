using Front_ApiSoap_DentalFlow.Models.Tratamiento;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moduloclinicalStaff;
using servicioSesionTratamiento;
using System.Globalization;
using System.ServiceModel;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class SesionTratamientoController : Controller
    {
       private readonly SesionTratamientoEndpoint _sesionTratamientoService;
        private readonly ClinicalStaffService _clinicalStaffService;

        public SesionTratamientoController(SesionTratamientoEndpoint sesionTratamientoService, ClinicalStaffService clinicalStaffService)
        {
            _sesionTratamientoService = sesionTratamientoService;
            _clinicalStaffService = clinicalStaffService;
        }
        public async Task<ActionResult> Index(Boolean asistencia,int idTratamiento)
        {
            try
            {
                if (idTratamiento !=null || idTratamiento !=0)
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

                    return View(sesionesVm);
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

                return View(sesionesVM);
            }
            catch (FaultException fault)
            {
                TempData["ErrorMessage"] = fault.Reason.ToString();
                return View(new List<SesionTratameintoVM>());
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
            return View();
        }

        // POST: SesionTratamientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SesionTratamientoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SesionTratamientoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

     
    }
}
