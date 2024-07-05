namespace NoteBoardServer.Models.DTOs.Notes;

public class GetSingleNoteResponseDto
{
    public int StatusCode { get; set; }
    public bool Ok { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Error { get; set; } = new List<string>();
    public SingleNoteDto Note { get; set; } = new SingleNoteDto();
}