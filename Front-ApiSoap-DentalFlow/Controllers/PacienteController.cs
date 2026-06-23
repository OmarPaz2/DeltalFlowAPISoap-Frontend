using Front_ApiSoap_DentalFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moduloCitas;
using moduloPaciente;
using System.Collections;
namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class PacienteController : Controller
    {

        private readonly PatientService _patientService;

        public PacienteController(PatientService patientService)
        {
            _patientService = patientService;
        }
        public async Task<ActionResult> Index(string nombre, string dni, string apellido)
        {
            try
            {
                IEnumerable<PacienteResponse> response = new List<PacienteResponse>();
                if (dni != null || nombre != null || apellido != null)
                {
                    var parametros = new searchPatientRequest
                    {

                        arg0 = dni,
                        arg1 = nombre,
                        arg2 = apellido
                    };
                    searchPatientResponse paciente = await _patientService.searchPatientAsync(parametros);

                    var resultadoView = mappearPacienteResposeSearch(paciente);
                    return View(resultadoView);
                }
                else
                {
                    getAllPatientsResponse paciente = await _patientService.getAllPatientsAsync(new getAllPatientsRequest());

                    return View(mappearPacienteRespose(paciente));
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al conectar con el servicio SOAP: {ex.Message}");

            }
            return View(new List<PacienteResponse>());
        }

        // GET: PacienteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PacienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PacienteController/Create
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

        // GET: PacienteController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PacienteController/Edit/5
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

        // GET: PacienteController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PacienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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


            return allPacientes.@return.Select(p => new PacienteResponse
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
    }

}
