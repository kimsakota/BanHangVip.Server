using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHangVip.Backend.Models
{
    public class HistoryItem
    {
        [Key]
        public int Id { get; set; } 

        public string? Type { get; set; } // "INTAKE", "DELIVERY", "PAYMENT"

        public int? ProductId { get; set; } //
        [ForeignKey("ProductId")]
        public Product? Product { get; set; } //

        public string? ProductName { get; set; } //
        public double Weight { get; set; } //

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } //

        public DateTime Timestamp { get; set; } = DateTime.Now; //

        // Thuộc tính tính toán, không tạo cột trong Database
        [NotMapped]
        public decimal TotalAmount => (decimal)Weight * Price; //
    }
}