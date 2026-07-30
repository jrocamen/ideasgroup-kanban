namespace IdeasGroupKanban.Domain.Entities;

public class KanbanTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid ColumnId { get; set; }
    public Guid AssigneeId { get; set; }

    // Navigation properties
    public Column Column { get; set; } = null!;
    public User Assignee { get; set; } = null!;
}
