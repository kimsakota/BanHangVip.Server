using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHangVip.Backend.Models
{
    public enum OrderStatus { Pending, Delivered, Canceled }

    public class Order
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        public List<OrderItem> Items { get; set; } = new(); // Quan hệ 1-n
    }
}