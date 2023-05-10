using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    [Authorize]
    public class SummaryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private UserManager<IdentityUser> _userManager { get; set; }

        public SummaryController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            SummaryVM summaryVM = new SummaryVM();
            var sizedItems = _dbContext.SizedItems.Include(si => si.Size).Include(si => si.Item).ThenInclude(i => i.Category);
            summaryVM.User = await _userManager.GetUserAsync(HttpContext.User) as OnlineShop.Models.User;
            
            if (shoppingCartList != null)
            {
                foreach (var shoppingCartItem in shoppingCartList)
                {
                    var sizedItem = await sizedItems.FirstOrDefaultAsync(si => si.Id == shoppingCartItem.SizedItemId);
                    if (sizedItem != null)
                    {
                        if (sizedItem.Amount < shoppingCartItem.Amount)
                        {
                            return RedirectToAction("Index", "Cart");
                        }
                        summaryVM.ItemsDictionary.Add(sizedItem, shoppingCartItem.Amount);
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(summaryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SummaryVM summaryVM)
        {
            TempData[WC.MessageAlertName] = "Your order has been succesfully placed. Please wait our feedback";

            HttpContext.Session.Remove(WC.SessionCart);

            return RedirectToAction("Index", "Home");
        }

    }
}
