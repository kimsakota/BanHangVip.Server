using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BanHangVip.Backend.Models
{
    // Loại bỏ ObservableObject vì đây là Backend
    public class OrderItem
    {
        [Key]
        public int Id { get; set; } // PK nội bộ cho DB

        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public double Weight { get; set; }

        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal Total => (decimal)Weight * Price;

        // Foreign Key để link với Order
        public int OrderId { get; set; }

        [JsonIgnore] // Tránh vòng lặp khi serialize JSON
        public Order? Order { get; set; }
    }
}