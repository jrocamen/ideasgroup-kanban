namespace IdeasGroupKanban.Application.DTOs;

public class CreateColumnDto
{
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}
