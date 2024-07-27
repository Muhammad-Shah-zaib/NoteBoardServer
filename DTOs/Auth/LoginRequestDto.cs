using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; } = string.Empty;
} 