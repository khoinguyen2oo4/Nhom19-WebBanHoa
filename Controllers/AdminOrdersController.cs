using Microsoft.AspNetCore.Mvc;
using Nhom19_WebBanHoa.Models;
using System.Linq;

namespace Nhom19_WebBanHoa.Controllers.Admin
{
    public class AdminOrdersController : Controller
    {
        private readonly FlowerContext _context;

        public AdminOrdersController(FlowerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string newStatus)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = newStatus;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
