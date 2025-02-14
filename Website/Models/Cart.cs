using Microsoft.EntityFrameworkCore;
using Website.Data;
using Website.Migrations;

namespace Website.Models
{
	public class Cart
	{
        private readonly Book_DB_Context _context;
        private readonly string _ipAddress;
        public Cart(Book_DB_Context context, string ipAddress)
        {
            _context = context;
            _ipAddress = ipAddress;
        }


        public List<CartItem> CartItems { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            var context = services.GetService<Book_DB_Context>();
            var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            string ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            return new Cart(context, ipAddress);
        }

        public List<CartItem> GetAllCartItems()
        {
            return CartItems ??
                (CartItems = _context.CartItems
                .Where(ci => ci.IpAddress == _ipAddress)
                .Include(ci => ci.Book)
                .ToList());
        }

        public CartItem GetCartItem(Book book)
        {
            return _context.CartItems.SingleOrDefault(ci => ci.Book.Id == book.Id && ci.IpAddress == _ipAddress);
        }

        public void AddtoCart(Book book, int quantity)
        {
            var cartItem = GetCartItem(book);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Book = book,
                    Quantity = quantity,
                    IpAddress = _ipAddress
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            _context.SaveChanges();
        }


        public int ReduceQuantity(Book book)
        {
            var cartItem = GetCartItem(book);
            var remainingQuantity = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    remainingQuantity = --cartItem.Quantity;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            _context.SaveChanges();
            return remainingQuantity;
        }

		public int IncreaseQuantity(Book book)
		{
			var cartItem = GetCartItem(book);
			var remainingQuantity = 0;

			if (cartItem != null)
			{
				if (cartItem.Quantity > 0)
				{
					remainingQuantity = ++cartItem.Quantity;
				}
				
			}
			_context.SaveChanges();
			return remainingQuantity;
		}

        public void RemoveFromCart(Book book)
        {
            var cartItem = GetCartItem(book);

            if(cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
            }
            _context.SaveChanges();
        }

        public void ClearCart()
        {
            var cartItems = _context.CartItems.Where(ci => ci.IpAddress == _ipAddress);
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();
        }

        public decimal GetCartTotal()
        {
            return _context.CartItems
                .Where(ci => ci.IpAddress == _ipAddress)
                .Select(ci => ci.Book.Price * ci.Quantity)
                .Sum();
        }
    }
}
