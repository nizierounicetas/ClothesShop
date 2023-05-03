using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using System.Diagnostics;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            this._dbContext = dbContext;
        }

        public IActionResult Index()
        {
            HomeItemVM homeItemVM = new HomeItemVM()
            {
                Items = _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems),
                Categories = _dbContext.Categories,
                Sizes = _dbContext.Sizes
            };

            return View(homeItemVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int? id)
        {
            Item item = await _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems)
                .ThenInclude(si => si.Size).Where(i => i.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            return View(new DetailsVM() { 
                Item = item,
                ExistingSizesSelectList = this.ExtractExistingSizes(item)
                });;
        }

        private IEnumerable<SelectListItem> ExtractExistingSizes(Item item)
        {
            if (item.SizedItems == null)
                return null;

            List<SelectListItem> sizes = new List<SelectListItem>();
            foreach(var sizedItem in item.SizedItems)
            {
                if (sizedItem.Size != null && sizedItem.Amount > 0)
                {
                    sizes.Add(new SelectListItem() { Value = sizedItem.Id.ToString(), Text = sizedItem.Size.Name });
                }
            }

            return sizes;
        }
    }
}