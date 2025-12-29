using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BanHangVip.Backend.Models
{
    // Định nghĩa loại hình bán
    public enum SaleType
    {
        Weight = 0,     // Mặc định: Nhập Kg (Ngao, Sò, Ốc...)
        Quantity = 1,   // Theo con: Nhập số con (Cua - chưa biết cân)
        Separated = 2   // Tách dòng: Nhập ước lượng kg cho 1 con (Cá - bán từng con)
    }
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Icon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultPrice { get; set; }

        public SaleType SaleType { get; set; } = SaleType.Weight;

        public string? Preparation { get; set; }

        [JsonIgnore]
        public List<HistoryItem> HistoryItems { get; set; } = new();
    }
}