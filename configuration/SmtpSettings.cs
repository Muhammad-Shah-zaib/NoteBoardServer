namespace NoteBoardServer.configuration;

public class SmtpSettings
{
    public int SmtpPort { get; set; }
    public string SmtpServer { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}