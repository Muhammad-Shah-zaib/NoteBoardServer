using Microsoft.AspNetCore.Mvc;
using NoteBoardServer.Models;
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
}