using System.Text;
using Microsoft.AspNetCore.Mvc;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Email;
using NoteBoardServer.repositories;

namespace NoteBoardServer.services;

public class UserService(NoteboardContext context): IUserRepository
{
    private readonly NoteboardContext _context = context;
    public async Task<User?> CheckUserByIdAsync(int userId)
    {
        var existingUser = await this._context.Users.FindAsync(userId);
        return existingUser;
    }
    
    // GENERATE REGISTER EMAIL
    public MailDto GenerateRegisterEmailAsync(string receiverEmail, string username = "User")
    {
        var token = GenerateUniqueToken.GenerateToken();
        const string subject = "Email Verification from NoteBoard";
        var body = new StringBuilder();
        body.Append($"<h1>Hello {username}</h1>");
        body.Append($"<p><strong><a href=\"https://localhost:5173/verify-email/{token}\">Click here to verify your email</a></strong></p>");

        return new MailDto()
        {
            ReceiverEmail = receiverEmail,
            Subject = subject,
            Body = body.ToString()
        };
    }
}