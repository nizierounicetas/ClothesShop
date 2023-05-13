using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class SizeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SizeController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IActionResult Index()
        {
            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(_dbContext.Sizes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Size size)
        {
            if(!ModelState.IsValid)
            {
                return View(size);
            }

            await _dbContext.AddAsync(size);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Size {size.Name} added succesfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var size = await _dbContext.Sizes.FindAsync(id);

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View(size);
            }

            _dbContext.Sizes.Update(size);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Size {size.Name} edited succesfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var size = await _dbContext.Sizes.FindAsync(id);

            return View(size);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? code)
        {
            var size = await _dbContext.Sizes.FindAsync(code);

            if (size == null)
            {
                return NotFound();
            }

            _dbContext.Sizes.Remove(size);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Size {size.Name} deleted succesfully!";

            return RedirectToAction(nameof(Index));
        }

    }
}
