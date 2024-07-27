namespace NoteBoardServer.Models.DTOs.whiteboard;

public class SingleWhiteboardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set;} = string.Empty;
    public int UserId { get; set; }
}