using Front_ApiSoap_DentalFlow.Models.Material;
using Microsoft.AspNetCore.Mvc;
using moduloMaterial;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class MaterialController : Controller
    {
        private readonly MaterialEndpoint _materialService;

        public MaterialController(MaterialEndpoint materialService)
        {
            _materialService = materialService;
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
            var channel = (IClientChannel)((ClientBase<MaterialEndpoint>)_materialService).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _materialService.materialGetAllAsync(
                        new materialGetAllRequest()
                    );

                    var materiales = response.@return == null
                    ? new List<material>()
                  : response.@return.ToList();

                    var lista = materiales.Select(m => new MaterialViewModel
                    {
                        Id = m.id,
                        Nombre = m.nombre,
                        Stock = m.stock,
                        StockMinimo = m.stockMinimo,
                        CostoUnitario = m.costoUnitario
                    }).ToList();

                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al listar materiales: " + ex.Message;
                return View(new List<MaterialViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var rq = new materialCreateRequest
                    {
                        nombre = model.Nombre,
                        costoUnitario = model.CostoUnitario,
                        stock = model.Stock,
                        stockMinimo = model.StockMinimo
                    };

                    await _materialService.materialCreateAsync(rq);
                }

                TempData["Success"] = "Material registrado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar material: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _materialService.materialGetByIdAsync(new materialGetByIdRequest(id));
                    var m = response.@return;

                    var model = new MaterialViewModel
                    {
                        Id = m.id,
                        Nombre = m.nombre,
                        Stock = m.stock,
                        StockMinimo = m.stockMinimo,
                        CostoUnitario = m.costoUnitario
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar material: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaterialViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var actualizado = new materialUpdateRequest
                    {
                        id = model.Id,
                        stockMinimo = model.StockMinimo,
                        stock = model.Stock,
                        costoUnitario = model.CostoUnitario,
                        nombre = model.Nombre
                    };

                    await _materialService.materialUpdateAsync(actualizado);
                }

                TempData["Success"] = "Material actualizado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar material: " + ex.Message;
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _materialService.materialGetByIdAsync(new materialGetByIdRequest(id));
                    var m = response.@return;

                    var model = new MaterialViewModel
                    {
                        Id = m.id,
                        Nombre = m.nombre,
                        Stock = m.stock,
                        StockMinimo = m.stockMinimo,
                        CostoUnitario = m.costoUnitario
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar material: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    await _materialService.materialDeleteAsync(new materialDeleteRequest(id));
                }

                TempData["Success"] = "Material eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar material: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StockCritico()
        {
            try
            {
                using (CreateScope())
                {
                    AddSoapAuth();

                    var response = await _materialService.stockCriticoAsync(new stockCriticoRequest());

                    var lista = response.@return.Select(m => new MaterialViewModel
                    {
                        Id = m.id,
                        Nombre = m.nombre,
                        Stock = m.stock,
                        StockMinimo = m.stockMinimo,
                        CostoUnitario = m.costoUnitario
                    }).ToList();

                    return View(lista);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al listar stock crítico: " + ex.Message;
                return View(new List<MaterialViewModel>());
            }
        }
    }
}