using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
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

            return View(_dbContext.Categories.OrderBy(c => c.Name));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Category {category.Name} added successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await _dbContext.Categories.FindAsync(id);

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Category {category.Name} edited successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await _dbContext.Categories.FindAsync(id);
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? code)
        {

            var category = await _dbContext.Categories.FindAsync(code);

            if (category == null)
            {
                return NotFound();
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Category {category.Name} deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
  