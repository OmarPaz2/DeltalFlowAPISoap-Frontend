using Front_ApiSoap_DentalFlow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using servicioAuth;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AuthEndpoint _authEndpoint;
        
        public UsuarioController(AuthEndpoint authEndpoint)
        {
            _authEndpoint = authEndpoint;
        }

        private void AddSoapAuth()
        {
            var token = Request.Cookies["jwt_token"];

            if (string.IsNullOrEmpty(token))
                return;

            var httpRequest = new HttpRequestMessageProperty();
            httpRequest.Headers["Authorization"] = $"Bearer {token}";

            OperationContext.Current.OutgoingMessageProperties[
                HttpRequestMessageProperty.Name
            ] = httpRequest;
        }

        private IDisposable CreateScope()
        {
            var channel = (IClientChannel)((ClientBase<AuthEndpoint>)_authEndpoint).InnerChannel;
            return new OperationContextScope(channel);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
               

                using (CreateScope())
                {
                    AddSoapAuth();
                    var request = new getAllUsersRequest();
                    var response = await _authEndpoint.getAllUsersAsync(request);

                    var usuarios = response.@return?.ToList() ?? new List<usuario>();

                    return View(usuarios);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<usuario>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UsuarioViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var request = new createUserRequest
            {
                username = model.Username,
                password = model.Password,
                rolId = model.RolId
            };

            using (CreateScope())
            {
                AddSoapAuth();

                await _authEndpoint.createUserAsync(request);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = new getAllUsersRequest();

            using (CreateScope())
            {
                AddSoapAuth();

                var response = await _authEndpoint.getAllUsersAsync(request);

                var usuario = response.@return?.FirstOrDefault(x => x.id == id);

                if (usuario == null)
                    return NotFound();

                var vm = new UsuarioViewModel
                {
                    Id = usuario.id,
                    Username = usuario.username,
                    RolId = usuario.rol?.id ?? 0,
                    Activo = usuario.activo
                };

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var request = new updateUserRequest
            {
                userId = model.Id,
                username = model.Username,
                rolId = model.RolId,
                activo = model.Activo
            };

            using (CreateScope())
            {
                AddSoapAuth();

                await _authEndpoint.updateUserAsync(request);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new disableUserRequest
            {
                userId = id
            };

            using (CreateScope())
            {
                AddSoapAuth();

                await _authEndpoint.disableUserAsync(request);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}