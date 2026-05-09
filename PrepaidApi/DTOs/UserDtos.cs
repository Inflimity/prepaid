using System.ComponentModel.DataAnnotations;

namespace PrepaidApi.DTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters.")]
        public string FullName { get; set; } = "String";

        [Required(ErrorMessage = "Phone Number is required.")]
        [Range(100000000, 999999999999999, ErrorMessage = "Phone number must be a valid numeric value.")]
        public long PhoneNumber { get; set; } = 12345678900;

        [Range(0, double.MaxValue, ErrorMessage = "Initial balance must be zero or positive.")]
        public int Balance { get; set; } = 0;
    }

    public class UpdateBalanceDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive number (no negatives or zero allowed).")]
        public int Amount { get; set; } = 0;
    }
}
