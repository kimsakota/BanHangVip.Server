using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHangVip.Backend.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Icon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultPrice { get; set; }
    }
}