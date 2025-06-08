using Microsoft.AspNetCore.Mvc;
using Nhom19_WebBanHoa.Models;
using Microsoft.EntityFrameworkCore;

namespace Nhom19_WebBanHoa.Controllers
{
    public class ProductController : Controller
    {
        private readonly FlowerContext _context;

        public ProductController(FlowerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ProductList()
        {
            var flowers = await _context.Flowers.ToListAsync();
            return View(flowers);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null)
            {
                return NotFound();
            }

            return View(flower);
        }

    }
}
