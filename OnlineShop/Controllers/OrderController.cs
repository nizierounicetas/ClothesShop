using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Email;
using OnlineShop.Models;
using OnlineShop.ViewModels;
using OnlineShop.Utility;
using System.Text;

namespace OnlineShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly MutexSynchronizer _mutexSynchronizer;

        public OrderController(ILogger<HomeController> logger, ApplicationDbContext dbContext, EmailService emailService, MutexSynchronizer mutexSynchronizer)
        {
            _logger = logger; 
            _dbContext = dbContext;
            _emailService = emailService;
            _mutexSynchronizer = mutexSynchronizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotConsidered()
        {
            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            var notConsideredOrders = _dbContext.Orders.Include(o => o.User).Where(o => o.OrderStatus == OrderStatus.NotConsidered).OrderByDescending(o => o.Id);
            return View(notConsideredOrders);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id && o.OrderStatus == OrderStatus.NotConsidered);

            if (order == null)
            {
                return NotFound();
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            var r = ModelState.IsValid;
            foreach (var orderedItem in order.OrderedSizedItems)
            {

                var sizedItem = await _dbContext.SizedItems.FindAsync(orderedItem.SizedItemId);
                if (sizedItem.Amount < orderedItem.OrderedAmount)
                {
                    TempData[WC.ErrorMessageAlertName] = $"Order item {sizedItem.Id} amount can't exсeed available amount";
                    return RedirectToAction();
                }
            }


            _dbContext.Entry(order).State = EntityState.Modified;

            _dbContext.Orders.Update(order);
            _dbContext.OrderedSizedItems.UpdateRange(order.OrderedSizedItems);
            await _dbContext.SaveChangesAsync();

            TempData[WC.MessageAlertName] = $"Order {order.Id} edited successfully!";

            return RedirectToAction(nameof(NotConsidered));
        }

        public async Task<IActionResult> Reject(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id && o.OrderStatus == OrderStatus.NotConsidered);

            if (order == null)
            {
                return NotFound();
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(order);
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPost(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus != OrderStatus.NotConsidered)
            {
                TempData[WC.ErrorMessageAlertName] = $"Error. Order {order.Id} status is {order.OrderStatus}";
                return RedirectToAction();
            }

            try
            {
                //   _mutexSynchronizer.OrderMutex.WaitOne();

                order.OrderStatus = OrderStatus.Rejected;
                order.ResponseTime = DateTime.Now;

                _dbContext.Update(order);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                //  _mutexSynchronizer.OrderMutex.ReleaseMutex();
            }


            try
            {
                string msg = $"""
                    <h3>Dear {order.User.FirstName} {order.User.LastName}. Your order {order.Id} is rejected unfortunately.</h3>
                    <br/>
                    <div>P. S. If you have not ordered anything or have some quiestions, ask them in our social media:</div>
                    <ul>
                        <li>Telegram: {WC.TelegramLink}</li>
                        <li>Instagram: {WC.InstagramLink}</li>
                    </ul>
                    """;

                await _emailService.SendEmailSync(order.User.Email, "Order request's rejected :(", msg);
            }
            catch (Exception ex)
            {
                TempData[WC.ErrorMessageAlertName] = $"Order {order.Id} is rejected! But User {order.User.UserName} is not aware of this!";
                _logger.LogError(ex.Message);
            }

            TempData[WC.MessageAlertName] = $"Order {order.Id} is rejected!";
            return RedirectToAction(nameof(NotConsidered));
        }


        public async Task<IActionResult> Confirm(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id && o.OrderStatus == OrderStatus.NotConsidered);

            if (order == null)
            {
                return NotFound();
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(order);
        }

        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPost(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus != OrderStatus.NotConsidered)
            {
                TempData[WC.ErrorMessageAlertName] = $"Error. Order {order.Id} status is {order.OrderStatus}";
                return RedirectToAction();
            }

            StringBuilder itemslist = new StringBuilder("");

            try
            {
             //   _mutexSynchronizer.OrderMutex.WaitOne();

                foreach(OrderedSizedItem orderedSizedItem in order.OrderedSizedItems)
                { 
                    if (orderedSizedItem.OrderedAmount <= orderedSizedItem.SizedItem.Amount)
                    {
                        itemslist.Append($"<li>{orderedSizedItem.SizedItem.Item.Category.Name.ToUpper()} {orderedSizedItem.SizedItem.Item.Name}: x{orderedSizedItem.OrderedAmount}</li>");
                        orderedSizedItem.SizedItem.Amount -= orderedSizedItem.OrderedAmount;

                        _dbContext.Update(orderedSizedItem.SizedItem);
                    }
                    else
                    {
                        TempData[WC.ErrorMessageAlertName] = $"Order item {orderedSizedItem.Id} amount can't exсeed available amount";
                        return RedirectToAction();
                    }
                }

                order.OrderStatus = OrderStatus.Confirmed;
                order.ResponseTime = DateTime.Now;

                _dbContext.Update(order);
                await _dbContext.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
              //  _mutexSynchronizer.OrderMutex.ReleaseMutex();
            }


            try
            {
                string msg = $"""
                    <h3>Dear {order.User.FirstName} {order.User.LastName}.Your order {order.Id} is confirmed.</h3>
                    <br/>
                    <h3>Positions:</h3>
                    <ol>{itemslist}</ol>
                    <h3>Total sum: {order.TotalSum.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)} $</h3>
                    <div>P. S. If you have not ordered anything or have some quiestions, ask them in our social media:</div>
                    <ul>
                        <li>Telegram: {WC.TelegramLink}</li>
                        <li>Instagram: {WC.InstagramLink}</li>
                    </ul>
                    """;

                await _emailService.SendEmailSync(order.User.Email, "Order request's confirmed!", msg);
            }
            catch(Exception ex)
            {
                TempData[WC.ErrorMessageAlertName] = $"Order {order.Id} is confirmed! But User {order.User.UserName} is not aware of this!";
                _logger.LogError(ex.Message);
            }

            TempData[WC.MessageAlertName] = $"Order {order.Id} is confirmed!";
            return RedirectToAction(nameof(NotConsidered));
        }

        public IActionResult Confirmed()
        {
            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            var confirmedOrders = _dbContext.Orders.Include(o => o.User).Where(o => o.OrderStatus == OrderStatus.Confirmed).OrderByDescending(o => o.Id); ;
            return View(confirmedOrders);
        }

        public IActionResult Rejected()
        {
            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            var deniedOrders = _dbContext.Orders.Include(o => o.User).Where(o => o.OrderStatus == OrderStatus.Rejected).OrderByDescending(o => o.Id); ;
            return View(deniedOrders);
        }

        public async Task<IActionResult> Restore(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id && o.OrderStatus != OrderStatus.NotConsidered);

            if (order == null)
            {
                return NotFound();
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(order);
        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestorePost(int? id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Item)
                .ThenInclude(i => i.Category)
                .Include(o => o.OrderedSizedItems)
                .ThenInclude(osi => osi.SizedItem)
                .ThenInclude(si => si.Size)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus == OrderStatus.NotConsidered)
            {
                TempData[WC.ErrorMessageAlertName] = $"Error. Order {order.Id} status is {order.OrderStatus}";
                return RedirectToAction();
            }


            try
            {
                //   _mutexSynchronizer.OrderMutex.WaitOne();

                if (order.OrderStatus == OrderStatus.Confirmed)
                {
                    foreach (OrderedSizedItem orderedSizedItem in order.OrderedSizedItems)
                    {
                        orderedSizedItem.SizedItem.Amount += orderedSizedItem.OrderedAmount;
                        _dbContext.Update(orderedSizedItem.SizedItem);
                    }
                }

                order.OrderStatus = OrderStatus.NotConsidered;
                order.ResponseTime = null;

                _dbContext.Update(order);

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                //  _mutexSynchronizer.OrderMutex.ReleaseMutex();
            }

            TempData[WC.MessageAlertName] = $"Order {order.Id} is sent to \"Not considered\"!";

            return RedirectToAction(nameof(NotConsidered));

        }

        private OrdersVM GetOrdersVM()
        {
            var orders = _dbContext.Orders.Include(o => o.User);
            var ordersVM = new OrdersVM()
            {
                ConfirmedOrders = orders.Where(o => o.OrderStatus == OrderStatus.Confirmed),
                NotConsideredOrders = orders.Where(o => o.OrderStatus == OrderStatus.NotConsidered),
                DeniedOrders = orders.Where(o => o.OrderStatus == OrderStatus.Rejected)
            };


            return ordersVM;
        }
    }
}
