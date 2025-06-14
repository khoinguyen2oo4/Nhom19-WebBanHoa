using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom19_WebBanHoa.Models;
using Nhom19_WebBanHoa.Helpers;

namespace Nhom19_WebBanHoa.Controllers
{
    public class OrderController : Controller
    {
        private readonly FlowerContext _context;

        public OrderController(FlowerContext context)
        {
            _context = context;
        }

        // GET: Hiển thị form đặt hàng
        [HttpGet]
        public IActionResult PlaceOrder()
        {

            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
            if (cart.CartItems.Count == 0)
                return RedirectToAction("ViewCart", "Cart");

            ViewBag.Cart = cart;
            return View("Checkout"); // ✅ sửa lại đúng tên file view bạn đang dùng
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
            if (cart.CartItems.Count == 0)
                return RedirectToAction("ViewCart", "Cart");

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                return View("Checkout", model);
            }

            var order = new Order
            {
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                Phone = model.Phone,
                Address = model.Address,
                OrderDate = DateTime.Now,
                Status = "Đang xử lý",
                TotalAmount = cart.CartItems.Sum(i => i.Flower.Gia * i.Quantity),
                OrderItems = cart.CartItems.Select(i => new OrderItem
                {
                    FlowerId = i.FlowerId,
                    Quantity = i.Quantity,
                    Price = i.Flower.Gia
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Load lại đầy đủ đơn hàng (bao gồm Flower)
            var fullOrder = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Flower)
                .FirstOrDefault(o => o.Id == order.Id);

            HttpContext.Session.Remove("Cart");
            return View("OrderSuccess", fullOrder);
        }





        // GET: Lịch sử đơn hàng người dùng
        public IActionResult MyOrders()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var orders = _context.Orders
                .Where(o => o.CustomerEmail == username || o.CustomerName == username)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View("MyOrders", orders);
        }

        // GET: Chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Flower)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
