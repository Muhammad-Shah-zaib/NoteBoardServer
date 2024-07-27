namespace NoteBoardServer.Models.DTOs.Notes;

public class AddNoteResponseDto
{
    public int StatusCode { get; set; } 
    public bool Ok { get; set; }
    public string Message { get; set; } = string.Empty;
    public SingleNoteDto Note { get; set; } = new SingleNoteDto();
    public List<string> Error { get; set; } = new List<string>();
}