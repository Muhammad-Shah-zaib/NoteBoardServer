namespace NoteBoardServer.Models.DTOs.Auth;

public class RegistrationResponseDto
{
    public int StatusCode { get; set; }
    public bool Ok { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Error { get; set; } = new List<string>();
    public SingleUserDto User { get; set; } = new SingleUserDto();
}