namespace Nhom19_WebBanHoa.Models
{
    public class Cart
    {
        public int Id { get; set; }  // Mã giỏ hàng
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();  // Danh sách các mục trong giỏ hàng
    }
}
