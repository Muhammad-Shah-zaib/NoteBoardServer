namespace NoteBoardServer.Models.DTOs.Auth;

public class VerifyCredentialsResponseDto
{
    public bool Ok { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Error { get; set; } = new List<string>();
    public SingleUserDto UserDto { get; set; } = new SingleUserDto();
}