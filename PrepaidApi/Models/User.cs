using System.ComponentModel.DataAnnotations;

namespace PrepaidApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
