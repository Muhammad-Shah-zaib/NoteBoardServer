using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Notes;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController (NoteBoardDbContext context): ControllerBase
{
    private readonly NoteBoardDbContext _context = context;
    
    [HttpGet]
    public async Task<IActionResult> GetNotes([FromQuery] int userId)
    {
        var notes = this._context.Notes;

        return Ok(
            await notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Id)
                .ToListAsync()
            );
    }

    [HttpPost]
    public async Task<IActionResult> AddNote([FromBody] AddNoteRequestDto addNoteRequestDto)
    {
        try
        {
            if (addNoteRequestDto.Content == string.Empty)
                return BadRequest(new AddNoteResponseDto()
                {
                    StatusCode = 400,
                    Ok = false,
                    Message = "Provided empty Content field is not valid",
                    Error = ["Content field is empty"]
                });
            // validation for userId
            var user = await this._context.Users.FindAsync(addNoteRequestDto.UserId);
            if (user == null) return NotFound(new AddNoteResponseDto()
            {
                StatusCode = 404,
                Ok = false,
                Message = $"User with id {addNoteRequestDto.UserId} not found.",
                Error = ["The userId provided is not correct since the user is not registered in db."]
            });
            await this._context.Notes.AddAsync(new Note()
            {
                Title = addNoteRequestDto.Title,
                Content = addNoteRequestDto.Content,
                UserId = addNoteRequestDto.UserId
            });
            await this._context.SaveChangesAsync();

            var newNote = await this._context.Notes
                .Where(n => n.UserId == addNoteRequestDto.UserId && n.Title == addNoteRequestDto.Title)
                .Select(n => new SingleNoteDto()
                {
                    Id = n.Id,
                    Content = n.Content,
                    Title = n.Title,
                    UserId = n.UserId
                })
                .FirstOrDefaultAsync();
            return Ok(new AddNoteResponseDto()
            {
                StatusCode = 200,
                Ok = true,
                Message = $"Note with title {addNoteRequestDto.Title} has been added successfully",
                Note = newNote!, // if newNote was null then error would have been thrown
                Error = new List<string>()
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new AddNoteResponseDto()
            {
                StatusCode = 500,
                Ok = false,
                Message = $"Something went wrong please try again later",
                Error = [e.ToString()],
            });
        }
    }
    
}