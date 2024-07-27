using System.ComponentModel.DataAnnotations;

namespace NoteBoardServer.Models.DTOs.Notes;

public class UpdateNoteRequestDto
{
    [Required] public string Content { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
}