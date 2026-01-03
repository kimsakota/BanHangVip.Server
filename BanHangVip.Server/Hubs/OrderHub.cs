using Microsoft.AspNetCore.SignalR;

namespace BanHangVip.Server.Hubs
{
    public class OrderHub : Hub
    {
        // Hiện tại chúng ta chỉ cần class này để định nghĩa Hub.
        // Client sẽ lắng nghe, Server sẽ phát.
        // Nếu sau này Client cần gửi tin nhắn lên thì viết hàm tại đây.
    }
}
