
using System.ComponentModel.DataAnnotations;

public class Client
{
    public long UserId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string PasswordHash { get; set; } = "";

    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "Address 1 is required")]
    [StringLength(150, ErrorMessage = "Address cannot exceed 150 characters")]
    public string Address1 { get; set; } = "";

    [StringLength(150, ErrorMessage = "Address cannot exceed 150 characters")]
    public string Address2 { get; set; } = "";

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string City { get; set; } = "";

    [Required(ErrorMessage = "State is required")]
    [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
    public string State { get; set; } = "";

    [Required(ErrorMessage = "Country is required")]
    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
    public string Country { get; set; } = "";

    [Required(ErrorMessage = "Zip Code is required")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Invalid Zip Code format (expected: 00000-000)")]
    public string ZipCode { get; set; } = "";

    public List<string> Carts { get; set; } = new();
}
