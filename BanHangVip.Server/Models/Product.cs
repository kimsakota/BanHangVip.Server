using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BanHangVip.Backend.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Icon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultPrice { get; set; }

        [JsonIgnore]
        public List<HistoryItem> HistoryItems { get; set; } = new();
    }
}