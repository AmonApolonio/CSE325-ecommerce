
using System.ComponentModel.DataAnnotations; // <--- Required for [Required] to work

namespace cse325_ecommerce.Shared // <--- Must be the same name you used when creating the Class Library
{
    public class LoginModel
    {
        [Required(ErrorMessage = "The Username field is required.")]
        public string Usuario { get; set; } = string.Empty; // Initializes empty to avoid null warnings

        [Required(ErrorMessage = "The password is required.")]
        [MinLength(3, ErrorMessage = "The password must have at least 3 characters.")]
        public string Senha { get; set; } = string.Empty;
    }
}
