using System.ComponentModel.DataAnnotations;

namespace BanHangVip.Backend.Models
{
    public class Customer
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
    }
}