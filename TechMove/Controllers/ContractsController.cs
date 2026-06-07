using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;
using TechMove.Models.ViewModels;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public ContractsController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("Username") != null;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public async Task<IActionResult> Index(ContractFilterViewModel filter)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.SelectedStatus = filter.Status;
            ViewBag.StartDateFrom = filter.StartDateFrom?.ToString("yyyy-MM-dd");
            ViewBag.EndDateTo = filter.EndDateTo?.ToString("yyyy-MM-dd");

            if (IsAdmin())
            {
                if (filter.Status.HasValue)
                {
                    query = query.Where(c => c.Status == filter.Status.Value);
                }

                if (filter.StartDateFrom.HasValue)
                {
                    query = query.Where(c => c.StartDate >= filter.StartDateFrom.Value);
                }

                if (filter.EndDateTo.HasValue)
                {
                    query = query.Where(c => c.EndDate <= filter.EndDateTo.Value);
                }
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();
            return View(contract);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile signedAgreementFile)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (signedAgreementFile == null)
            {
                ModelState.AddModelError("signedAgreementFile", "Signed agreement PDF is required.");
            }

            if (ModelState.IsValid)
            {
                var fileResult = await _fileService.SavePdfAsync(signedAgreementFile);

                if (!fileResult.Success)
                {
                    ModelState.AddModelError("signedAgreementFile", fileResult.ErrorMessage);
                }
                else
                {
                    contract.SignedAgreementFilePath = fileResult.FilePath;
                    contract.SignedAgreementOriginalFileName = fileResult.OriginalFileName;

                    _context.Add(contract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreementFile)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id != contract.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContract = await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingContract == null) return NotFound();

                    if (signedAgreementFile != null)
                    {
                        var fileResult = await _fileService.SavePdfAsync(signedAgreementFile);
                        if (!fileResult.Success)
                        {
                            ModelState.AddModelError("signedAgreementFile", fileResult.ErrorMessage);
                            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
                            return View(contract);
                        }

                        contract.SignedAgreementFilePath = fileResult.FilePath;
                        contract.SignedAgreementOriginalFileName = fileResult.OriginalFileName;
                    }
                    else
                    {
                        contract.SignedAgreementFilePath = existingContract.SignedAgreementFilePath;
                        contract.SignedAgreementOriginalFileName = existingContract.SignedAgreementOriginalFileName;
                    }

                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null || string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                return NotFound();
            }

            var relativePath = contract.SignedAgreementFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var downloadName = string.IsNullOrWhiteSpace(contract.SignedAgreementOriginalFileName)
                ? "SignedAgreement.pdf"
                : contract.SignedAgreementOriginalFileName;

            return File(fileBytes, "application/pdf", downloadName);
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}