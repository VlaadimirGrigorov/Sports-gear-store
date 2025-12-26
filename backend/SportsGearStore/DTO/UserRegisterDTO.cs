using System.ComponentModel.DataAnnotations;

namespace SportsGearStore.DTO
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(30, ErrorMessage = "Email cannot exceed 30 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Firstname is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required.")]
        public string LastName { get; set; }
    }
}
