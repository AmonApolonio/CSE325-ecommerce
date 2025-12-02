
using System.ComponentModel.DataAnnotations;

public class Seller
{
    public long SellerId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string PasswordHash { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "Photo URL is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string PhotoUrl { get; set; } = "";

    public string AboutMe { get; set; } = "";

    [Required(ErrorMessage = "Address 1 is required")]
    [StringLength(150, ErrorMessage = "Address cannot exceed 150 characters")]
    public string Address1 { get; set; } = "";

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

    [Range(0, 100, ErrorMessage = "Commission rate must be between 0 and 100")]
    public double CommisionRate { get; set; }

    public List<string> Products { get; set; } = new();
}
