using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;
using TechMove.Models.ViewModels;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ContractValidationService _contractValidationService;
        private readonly CurrencyApiService _currencyApiService;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ContractValidationService contractValidationService,
            CurrencyApiService currencyApiService)
        {
            _context = context;
            _contractValidationService = contractValidationService;
            _currencyApiService = currencyApiService;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("Username") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var serviceRequests = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();

            return View(serviceRequests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var vm = new ServiceRequestCreateViewModel
            {
                Contracts = await _context.Contracts
                    .Include(c => c.Client)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"Contract #{c.Id} - {c.Client!.Name} ({c.Status})"
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel vm)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == vm.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Selected contract was not found.");
            }
            else if (!_contractValidationService.CanCreateServiceRequest(contract.Status))
            {
                ModelState.AddModelError("ContractId", "A service request cannot be created for an Expired or On Hold contract.");
            }

            var rate = await _currencyApiService.GetUsdToZarRateAsync();
            if (rate == null)
            {
                ModelState.AddModelError(string.Empty, "Could not retrieve the exchange rate at this time.");
            }
            else
            {
                vm.ExchangeRateUsed = rate.Value;
                vm.LocalCostZAR = _currencyApiService.ConvertUsdToZar(vm.CostUSD, rate.Value);
            }

            if (ModelState.IsValid)
            {
                var serviceRequest = new ServiceRequest
                {
                    ContractId = vm.ContractId,
                    Description = vm.Description,
                    CostUSD = vm.CostUSD,
                    ExchangeRateUsed = vm.ExchangeRateUsed,
                    LocalCostZAR = vm.LocalCostZAR,
                    Status = vm.Status
                };

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.Contracts = await _context.Contracts
                .Include(c => c.Client)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"Contract #{c.Id} - {c.Client!.Name} ({c.Status})"
                })
                .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null) return NotFound();

            ViewBag.Contracts = await _context.Contracts
                .Include(c => c.Client)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"Contract #{c.Id} - {c.Client!.Name} ({c.Status})"
                })
                .ToListAsync();

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id != serviceRequest.Id) return NotFound();

            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Selected contract was not found.");
            }
            else if (!_contractValidationService.CanCreateServiceRequest(contract.Status))
            {
                ModelState.AddModelError("ContractId", "A service request cannot be linked to an Expired or On Hold contract.");
            }

            var rate = await _currencyApiService.GetUsdToZarRateAsync();
            if (rate == null)
            {
                ModelState.AddModelError(string.Empty, "Could not retrieve the exchange rate at this time.");
            }
            else
            {
                serviceRequest.ExchangeRateUsed = rate.Value;
                serviceRequest.LocalCostZAR = _currencyApiService.ConvertUsdToZar(serviceRequest.CostUSD, rate.Value);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Contracts = await _context.Contracts
                .Include(c => c.Client)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"Contract #{c.Id} - {c.Client!.Name} ({c.Status})"
                })
                .ToListAsync();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}