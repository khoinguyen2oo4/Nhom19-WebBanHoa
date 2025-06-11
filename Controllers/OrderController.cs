using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using Nhom19_WebBanHoa.Models;

namespace Nhom19_WebBanHoa.Controllers
{
    public class OrderController : Controller
    {
        private readonly FlowerContext _context;

        public OrderController(FlowerContext context)
        {
            _context = context;
        }

        // Thực hiện đặt hàng
        public IActionResult PlaceOrder()
        {
            var cart = GetCartFromSession();
            if (cart.CartItems.Count == 0)
            {
                return RedirectToAction("ViewCart");
            }

            var order = new Order
            {
                CustomerName = "Tên khách hàng", // Lấy tên khách hàng từ form hoặc session
                CustomerEmail = "Email khách hàng", // Lấy email khách hàng từ form hoặc session
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(item => item.Flower.Gia * item.Quantity),
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    FlowerId = item.FlowerId,
                    Quantity = item.Quantity,
                    Price = item.Flower.Gia
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng
            HttpContext.Session.Remove("Cart");

            return View("OrderSuccess", order);
        }

        // Lấy giỏ hàng từ session
        private Cart GetCartFromSession()
        {
            var cart = HttpContext.Session.GetString("Cart");
            if (cart == null)
            {
                return new Cart();
            }

            return JsonConvert.DeserializeObject<Cart>(cart);
        }
    }
}

