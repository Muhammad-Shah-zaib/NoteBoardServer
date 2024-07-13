using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.whiteboard;

public class AddWhiteboardRequestDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string ImageUrl { get; set; } = string.Empty;
}