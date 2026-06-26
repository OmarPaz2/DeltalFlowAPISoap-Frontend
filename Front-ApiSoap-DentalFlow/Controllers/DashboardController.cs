using Front_ApiSoap_DentalFlow.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using moduloDashboard;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardEndpoint _dashboardService;

        public DashboardController(DashboardEndpoint dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _dashboardService.obtenerMetricasAsync(new obtenerMetricasRequest());
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

        public async Task<IActionResult> Grafica()
        {
            try
            {
                var response = await _dashboardService.obtenerPagosTotalesUltimos5MesesAsync(new obtenerPagosTotalesUltimos5MesesRequest ());

                var pagosUltimos5Meses = response.@return.Select(p => new PagosUltimos5MesesVM
                {
                    anio = p.anio,
                    mes = GetMonthName(p.mes),
                    totalPago = p.totalPago
                }).ToList();
                Console.WriteLine("PagosUltimos5Meses: " + pagosUltimos5Meses.Count);
                return View(pagosUltimos5Meses);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la gráfica: " + ex.Message;
                return View(new List<PagosUltimos5MesesVM>());
            }

        }

        private string GetMonthName(int month)
        {
            return new DateTime(1, month, 1).ToString("MMMM");
        }
    }
}
