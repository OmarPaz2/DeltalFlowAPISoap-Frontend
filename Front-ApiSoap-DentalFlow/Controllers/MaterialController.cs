using Front_ApiSoap_DentalFlow.Models.Material;
using Microsoft.AspNetCore.Mvc;
using moduloMaterial;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class MaterialController : Controller
    {
        private readonly MaterialEndpoint _materialService;

        public MaterialController(MaterialEndpoint materialService)
        {
            _materialService = materialService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _materialService.materialGetAllAsync(new materialGetAllRequest());

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

                var rq = new materialCreateRequest
                {
                    nombre = model.Nombre,
                    costoUnitario = model.CostoUnitario,
                    stock = model.Stock,
                    stockMinimo = model.StockMinimo
                };
                await _materialService.materialCreateAsync(rq);

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
                var actualizado = new materialUpdateRequest
                {
                   id = model.Id,
                    stockMinimo = model.StockMinimo,
                    stock = model.Stock,
                    costoUnitario = model.CostoUnitario,
                    nombre = model.Nombre
                };

                await _materialService.materialUpdateAsync(actualizado);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _materialService.materialDeleteAsync(new materialDeleteRequest(id));
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
            catch (Exception ex)
            {
                TempData["Error"] = "Error al listar stock crítico: " + ex.Message;
                return View(new List<MaterialViewModel>());
            }
        }
    }
}
