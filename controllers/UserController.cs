using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Auth;
using NoteBoardServer.Models.DTOs.Email;
using NoteBoardServer.repositories;
using NoteBoardServer.services;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class UserController(NoteboardContext context, EmailService emailService, IUserRepository userRepository): ControllerBase
{
    private readonly NoteboardContext _context = context;
    private readonly EmailService _emailService = emailService;
    private readonly IUserRepository _userRepository = userRepository;
    
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
        
        // getting the mail dto for the registration email
        var mailDto = this._userRepository.GenerateRegisterEmail(receiverEmail:newUser.Email, username: newUser.Username);
        
        // NOW WE NEED TO STORE THE TOKEN IN THE DB
        await this._context.AuthTokens.AddAsync(new AuthToken()
        {
            Token = mailDto.Token,
            UserId = newUser.Id,
            TokenType = TokenTypeEnum.EMAIL_VERIFICATION.ToString(),
        });
        await this._context.SaveChangesAsync();
        
        // NOW WE NEED TO SEND THE EMAIL
        await this._emailService.SendEmailAsync(mailDto);
        
        // RETURNING RESPONSE
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

    [HttpPost]
    [Route("VerifyEmail/{token}")]
    public async Task<IActionResult> VerifyEmail([FromRoute] string token)
    {
        // validate is the token true or not
        var authToken = await this._context.AuthTokens.FirstOrDefaultAsync(at => at.Token == token);
        if (authToken == null) return Unauthorized(new VerifyEmailResponseDto()
        {
            Ok = false,
            StatusCode = 401,
            Message = "Invalid token",
            Error = [$"No tuple found with token #{token}"],
        });
        
        // lets get the user
        var user = await this._context.Users.FindAsync(authToken.UserId);
        if (user == null) return NotFound(new VerifyEmailResponseDto()
        {
            Ok = false,
            StatusCode = 404,
            Message = "User not found",
            Error = [$"User with id #{authToken.UserId} not found"],
        });
        
        // verifying the email
        user.EmailVerified = true;
        await this._context.SaveChangesAsync();
        
        return Ok(new VerifyEmailResponseDto()
        {
            Ok = true,
            StatusCode = 200,
            Message = "Email verified successfully",
            Error = [],
            UserDto = new SingleUserDto()
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = (user.Lastname == null) ? user.Lastname : "",
                Username = user.Username,
                Email = user.Email,
            }
        });
    }
    
}