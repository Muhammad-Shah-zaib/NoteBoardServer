namespace NoteBoardServer.Models.DTOs.Notes;

public class SingleNoteDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
}