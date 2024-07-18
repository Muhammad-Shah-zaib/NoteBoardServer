using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Auth;
using NoteBoardServer.services;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController(NoteboardContext context, EmailService emailService): ControllerBase
{
    private readonly NoteboardContext _context = context;
    private readonly EmailService _emailService = emailService;
    
    [HttpPost]
    [Route("Register")]
    // for registration
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
    {
        // NO NEED FOR VALIDATIONS SINCE THE VALIDATIONS ARE ALREADY DONe
        
        // check fpr email availability
        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Email == registrationRequestDto.Email);
        if (user != null) return Conflict(new RegistrationResponseDto()
        {
            Ok = false,
            Message = "User already exist with this Email",
            Error = [$"Email #{registrationRequestDto.Email} already exist"]
        });
        
        
        // check for username availability
        user = await this._context.Users.FirstOrDefaultAsync(u => u.Username == registrationRequestDto.Username);
        if (user != null) return Conflict(new RegistrationResponseDto()
        {
            Ok = false,
            Message = "User already exist with this Username",
            Error = [$"Username #{registrationRequestDto.Username} already exist"]
        });
        
        // SINCE THE USERNAME AND EMAIL IS NOT ALREADY TAKEN SO WE CAN GO WITH THE REGISTRATION
        var newUser = new User()
        {
            Username = registrationRequestDto.Username,
            Email = registrationRequestDto.Email,
            Firstname = registrationRequestDto.Firstname,
            Lastname = registrationRequestDto.Lastname,
            EmailVerified = false,
        };
        await this._context.Users.AddAsync(newUser);
        await this._context.SaveChangesAsync();

        return Ok(new RegistrationResponseDto()
        {
            Ok = true,
            Message = "Registration successful",
            Error = [],
            User = new SingleUserDto()
            {
                Id = newUser.Id,
                Firstname = newUser.Firstname,
                Lastname = newUser.Lastname,
                Username = newUser.Username,
                Email = newUser.Email,
            }
        });
    }
    
    [HttpGet]
    [Route("EmailTest")]
    // TESTING EMAIL
    public async Task<IActionResult> TestEmail()
    {
        const string subject = "TESTING";
        const string body = "<h1>Hello world, This is just a testing mail</h1>";
        await this._emailService.SendEmailAsync(email: "ali.hassam1@gmail.com",subject, body);
        return Ok("SUCCESS");
    }
    
}