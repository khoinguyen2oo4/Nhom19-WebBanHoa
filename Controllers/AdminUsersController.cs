using Microsoft.AspNetCore.Mvc;
using Nhom19_WebBanHoa.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Nhom19_WebBanHoa.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly FlowerContext _context;

        public AdminUsersController(FlowerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Password = model.Password;
            user.Role = model.Role;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}