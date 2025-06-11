using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nhom19_WebBanHoa.Models;
using System.Linq;

namespace Nhom19_WebBanHoa.Controllers
{
    public class CartController : Controller
    {
        private readonly FlowerContext _context;

        public CartController(FlowerContext context)
        {
            _context = context;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Số lượng phải lớn hơn 0.");
            }

            var cart = GetCartFromSession();
            var existingItem = cart.CartItems.FirstOrDefault(item => item.FlowerId == id);

            if (existingItem != null)
            {
                // Cập nhật số lượng nếu sản phẩm đã có trong giỏ
                existingItem.Quantity += quantity;
            }
            else
            {
                var flower = _context.Flowers.Find(id);
                if (flower == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Flower with ID {id} not found.");
                    return NotFound();
                }
                cart.CartItems.Add(new CartItem { Flower = flower, Quantity = quantity });
            }

            SaveCartToSession(cart);
            return RedirectToAction("ViewCart"); // Sử dụng RedirectToAction để tránh lặp lịch sử
        }

        // Xem giỏ hàng
        public IActionResult ViewCart()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]  // Đảm bảo bảo vệ chống CSRF
        public IActionResult DeleteItem(int id)
        {
            var cart = GetCartFromSession();
            var itemToRemove = cart.CartItems.FirstOrDefault(item => item.FlowerId == id);

            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);  // Xóa sản phẩm khỏi giỏ hàng
                SaveCartToSession(cart);
                System.Diagnostics.Debug.WriteLine($"Deleted item with FlowerId {id}.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Item with FlowerId {id} not found in cart.");
            }

            return RedirectToAction("ViewCart");  // Chuyển hướng về trang giỏ hàng sau khi xóa
        }

        // Xóa nhiều sản phẩm đã chọn khỏi giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]  // Đảm bảo bảo vệ chống CSRF
        public IActionResult DeleteSelectedItems(int[] selectedItems)
        {
            var cart = GetCartFromSession();

            if (selectedItems != null && selectedItems.Length > 0)
            {
                foreach (var flowerId in selectedItems)
                {
                    var itemToRemove = cart.CartItems.FirstOrDefault(item => item.FlowerId == flowerId);
                    if (itemToRemove != null)
                    {
                        cart.CartItems.Remove(itemToRemove);  // Xóa sản phẩm khỏi giỏ hàng
                        System.Diagnostics.Debug.WriteLine($"Deleted item with FlowerId {flowerId}.");
                    }
                }

                SaveCartToSession(cart);
            }

            return RedirectToAction("ViewCart");  // Chuyển hướng về trang giỏ hàng
        }

        // Lấy giỏ hàng từ session
        private Cart GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new Cart();  // Nếu không có giỏ hàng trong session, trả về giỏ hàng mới
            }

            var cart = JsonConvert.DeserializeObject<Cart>(cartJson);
            return cart ?? new Cart();
        }

        // Lưu giỏ hàng vào session
        private void SaveCartToSession(Cart cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));  // Lưu giỏ hàng vào session
            System.Diagnostics.Debug.WriteLine($"Cart saved with {cart.CartItems.Count} items.");
        }
    }
}
