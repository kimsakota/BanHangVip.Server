using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHangVip.Backend.Models
{
    public class HistoryItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Tự sinh ID nếu client không gửi

        public string Type { get; set; } // "INTAKE", "DELIVERY", "PAYMENT"
        public string ProductName { get; set; } //
        public double Weight { get; set; } //

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } //

        public DateTime Timestamp { get; set; } = DateTime.Now; //

        // Thuộc tính tính toán, không tạo cột trong Database
        [NotMapped]
        public decimal TotalAmount => (decimal)Weight * Price; //
    }
}