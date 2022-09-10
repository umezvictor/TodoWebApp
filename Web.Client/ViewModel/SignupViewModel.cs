using System.ComponentModel.DataAnnotations;

namespace Web.Client.ViewModel
{
    public class SignupViewModel
    {

        [Required(ErrorMessage = "First name is required")]
        [DataType(DataType.Text)]
        [StringLength(30, MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        [StringLength(30, MinimumLength = 3)]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;      
        public string Password { get; set; } = string.Empty;
      
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string .Empty;
    }
}
