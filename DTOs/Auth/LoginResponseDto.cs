namespace NoteBoardServer.Models.DTOs.Auth;

public class LoginResponseDto
{
    public bool Ok { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Error = new List<string>();
    public SingleUserDto? User { get; set; }
}