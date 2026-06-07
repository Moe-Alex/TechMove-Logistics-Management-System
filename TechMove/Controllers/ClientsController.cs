using Microsoft.AspNetCore.Mvc;
using TechMove.ApiClients;
using TechMove.ApiDtos;

namespace TechMove.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientApiClient _clientApiClient;

        public ClientsController(IClientApiClient clientApiClient)
        {
            _clientApiClient = clientApiClient;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Username"));
        }

        private IActionResult RedirectToLoginIfNeeded()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return null!;
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var clients = await _clientApiClient.GetClientsAsync();

            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var client = await _clientApiClient.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        public IActionResult Create()
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClientDto dto)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var createdClient = await _clientApiClient.CreateClientAsync(dto);

            if (createdClient == null)
            {
                ModelState.AddModelError(string.Empty, "The client could not be created. Please try again.");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var client = await _clientApiClient.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            var dto = new UpdateClientDto
            {
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            };

            ViewBag.ClientId = id;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateClientDto dto)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ClientId = id;
                return View(dto);
            }

            var updated = await _clientApiClient.UpdateClientAsync(id, dto);

            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "The client could not be updated. Please try again.");
                ViewBag.ClientId = id;
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var client = await _clientApiClient.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var deleted = await _clientApiClient.DeleteClientAsync(id);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "The client could not be deleted. It may have linked contracts.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}