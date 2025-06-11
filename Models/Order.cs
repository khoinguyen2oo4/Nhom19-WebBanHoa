using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nhom19_WebBanHoa.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }  // VD: "Đang xử lý", "Đã giao", "Đã huỷ"

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        // Tổng tiền của đơn hàng
        public decimal TotalAmount { get; set; }

        // Danh sách chi tiết đơn hàng
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
