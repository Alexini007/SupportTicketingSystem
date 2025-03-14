using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(5, ErrorMessage = "Password must be at least 5 characters long.")]
    [RegularExpression(@"^[A-Za-z](?=.*\d)(?=.*[A-Za-z])[A-Za-z\d\W_]{4,}$", ErrorMessage = "Password must start with a letter, contain at least one number, and be at least 5 characters long.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Team is required.")]
    [RegularExpression(@"^(Development|Support|Sales)$", ErrorMessage = "Invalid team. Choose Development, Support, or Sales.")]
    public string Team { get; set; }
}
