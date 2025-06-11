namespace Nhom19_WebBanHoa.Models
{
    public class CartItem
    {
        public int Id { get; set; }  // Mã mục trong giỏ hàng
        public int FlowerId { get; set; }  // Mã hoa (khóa ngoại liên kết với Flower)
        public Flower Flower { get; set; }  // Thông tin hoa
        public int Quantity { get; set; }  // Số lượng hoa trong giỏ hàng
    }
}
