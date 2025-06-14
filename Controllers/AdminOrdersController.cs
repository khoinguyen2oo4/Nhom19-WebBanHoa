using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom19_WebBanHoa.Models;
using System.Linq;

namespace Nhom19_WebBanHoa.Controllers
{
    public class AdminOrdersController : Controller
    {
        private readonly FlowerContext _context;

        public AdminOrdersController(FlowerContext context)
        {
            _context = context;
        }

        // GET: /AdminOrders
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Flower)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: /AdminOrders/Details/5
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

        // POST: /AdminOrders/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
