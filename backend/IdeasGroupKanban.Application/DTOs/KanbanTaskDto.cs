namespace IdeasGroupKanban.Application.DTOs;

public class KanbanTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ColumnId { get; set; }
    public Guid AssigneeId { get; set; }
}
