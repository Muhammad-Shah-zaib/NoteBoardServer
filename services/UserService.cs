using System.Text;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Email;
using NoteBoardServer.repositories;

namespace NoteBoardServer.services;
public class UserService(NoteboardContext context) : IUserRepository
{
    public async Task<User?> CheckUserByIdAsync(int userId)
    {
        var existingUser = await context.Users.FindAsync(userId);
        return existingUser;
    }

    // GENERATE LOGIN EMAIL
    public MailDto GenerateLoginEmail(string receiverEmail, string username = "User")
    {
        var token = GenerateUniqueToken.GenerateToken();
        const string subject = "Login Verification from NoteBoard";
        var body = new StringBuilder();
        body.Append($"<h1>Hello, {username}</h1>");  // Added the username variable
        body.Append($"<p><strong><a href=\"http://localhost:5173/verify-login/{token}\">Click here to verify your login</a></strong></p>");

        return new MailDto()
        {
            Token = token,
            Username = username,
            ReceiverEmail = receiverEmail,
            Subject = subject,
            Body = body.ToString()
        };
    }

    // GENERATE REGISTER EMAIL
    public MailDto GenerateRegisterEmail(string receiverEmail, string username = "User")
    {
        var token = GenerateUniqueToken.GenerateToken();
        const string subject = "Email Verification from NoteBoard";
        var body = new StringBuilder();
        body.Append($"<h1>Hello {username}</h1>");
        body.Append($"<p><strong><a href=\"http://localhost:5173/verify-email/{token}\">Click here to verify your email</a></strong></p>");

        return new MailDto()
        {
            Token = token,
            Username = username,
            ReceiverEmail = receiverEmail,
            Subject = subject,
            Body = body.ToString()
        };
    }
}
