using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.Notes;
using NoteBoardServer.repositories;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController(NoteboardContext context, IUserRepository userRepository) : ControllerBase
{
    private readonly NoteboardContext _context = context;
    private readonly IUserRepository _userRepo = userRepository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SingleNoteDto>>> GetNotes([FromQuery] int userId)
    {
        var notes = this._context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Id);

        return Ok(await notes
            .Select(n => new SingleNoteDto()
            {
                Id = n.Id,
                Content = n.Content,
                Title = n.Title,
                UserId = n.UserId
            })
            .ToListAsync());
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
            if (user == null)
                return NotFound(new AddNoteResponseDto()
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
            var response = new AddNoteResponseDto()
            {
                StatusCode = 200,
                Ok = true,
                Message = $"Note with title {addNoteRequestDto.Title} has been added successfully",
                Note = newNote!, // if newNote was null then error would have been thrown
                Error = new List<string>()
            };
            return Ok(response);
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
    [Route("{noteId:int}")]
    public async Task<ActionResult<UpdateNoteResponseDto>> UpdateNote([FromRoute] int noteId, [FromQuery] int userId,
        [FromBody] UpdateNoteRequestDto updateNoteRequestDto)
    {
        // VALIDATING USER
        var user = await _userRepo.CheckUserByIdAsync(userId);
        if (user == null)
            return NotFound(new UpdateNoteResponseDto()
            {
                StatusCode = 404,
                Ok = false,
                Message = $"user with id #{userId} not Found.",
                Error = [$"user-id #{userId} is not valid"]
            });

        // VALIDATING NOTE
        var currentNote = await this._context.Notes.FindAsync(noteId);
        if (currentNote == null)
            return NotFound(new UpdateNoteResponseDto()
            {
                StatusCode = 404,
                Ok = false,
                Message = $"Note with id #{noteId} not found",
                Error = [$"note-id #{noteId} is not valid"]
            });

        // VALIDATING USER IS AUTHORIZED FOR THE GIVEN NOTE OR NOT
        if (currentNote.UserId != userId)
            return Unauthorized(new UpdateNoteResponseDto()
            {
                StatusCode = 401,
                Ok = false,
                Message = $"User with id #{noteId} is authorized to access note with id #{noteId}",
                Error = [$"No tuple found with user-id #{userId} and note-id #{noteId}"]
            });

        // UPDATING THE NOTE
        currentNote.Title = updateNoteRequestDto.Title;
        currentNote.Content = updateNoteRequestDto.Content;
        await this._context.SaveChangesAsync();
        
        // now we can fetch the notes again
        // RETURNING THE 200 RESPONSE
        var notes = await this._context.Notes
            .Where(n => n.UserId == userId)
            .Select(n => new SingleNoteDto()
            {
                Content = n.Content,
                Title = n.Title,
                Id = n.Id,
                UserId = n.UserId
            })
            .OrderByDescending(n=> n.Id)
            .ToListAsync();
        var response = new UpdateNoteResponseDto()
        {
            StatusCode = 200,
            Ok = true,
            Message = $"Note #{noteId} has been updated successfully",
            Note = new SingleNoteDto()
            {
                Id = noteId,
                UserId = userId,
                Content = updateNoteRequestDto.Content,
                Title = updateNoteRequestDto.Title,
            },
            Notes = notes,
            Error = new List<string>()
        };
        return Ok(response);
    }

    [HttpGet]
    [Route("{noteId:int}")]
    public async Task<ActionResult<GetSingleNoteResponseDto>> GetNoteById([FromRoute] int noteId,
        [FromQuery] int userId)
    {
        // GETTING THE NOTE
        var note = await this._context.Notes.FindAsync(noteId);

        // VALIDATING NOTE
        if (note == null)
            return NotFound(new GetSingleNoteResponseDto()
            {
                Ok = false,
                StatusCode = 404,
                Message = $"Note with id #{noteId} not found.",
                Error = [$"note-id #{noteId} is not valid."]
            });

        // VALIDATING USER ACCESS FOR NOTE
        if (note.UserId != userId)
            return Unauthorized(new GetSingleNoteResponseDto()
            {
                Ok = false,
                StatusCode = 401,
                Message = $"User with id #{userId} has no access to note with id ${noteId}",
                Error = [$"No tuple found with note-id #{noteId} and user-id #{userId}"]
            });

        // RETURNING RESPONSE
        var response = new GetSingleNoteResponseDto()
        {
            Ok = true,
            StatusCode = 200,
            Message = $"Note with id #{noteId} is fetched successfully",
            Error = [],
            Note = new SingleNoteDto()
            {
                Id = note.Id,
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
            }
        };
        return Ok(response);
    }

    [HttpDelete]
    [Route("{noteId:int}")]
    public async Task<ActionResult<DeleteResponseDto>> DeleteNote([FromRoute] int noteId, [FromQuery] int userId)
    {
        try
        {
            var note = await this._context.Notes.FindAsync(noteId);
        
            // VALIDATIONS
            if (note == null)
                return NotFound(new DeleteResponseDto()
                {
                    Ok = false,
                    StatusCode = 404,
                    Message = "Requested Note not found",
                    Error = [$"Note id #{noteId} is not valid"]
                });

            if (note.UserId != userId)
            {
                return Unauthorized(new DeleteResponseDto()
                {
                    Ok = false,
                    StatusCode = 401,
                    Message = "Requested use has no access to provided note",
                    Error = [$"User with id ${userId} has no access to note with id ${noteId}"]
                });
            }
        
            // NOW WE CAN DELETE THE NOTE
            this._context.Notes.Remove(note);
            await this._context.SaveChangesAsync();
            
            // GET THE UPDATED NOTES NOW
            var notes = await this._context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new SingleNoteDto()
                {
                    UserId = n.UserId,
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content
                })
                .ToListAsync();
            Console.WriteLine("asdfafda=> " + notes.Count);
            return Ok(new DeleteResponseDto()
            {
                Ok = true,
                Message = $"Note with id #{noteId} has been deleted successfully",
                StatusCode = 200,
                Notes = notes,
                Error = []
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(505, new DeleteResponseDto()
            {
                Ok = false,
                StatusCode = 505,
                Message = "Something went wrong, please try again later",
                Error = []
            });
        }
    }
}
