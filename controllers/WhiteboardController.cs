using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;
using NoteBoardServer.Models.DTOs.whiteboard;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class WhiteboardController(NoteboardContext context): ControllerBase
{
    private readonly NoteboardContext _context = context;
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SingleWhiteboardDto>>> GetWhiteboardByUserId([FromQuery] int userId)
    {
        try
        {
            var whiteboards = await this._context.Whitboards
                .Where(wb => wb.UserId == userId)
                .OrderByDescending(wb => wb.Id)
                .ToListAsync();

            return Ok(
                whiteboards.Select(wb => new SingleWhiteboardDto()
                    {
                        Id = wb.Id,
                        UserId = wb.UserId,
                        ImageUrl = wb.ImageUrl,
                        Title = wb.Title
                    })
                );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Server error, please try again later");
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddWhiteboardResponseDto>> AddWhiteboard([FromQuery] int userId, [FromBody] AddWhiteboardRequestDto addWhiteboardRequestDto) 
    {
        var whiteboard = new Whitboard()
        {
            Title = addWhiteboardRequestDto.Title,
            ImageUrl = addWhiteboardRequestDto.ImageUrl,
            UserId = userId
        };
        try
        {
            // model state will be valid as there are data annotations on the fields
            // now we will add this whiteboard
            await this._context.Whitboards
                .AddAsync(whiteboard);
            await this._context.SaveChangesAsync();
            
            // GETTING THE WHITEBOARDS FOR REDUCING NETWORK TRAFFIC
            var whiteboards = await  this._context.Whitboards
                .Where(wb => wb.UserId == userId)
                .OrderByDescending(wb => wb.Id)
                .Select(wb => new SingleWhiteboardDto()
                {
                    Id = wb.Id,
                    Title = wb.Title,
                    ImageUrl = wb.ImageUrl,
                    UserId = wb.UserId
                }).ToListAsync();
            // RETURNING THE RESPONSE
            return Ok(new AddWhiteboardResponseDto()
            {
                StatusCode = 200,
                Ok = true,
                Message = "Whiteboard added successfully",
                Whiteboard = new SingleWhiteboardDto()
                {
                    Id = whiteboard.Id,
                    Title = whiteboard.Title,
                    ImageUrl = whiteboard.ImageUrl,
                    UserId = whiteboard.UserId
                },
                Whiteboards = whiteboards
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new AddWhiteboardResponseDto()
            {
                StatusCode = 500,
                Ok = false,
                Message = "Something went wrong, please try again later",
                Error = [ e.ToString() ]
            });
        }
    } 
}

