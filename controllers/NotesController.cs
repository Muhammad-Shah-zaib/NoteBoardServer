using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.Models;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController (NoteBoardDbContext context): ControllerBase
{
    private readonly NoteBoardDbContext _context = context;
    
    [HttpGet]
    [Route("{userId:int}")]
    public async Task<IActionResult> GetNotes([FromRoute] int userId)
    {
        var notes = this._context.Notes;

        return Ok(
            await notes
                .Where(n => n.UserId == userId)
                .ToListAsync()
            );
    }
    
}