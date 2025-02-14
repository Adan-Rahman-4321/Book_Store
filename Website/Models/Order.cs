using Microsoft.AspNetCore.Identity;

namespace Website.Models
{
	public class Order
	{
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CurrentAddress { get; set; }
        public string PhoneNo { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public decimal OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.OrderPlaced;
    }
}
