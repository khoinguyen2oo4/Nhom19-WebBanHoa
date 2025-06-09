using System;
using System.ComponentModel.DataAnnotations;

namespace Nhom19_WebBanHoa.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }

        // Thêm các thông tin khác nếu cần, ví dụ:
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        // Bạn có thể thêm danh sách chi tiết đơn hàng nếu cần
        // public List<OrderDetail> OrderDetails { get; set; }
    }
}
