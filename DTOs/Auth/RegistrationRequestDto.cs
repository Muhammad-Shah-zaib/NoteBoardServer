using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.Auth;

public class RegistrationRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Firstname must be between 8 and 50 characters")]
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression(@"^[a-z0-9_]{8,50}$", ErrorMessage = "Username must be between 8 and 50 characters and can only contain lowercase letters, digits, and underscores")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; } = string.Empty;
}