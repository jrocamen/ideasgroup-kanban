namespace IdeasGroupKanban.Application.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public Guid ColumnId { get; set; }
    public Guid AssigneeId { get; set; }
}
