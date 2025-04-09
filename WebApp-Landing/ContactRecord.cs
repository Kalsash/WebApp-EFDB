using System.ComponentModel.DataAnnotations;

namespace WebApp_Landing
{
    public class ContactRecord
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(edu)$", ErrorMessage = "Email must be from a .edu domain.")]

        public string? ContactEmail { get; set; }
        public string? Topic { get; set; }
        public string? Message { get; set; }
    }
}
