using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nhom19_WebBanHoa.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại của bạn phải đủ 10 số")]
        public string PhoneNumber { get; set; }

        // Avatar không bắt buộc, cho phép null
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Không lưu ConfirmPassword vào DB, chỉ dùng để xác nhận khi đăng ký
        [NotMapped]
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        // Vai trò không bắt buộc, nullable
        public string? Role { get; set; }
    }
}
