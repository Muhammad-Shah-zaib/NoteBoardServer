using NoteBoardServer.Models;

namespace NoteBoardServer.repositories;

public interface IUserRepository
{
    public Task<User?> CheckUserByIdAsync(int userId);
}