namespace IdeasGroupKanban.Application.DTOs;

public class ColumnDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid ProjectId { get; set; }
}
