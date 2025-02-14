using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Data;
using Website.Models;
using Website.Services;
using Website.ViewModel;

namespace Website.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly Book_DB_Context _context;
		private readonly Cart _cart;
		private readonly UserManager<DefaultUser> _userManager;
        private readonly IEmailSender _emailSender;

        public OrderController(Book_DB_Context context , Cart cart , UserManager<DefaultUser> userManager , IEmailSender emailSender)
        {
			_context = context;
            _cart = cart;
            _userManager = userManager;
            _emailSender = emailSender;
        }

		[HttpGet]
        public async Task<IActionResult> OrdersList()
        {
            var items = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Book)
        .ToListAsync();
            return View(items);
        }

        

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var cartItems = _cart.GetAllCartItems();
            _cart.CartItems = cartItems;

            if (_cart.CartItems.Count == 0)
            {
                ModelState.AddModelError("", "Cart is empty, Please add book first.");
            }

            if (ModelState.IsValid)
            {
                order.OrderPlaced = DateTime.Now;
                CreateOrder(order);
                _cart.ClearCart();
                return View("CheckoutComplete", order);
            }

            return View(order);
        }


        public  IActionResult CheckoutComplete(Order order)
		{
           
            return View(order);
		}


		public async void CreateOrder(Order order) 
		{
			order.OrderPlaced = DateTime.Now;

			var cartItems = _cart.CartItems;

			foreach (var item in cartItems)
			{
				var orderItem = new OrderItem()
				{
					Quantity = item.Quantity,
					BookId = item.Book.Id,
					OrderId = order.Id,
					Price = item.Book.Price * item.Quantity
				};
				order.OrderItems.Add(orderItem);
				order.OrderTotal += orderItem.Price;
			}
            order.FullName = order.FullName;
            order.Email = order.Email;
            order.CurrentAddress = order.CurrentAddress;
            order.PhoneNo = order.PhoneNo;

            _context.Orders.Add(order);
			_context.SaveChanges();

            await SendOrderStatusUpdateEmail(order);

        }

        [HttpGet]
        public async Task<IActionResult> MyOrdersHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.Email == user.Email)
                .OrderByDescending(o => o.OrderPlaced)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            await SendOrderStatusUpdateEmail(order);

            return RedirectToAction("OrdersList");
        }


        private async Task SendOrderStatusUpdateEmail(Order order)
        {
            string subject = $"Bookeefy Order - Order #{order.Id}";
            string message = $"Dear {order.FullName},<br><br>" +
                             $"The status of your order (Order #{order.Id}) has been updated to: {order.Status}.<br><br>" +
                             $"Thank you!<br><br>" +
                             $"Best regards,<br>Bookeefy";

            await _emailSender.SendEmailAsync(order.Email, subject, message);
        }
    }
}
