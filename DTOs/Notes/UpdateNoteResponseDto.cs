namespace NoteBoardServer.Models.DTOs.Notes;

public class UpdateNoteResponseDto
{
    public string Message { get; set; } = string.Empty;
    public bool Ok { get; set; }
    public int StatusCode { get; set; }
    public List<string> Error { get; set; } = new List<string>();
    public SingleNoteDto Note { get; set; } = new SingleNoteDto();
    public IEnumerable<SingleNoteDto> Notes { get; set; } = [];
}