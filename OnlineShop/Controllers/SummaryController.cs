using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using OnlineShop.ViewModels;
using OnlineShop.Email;
using System.Text;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = WC.CustomerRole)]
    public class SummaryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailService _emailService;

        public SummaryController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, EmailService emailService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._emailService = emailService;
            this._signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            SummaryVM summaryVM = new SummaryVM();
            var sizedItems = _dbContext.SizedItems.Include(si => si.Size).Include(si => si.Item).ThenInclude(i => i.Category);
            summaryVM.User = await _userManager.GetUserAsync(HttpContext.User) as OnlineShop.Models.User;
            summaryVM.Address = summaryVM.User.Address ?? "";

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
        public async Task<IActionResult> Index(SummaryVM summaryVM)
        {
            var user = await _dbContext.Users.FindAsync(summaryVM.User.Id);

            if (user == null)
            {
                TempData[WC.ErrorMessageAlertName] = "There're some problems with your account. Please try again soon.";
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            if (shoppingCartList == null || shoppingCartList.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order()
            {
                UserId = user.Id,
                Address = summaryVM.Address,
                PhoneNumber = summaryVM.PhoneNumber,
                Notes = summaryVM.Notes,
                OrderTime = DateTime.Now,
                OrderStatus = OrderStatus.NotConsidered
            };

            double total = 0;
            int count = 0;
            StringBuilder itemslist = new StringBuilder("");

            foreach (var shoppingCartItem in shoppingCartList)
            {
                var sizedItem = await _dbContext.SizedItems.Include(si => si.Item).ThenInclude(i => i.Category).FirstOrDefaultAsync(si => si.Id == shoppingCartItem.SizedItemId);
                if (sizedItem != null)
                {
                    if (sizedItem.Amount < shoppingCartItem.Amount)
                    {
                        return RedirectToAction("Index", "Cart");
                    }
                    order.OrderedSizedItems.Add(new OrderedSizedItem() { 
                        SizedItemId = sizedItem.Id, 
                        OrderedAmount = shoppingCartItem.Amount 
                    });
                    total += sizedItem.Item.Price * shoppingCartItem.Amount;
                    count += shoppingCartItem.Amount;
                    itemslist.Append($"<li>{sizedItem.Item.Category.Name.ToUpper()} {sizedItem.Item.Name}: x{shoppingCartItem.Amount}</li>");
                }
            }

            order.TotalSum = total;

            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.Remove(WC.SessionCart);
            try
            {
                string msg = $"""
                    <h3>Dear {user.FirstName} {user.LastName}. your order request is accepted.</h3>
                    <div>It's number is <b>{order.Id}</b>.</div>
                    <br/>
                    <h3>Positions:</h3>
                    <ol>{itemslist}</ol>
                    <h3>Total sum: {total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)} $</h3>
                    <div>P. S. If you have not ordered anything or have some quiestions, ask them in our social media:</div>
                    <ul>
                        <li>Telegram: {WC.TelegramLink}</li>
                        <li>Instagram: {WC.InstagramLink}</li>
                    </ul>
                    """;

                await _emailService.SendEmailSync(user.Email, "Order request's accepted!", msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                TempData[WC.ErrorMessageAlertName] = $"Success! Your order number is {order.Id}. But some problems accured while sending message to your email.";
            }

            TempData[WC.MessageAlertName] = $"Success! Your order number is {order.Id}. Please wait for our feedback." +
                $" If you have some questions, ask them in our social media. You can find them on the page below.";

            return RedirectToAction("Index", "Home");
        }

    }
}
