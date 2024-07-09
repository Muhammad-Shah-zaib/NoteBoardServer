using System.Collections;

namespace NoteBoardServer.Models.DTOs.Notes;

public class DeleteResponseDto
{
    public bool Ok { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Error { get; set; } = new List<string>();
    public List<SingleNoteDto> Notes { get; set; } = new List<SingleNoteDto>();
}