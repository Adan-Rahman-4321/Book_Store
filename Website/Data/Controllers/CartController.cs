using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly Book_DB_Context _context;
		private readonly Cart _cart;

        public CartController(Book_DB_Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            string ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            _cart = new Cart(context, ipAddress);
        }
        public IActionResult Index()
		{
			var items = _cart.GetAllCartItems();
			_cart.CartItems = items;

			return View(_cart);
		}

		public IActionResult AddToCart(int id)
		{
			var selectedBook = GetBookById(id);

			if (selectedBook != null) 
			{
				_cart.AddtoCart(selectedBook, 1);
			}
			return RedirectToAction("Index" , "Cart");
		}

		public IActionResult RemoveFromCart(int id)
		{
			var selectedBook = GetBookById(id);

			if (selectedBook != null)
			{
				_cart.RemoveFromCart(selectedBook);
			}

			return RedirectToAction("Index");
		}

		public IActionResult ReduceQuantity(int id)
		{
			var selectedBook = GetBookById(id);

			if (selectedBook != null)
			{
				_cart.ReduceQuantity(selectedBook);
			}

			return RedirectToAction("Index");
		}

		public IActionResult IncreaseQuantity(int id)
		{
			var selectedBook = GetBookById(id);

			if (selectedBook != null)
			{
				_cart.IncreaseQuantity(selectedBook);
			}

			return RedirectToAction("Index");
		}


		public IActionResult ClearCart()
		{
			_cart.ClearCart();
			return RedirectToAction("Index");
		}

		public Book GetBookById(int id)
		{
			return _context.Books.FirstOrDefault(b => b.Id == id);
		}
	}
}
