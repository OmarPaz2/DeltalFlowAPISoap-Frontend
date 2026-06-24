using Front_ApiSoap_DentalFlow.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using moduloDashboard;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardServiceImplClient _dashboardService;

        public DashboardController(DashboardServiceImplClient dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _dashboardService.obtenerMetricasAsync();
                var d = response.@return;

                var model = new DashboardViewModel
                {
                    CitasDelDia = d.citasDelDia,
                    TratamientosActivos = d.tratamientosActivos,
                    PagosRealizados = d.pagosRealizados,
                    StockCritico = d.stockCritico
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar Dashboard: " + ex.Message;
                return View(new DashboardViewModel());
            }
        }
    }
}
