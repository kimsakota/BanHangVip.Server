using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHangVip.Backend.Models
{
    public enum OrderStatus { Pending, Delivered, Canceled }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        public List<OrderItem> Items { get; set; } = new(); // Quan hệ 1-n
    }
}