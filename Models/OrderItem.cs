namespace Nhom19_WebBanHoa.Models
{
    public class OrderItem
    {
        public int Id { get; set; }  // Mã mục trong đơn hàng
        public int FlowerId { get; set; }  // Mã hoa (khóa ngoại liên kết với Flower)
        public Flower Flower { get; set; }  // Thông tin hoa
        public int Quantity { get; set; }  // Số lượng hoa trong đơn hàng
        public decimal Price { get; set; }  // Giá của hoa khi đặt hàng
    }
}
