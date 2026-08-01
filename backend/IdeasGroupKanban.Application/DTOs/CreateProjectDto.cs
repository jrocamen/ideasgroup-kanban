namespace IdeasGroupKanban.Application.DTOs;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public string? State { get; set; }
}
