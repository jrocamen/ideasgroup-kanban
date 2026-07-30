namespace IdeasGroupKanban.Domain.Entities;

public class Column
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    
    // Foreign Key
    public Guid ProjectId { get; set; }
    
    // Navigation properties
    public Project Project { get; set; } = null!;
    public ICollection<KanbanTask> Tasks { get; set; } = new List<KanbanTask>();
}
