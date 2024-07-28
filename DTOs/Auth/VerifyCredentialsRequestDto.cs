using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.Auth;

public class VerifyCredentialsRequestDto
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Firstname is required")]
    public string Firstname { set; get; } = string.Empty;
    [Required(ErrorMessage = "Lastname is required")]
    public string Lastname { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; set; } = string.Empty;
}