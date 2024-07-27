namespace NoteBoardServer.Models.DTOs.Auth;

public class VerifyEmailResponseDto
{
    public int StatusCode { get; set; } 
    public string Message { get; set; } = string.Empty;
    public bool Ok { get; set; } = false;
    public IEnumerable<string> Error { get; set; } = new List<string>();
    public SingleUserDto? UserDto { get; set; } = new SingleUserDto();
}
