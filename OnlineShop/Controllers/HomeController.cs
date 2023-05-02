using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using OnlineShop.Models;
using OnlineShop.Data;
using OnlineShop.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                Items = _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems)!.ThenInclude(si => si.Size),
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
    }
}