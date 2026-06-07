using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.ApiClients;
using TechMove.ApiDtos;
using TechMove.Models.Enums;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractApiClient _contractApiClient;
        private readonly IClientApiClient _clientApiClient;

        public ContractsController(
            IContractApiClient contractApiClient,
            IClientApiClient clientApiClient)
        {
            _contractApiClient = contractApiClient;
            _clientApiClient = clientApiClient;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Username"));
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        private IActionResult RedirectToLoginIfNeeded()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            return null!;
        }

        private async Task PopulateClientsDropDownAsync(int? selectedClientId = null)
        {
            var clients = await _clientApiClient.GetClientsAsync();

            ViewBag.ClientId = new SelectList(clients, "Id", "Name", selectedClientId);
        }

        public async Task<IActionResult> Index(
            ContractStatus? status,
            DateTime? startDateFrom,
            DateTime? endDateTo)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            ViewBag.IsAdmin = IsAdmin();
            ViewBag.SelectedStatus = status;
            ViewBag.StartDateFrom = startDateFrom?.ToString("yyyy-MM-dd");
            ViewBag.EndDateTo = endDateTo?.ToString("yyyy-MM-dd");

            var contracts = IsAdmin()
                ? await _contractApiClient.GetContractsAsync(status, startDateFrom, endDateTo)
                : await _contractApiClient.GetContractsAsync(null, null, null);

            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var contract = await _contractApiClient.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            await PopulateClientsDropDownAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractDto dto, IFormFile? signedAgreementFile)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientsDropDownAsync(dto.ClientId);
                return View(dto);
            }

            var createdContract = await _contractApiClient.CreateContractAsync(dto);

            if (createdContract == null)
            {
                ModelState.AddModelError(string.Empty, "The contract could not be created. Please try again.");
                await PopulateClientsDropDownAsync(dto.ClientId);
                return View(dto);
            }

            if (signedAgreementFile != null && signedAgreementFile.Length > 0)
            {
                await _contractApiClient.UploadAgreementAsync(createdContract.Id, signedAgreementFile);
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

            var contract = await _contractApiClient.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            var dto = new UpdateContractDto
            {
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel
            };

            ViewBag.ContractId = id;
            ViewBag.HasSignedAgreement = !string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath);

            await PopulateClientsDropDownAsync(contract.ClientId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateContractDto dto, IFormFile? signedAgreementFile)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ContractId = id;
                await PopulateClientsDropDownAsync(dto.ClientId);
                return View(dto);
            }

            var updated = await _contractApiClient.UpdateContractAsync(id, dto);

            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "The contract could not be updated. Please try again.");
                ViewBag.ContractId = id;
                await PopulateClientsDropDownAsync(dto.ClientId);
                return View(dto);
            }

            if (signedAgreementFile != null && signedAgreementFile.Length > 0)
            {
                await _contractApiClient.UploadAgreementAsync(id, signedAgreementFile);
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

            var contract = await _contractApiClient.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
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

            var deleted = await _contractApiClient.DeleteContractAsync(id);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "The contract could not be deleted. It may have linked service requests.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var redirect = RedirectToLoginIfNeeded();

            if (redirect != null)
            {
                return redirect;
            }

            var fileBytes = await _contractApiClient.DownloadAgreementAsync(id);

            if (fileBytes == null)
            {
                return NotFound();
            }

            return File(fileBytes, "application/pdf", "signed-agreement.pdf");
        }
    }
}