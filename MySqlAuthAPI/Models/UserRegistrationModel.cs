using System.ComponentModel.DataAnnotations;

namespace MySqlAuthAPI.Models
{
    public class UserRegistrationModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(maximumLength: 100, ErrorMessage = "First name should be upto 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(maximumLength: 100, ErrorMessage = "Last name should be upto 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(maximumLength:150,ErrorMessage = "Email Id should be upto 150 charcters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [StringLength(maximumLength: 10, ErrorMessage = "Phone number should be in 10 digits.")]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }

    }
}
