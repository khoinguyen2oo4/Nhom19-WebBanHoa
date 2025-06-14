using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace Nhom19_WebBanHoa.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();
        private static readonly ConcurrentDictionary<string, string> _clientConnections = new();
        private static readonly ConcurrentDictionary<string, string> _supportChatTargets = new();

        public async Task SendMessage(string message)
        {
            if (!_connections.TryGetValue(Context.ConnectionId, out var sender)) return;

            bool isSupport = sender.StartsWith("Nhân viên");

            if (isSupport)
            {
                if (_supportChatTargets.TryGetValue(Context.ConnectionId, out var customerConnId) &&
                    !string.IsNullOrEmpty(customerConnId) &&
                    _connections.ContainsKey(customerConnId))
                {
                    await Clients.Client(customerConnId).SendAsync("ReceiveMessage", sender, message);
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "⚠️ Hệ thống", "Bạn chưa chọn khách hàng hoặc khách đã ngắt kết nối.");
                }
            }
            else
            {
                await Clients.Group("Support").SendAsync("ReceiveMessage", sender, message);
                _clientConnections[Context.ConnectionId] = sender;
                await UpdateAllAdminsCustomerList();
            }
        }

        public async Task SelectCustomer(string customerConnId)
        {
            if (string.IsNullOrEmpty(customerConnId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "⚠️ Hệ thống", "Không tìm thấy connectionId của khách.");
                return;
            }

            if (!_connections.ContainsKey(customerConnId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "⚠️ Hệ thống", "Khách đã ngắt kết nối.");
                return;
            }

            _supportChatTargets[Context.ConnectionId] = customerConnId;
            await Clients.Caller.SendAsync("CustomerSelected", customerConnId);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.User;

            string userName;
            string roleName = "Customer";

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

                if (role == "admin")
                {
                    roleName = "Employer";
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Support");
                }

                userName = user.FindFirstValue("FullName") ?? user.FindFirstValue(ClaimTypes.Email);
                userName = string.IsNullOrEmpty(userName) ? "Ẩn danh" : userName;
                userName = (roleName == "Employer" ? "Nhân viên " : "Khách hàng ") + userName;

                await Clients.Caller.SendAsync("SetUserRole", roleName);
            }
            else
            {
                userName = "Khách vãng lai";
            }

            _connections[Context.ConnectionId] = userName;

            if (roleName != "Employer")
            {
                _clientConnections[Context.ConnectionId] = userName;
                await UpdateAllAdminsCustomerList(); // 🆗 gửi ngay luôn
            }


            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);
            _clientConnections.TryRemove(Context.ConnectionId, out _);
            _supportChatTargets.TryRemove(Context.ConnectionId, out _);

            await UpdateAllAdminsCustomerList();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToCustomer(string customerConnId, string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var sender))
            {
                if (string.IsNullOrEmpty(customerConnId) || !_connections.ContainsKey(customerConnId))
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", "⚠️ Hệ thống", "Khách đã ngắt kết nối.");
                    return;
                }

                await Clients.Client(customerConnId).SendAsync("ReceiveMessage", sender, message);
            }
        }

        private async Task UpdateAllAdminsCustomerList()
        {
            await Clients.Group("Support").SendAsync("UpdateCustomerList",
              _clientConnections.Select(kv => new {
                  connectionId = kv.Key,
                  name = kv.Value
              })
.ToList());
        }
    }
}
