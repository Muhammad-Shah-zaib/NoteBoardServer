using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.Notes;

public class UpdateNoteRequest
{
    [Required] public int Id { get; set; }
    [Required] public string Content { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
    [Required] public int UserId { get; set; }
}