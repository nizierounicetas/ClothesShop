using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            CartVM cartVM = new CartVM();
            var sizedItems = _dbContext.SizedItems.Include(si => si.Size).Include(si => si.Item).ThenInclude(i => i.Category);

            if (shoppingCartList != null)
            {
                foreach (var shoppingCartItem in shoppingCartList)
                {
                    var sizedItem = await sizedItems.FirstOrDefaultAsync(si => si.Id == shoppingCartItem.SizedItemId);
                    if (sizedItem != null)
                        cartVM.ItemsDictionary.Add(sizedItem, shoppingCartItem.Amount);
                }
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(cartVM);
        }

        public IActionResult Delete(int? id)
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            if (shoppingCartList != null)
            {
                shoppingCartList.RemoveAll(i => i.SizedItemId == id);
                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                TempData[WC.MessageAlertName] = "Item deleted from cart";
            }
            else
                TempData[WC.ErrorMessageAlertName] = "Some error occured";


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrement(int? id)
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            if (shoppingCartList != null)
            {
                var cartItem = shoppingCartList.FirstOrDefault(i => i.SizedItemId == id);
                if (cartItem != null)
                {
                    cartItem.Amount--;
                    if (cartItem.Amount == 0)
                    {
                        shoppingCartList.Remove(cartItem);
                    }
                    HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                }
            }
            else
                TempData[WC.ErrorMessageAlertName] = "Some error occured";


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Increment(int? id)
        {
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);
            var sizedItem = await _dbContext.SizedItems.FindAsync(id);

            if (shoppingCartList != null)
            {
                var cartItem = shoppingCartList.FirstOrDefault(i => i.SizedItemId == id);
                if (cartItem != null)
                {
                    if (cartItem.Amount == sizedItem.Amount)
                    {
                        TempData[WC.ErrorMessageAlertName] = "No more items available";
                    }
                    else
                    {
                        cartItem.Amount++;
                        HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
                    }
                }
            }
            else
                TempData[WC.ErrorMessageAlertName] = "Some error occured";


            return RedirectToAction(nameof(Index));
        }


    }
}
