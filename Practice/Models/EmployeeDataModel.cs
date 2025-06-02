using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Practice.Models
{
    public class EmployeeDataModel
    {
        [JsonPropertyName("id")]
        [Required(ErrorMessage = "Employee No is required.")]
        [Range(minimum: 1, maximum: 1000, ErrorMessage = "Employee no should be between 1 to 1000.")]
        [DataType(DataType.Text)]
        [Display(Name = "Employee Id", Description = "Enter employee Id", Order = 1, ShortName = "Emp Id", GroupName = "Employee")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name is reqiured.")]
        [StringLength(maximumLength: 100, ErrorMessage = "Name should be in 100 characters.")]
        [DataType(dataType: DataType.Text)]
        [Display(Name = "Name", Description = "Enter employee name", Order = 2, ShortName = "Name", GroupName = "Employee")]
        public string Name { get; set; }

        [JsonPropertyName("phone")]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(maximumLength: 10, ErrorMessage = "Phone number should be in 10 digits.")]
        [DataType(dataType: DataType.PhoneNumber)]
        [Display(Name = "Phone Number", Description = "Enter employee phone number", Order = 3, ShortName = "Phone", GroupName = "Employee")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(maximumLength: 200, ErrorMessage = "Email should be in 200 characters.")]
        [DataType(dataType: DataType.EmailAddress)]
        [Display(Name = "Email Id", Description = "Enter employee email id ", Order = 4, ShortName = "Email", GroupName = "Employee")]
        public string Email { get; set; }
    }
}
