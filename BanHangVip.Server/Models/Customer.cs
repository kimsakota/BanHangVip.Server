using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BanHangVip.Backend.Models
{
    public class Customer
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new();
    }
}