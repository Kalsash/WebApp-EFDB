using System.ComponentModel.DataAnnotations;

namespace WebApp_Landing
{
    public class SubscribeModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}
