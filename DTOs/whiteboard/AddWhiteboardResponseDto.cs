namespace NoteBoardServer.Models.DTOs.whiteboard;

public class AddWhiteboardResponseDto
{
    public int StatusCode { get; set; }
    public bool Ok { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Error { get; set; } = new List<string>();
    public SingleWhiteboardDto Whiteboard { get; set; } = new SingleWhiteboardDto();
    public IEnumerable<SingleWhiteboardDto> Whiteboards { get; set; } = [new SingleWhiteboardDto()];
}