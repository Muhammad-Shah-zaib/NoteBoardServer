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
        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Email == registrationRequestDto.Email.ToUpper());
        if (user != null) return Conflict(new RegistrationResponseDto()
        {
            StatusCode = 409,
            Ok = false,
            Message = "User already exist with this Email",
            Error = [$"Email #{registrationRequestDto.Email} already exist"]
        });
        
        
        // check for username availability
        user = await this._context.Users.FirstOrDefaultAsync(u => u.Username.ToUpper() == registrationRequestDto.Username.ToUpper());
        if (user != null) return Conflict(new RegistrationResponseDto()
        {
            StatusCode = 409,
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
            StatusCode = 200,
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

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        try
        {
            // NO NEED FOR MODEL-STATE-VALIDATIONS SINCE THE VALIDATIONS ARE ALREADY DONE

            // check for the user
            var user = await this._context.Users.FirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);
            if (user == null)
                return NotFound(new LoginResponseDto()
                {
                    Ok = false,
                    StatusCode = 404,
                    Message = "User not found",
                    Error = [$"User with email #{loginRequestDto.Email} not found"],
                });
            if (!user.EmailVerified)
                return StatusCode(403, new LoginResponseDto()
                {
                    Ok = false,
                    StatusCode = 403,
                    Message = "Email not verified, please verify your email",
                    Error = [
                        "Forbidden", 
                        $"Email #{loginRequestDto.Email} not verified"
                    ],
                });
            // now we have the user, so we need to send the login request to the email
            var mailDto = this._userRepository.GenerateLoginEmail(receiverEmail: user.Email, username: user.Username);
            
            // before storing the new token we need to remove if there is any token
            var authToken = await this._context.AuthTokens.FirstOrDefaultAsync(at => at.UserId == user.Id && at.TokenType == TokenTypeEnum.LOGIN_VERIFICATION.ToString());
            if (authToken != null) this._context.AuthTokens.Remove(authToken);
            
            // we need to safe the token in the db
            await this._context.AuthTokens.AddAsync(new AuthToken()
            {
                Token = mailDto.Token,
                UserId = user.Id,
                TokenType = TokenTypeEnum.LOGIN_VERIFICATION.ToString(),
            });
            await this._context.SaveChangesAsync();
            // sending email
            await this._emailService.SendEmailAsync(mailDto);

            return Ok(new LoginResponseDto()
            {
                Ok = true,
                StatusCode = 200,
                Message = "Login email sent successfully",
                Error = [],
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500, new LoginResponseDto()
            {
                Ok = false,
                StatusCode = 500,
                Message = "Internal server error",
                Error = [$"Internal server error: {e.Message}"],
            });
        }
    }

    [HttpGet]
    [Route("VerifyLoginToken/{token}")]
    public async Task<IActionResult> VerifyLoginToken([FromRoute] string token)
    {
        try
        {
            var authToken = await this._context.AuthTokens.Include(u => u.User).FirstOrDefaultAsync(at => at.Token == token);
            if (authToken == null) return NotFound(new LoginResponseDto()
            {
                Ok = false,
                StatusCode = 404,
                Message = "Invalid token",
                Error = [$"Token #{token} not found"],
            });
            
            // now since we have the token we can send the login true response
            return Ok(new LoginResponseDto()
            {
                Ok = true,
                StatusCode = 200,
                Message = "Login successful",
                Error = [],
                User = new SingleUserDto()
                {
                    Id = authToken.User.Id,
                    Firstname = authToken.User.Firstname,
                    Lastname = authToken.User.Lastname,
                    Username = authToken.User.Username,
                    Email = authToken.User.Email,
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
    }
}














