using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBoardServer.models;

namespace NoteBoardServer.controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController: ControllerBase
{
    
    [HttpGet]
    public IActionResult GetNotes(NoteBoardDbContext context)
    {
        return Ok(context.Notes.ToList());
    }
}