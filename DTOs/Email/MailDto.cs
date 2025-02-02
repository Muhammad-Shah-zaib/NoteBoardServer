namespace NoteBoardServer.Models.DTOs.Email;

public class MailDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public string Subject {get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}