using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.ApiClients;
using TechMove.ApiDtos;
using TechMove.Models.Enums;

namespace TechMove.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestApiClient _serviceRequestApiClient;
        private readonly IContractApiClient _contractApiClient;

        public ServiceRequestsController(
            IServiceRequestApiClient serviceRequestApiClient,
            IContractApiClient contractApiClient)
        {
            _serviceRequestApiClient = serviceRequestApiClient;
            _contractApiClient = contractApiClient;
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

        private async Task PopulateContractsDropDownAsync(int? selectedContractId = null)
        {
            var contracts = await _contractApiClient.GetContractsAsync(null, null, null);

            ViewBag.ContractId = new SelectList(
                contracts,
                "Id",
                "ClientName",
                selectedContractId);
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var serviceRequests = await _serviceRequestApiClient.GetServiceRequestsAsync();

            return View(serviceRequests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var serviceRequest = await _serviceRequestApiClient.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            await PopulateContractsDropDownAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceRequestDto dto)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                await PopulateContractsDropDownAsync(dto.ContractId);
                return View(dto);
            }

            var created = await _serviceRequestApiClient.CreateServiceRequestAsync(dto);

            if (created == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The service request could not be created. Please check that the selected contract is valid and active.");

                await PopulateContractsDropDownAsync(dto.ContractId);
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

            var serviceRequest = await _serviceRequestApiClient.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            var dto = new UpdateServiceRequestDto
            {
                ContractId = serviceRequest.ContractId,
                Description = serviceRequest.Description,
                CostUSD = serviceRequest.CostUSD,
                Status = serviceRequest.Status
            };

            ViewBag.ServiceRequestId = id;

            await PopulateContractsDropDownAsync(serviceRequest.ContractId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateServiceRequestDto dto)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ServiceRequestId = id;
                await PopulateContractsDropDownAsync(dto.ContractId);
                return View(dto);
            }

            var updated = await _serviceRequestApiClient.UpdateServiceRequestAsync(id, dto);

            if (!updated)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The service request could not be updated. Please check that the selected contract is valid and active.");

                ViewBag.ServiceRequestId = id;
                await PopulateContractsDropDownAsync(dto.ContractId);
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

            var serviceRequest = await _serviceRequestApiClient.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
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

            var deleted = await _serviceRequestApiClient.DeleteServiceRequestAsync(id);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "The service request could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}