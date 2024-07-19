using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Email;

namespace NoteBoardServer.repositories;

public interface IUserRepository
{
    // CHECK IF USER EXISTS BY ID
    public Task<User?> CheckUserByIdAsync(int userId);
    
    // GENERATE REGISTER EMAIL WITH A UNIQUE TOKEN
    public MailDto GenerateRegisterEmailAsync(string receiverEmail, string username = "User");
}