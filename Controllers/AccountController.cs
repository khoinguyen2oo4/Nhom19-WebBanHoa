using Microsoft.AspNetCore.Mvc;
using Nhom19_WebBanHoa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Nhom19_WebBanHoa.Controllers
{
    public class AccountController : Controller
    {
        private readonly FlowerContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(FlowerContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, IFormFile avatarFile)
        {
            // Kiểm tra username đã tồn tại chưa
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists.");
            }

            // Kiểm tra email có @gmail.com
            if (!string.IsNullOrEmpty(user.Email) && !user.Email.EndsWith("@gmail.com"))
            {
                ModelState.AddModelError("Email", "Email của bạn phải có đầy đủ ***@gmail.com");
            }

            // Kiểm tra số điện thoại
            if (!string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumber.Length != 10)
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại của bạn phải đủ 10 số");
            }

            // Kiểm tra xác nhận mật khẩu
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp");
            }

            // Nếu không upload avatar thì gán giá trị mặc định
            if (avatarFile == null || avatarFile.Length == 0)
            {
                user.Avatar = "default-avatar.png";
            }

            if (!ModelState.IsValid)
            {
                // Ghi log lỗi để debug
                var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                ViewBag.RegisterError = errors;
                return View(user);
            }

            // Xử lý upload avatar
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }
                user.Avatar = fileName;
            }

            user.Role = "user"; // Mặc định là user

            // Không lưu ConfirmPassword vào DB
            user.ConfirmPassword = user.Password;

            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ra View để debug, bao gồm inner exception nếu có
                var errorMsg = "Lỗi lưu dữ liệu: " + ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += " | Inner: " + ex.InnerException.Message;
                }
                ViewBag.RegisterError = errorMsg;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)

        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);

                // ✅ Gán Claims để SignalR đọc role và fullname
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? user.Username),
                    new Claim(ClaimTypes.Role, user.Role ?? "user"),
                    new Claim("FullName", user.FullName ?? user.Username)
                };
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                        HttpContext.Session.SetString("Role", user.Role ?? "");
                HttpContext.Session.SetString("Avatar", user.Avatar ?? "default-avatar.png");
                TempData["LoginSuccess"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutSuccess"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Login");
        }

        public IActionResult AdminOnly()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin")
            {
                return RedirectToAction("Login", "Account");
            }
            // Code cho admin ở đây
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.Username == username);
            if (user == null)
                return RedirectToAction("Login");
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User model, IFormFile avatarFile)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id && u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            // Validate fields (except Username, Role)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Password = model.Password;

            // Handle avatar upload
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }
                user.Avatar = fileName;
                HttpContext.Session.SetString("Avatar", user.Avatar);
            }

            _context.SaveChanges();
            TempData["EditSuccess"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }
    }
}
