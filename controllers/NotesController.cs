using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Notes;
using NoteBoardServer.repositories;
using NoteBoardServer.services;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController (NoteboardContext context, IUserRepository userRepository): ControllerBase
{
    private readonly NoteboardContext _context = context;
    private readonly IUserRepository _userRepo = userRepository;
    
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

    [HttpPut]
    public async Task<IActionResult> UpdateNote([FromBody] UpdateNoteRequest updateNoteRequest)
    {   
        // validations
        var user = await _userRepo.CheckUserByIdAsync(updateNoteRequest.UserId);
        if (user == null) return NotFound("User not found");
        
        // since now the request is valid so we can update
        var currentNote = await this._context.Notes.FindAsync(updateNoteRequest.Id);
        if (currentNote == null) return NotFound("Current Note not found.");
        if (currentNote.UserId != updateNoteRequest.UserId) return Unauthorized($"User with id #{updateNoteRequest.Id} is authorized to access note with id #{updateNoteRequest.Id}");
        // we need to update the currentNote
        currentNote.Title = updateNoteRequest.Title;
        currentNote.Content = updateNoteRequest.Content;
        await this._context.SaveChangesAsync();
        return Ok($"Note with id #{updateNoteRequest.Id} has updated successfully");
    }

    [HttpGet]
    [Route("{noteId:int}")]
    public async Task<IActionResult> GetNoteById([FromRoute] int noteId, [FromQuery] int userId)
    {
        var note = await this._context.Notes.FindAsync(noteId);
        if (note == null) return NotFound($"Note with id #{noteId} not found.");
        if (note.UserId != userId) return Unauthorized($"User with id #{userId} has no access to note with id ${noteId}");
        return Ok(note);
    }
}