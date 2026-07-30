namespace IdeasGroupKanban.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public ProjectState State { get; set; }
    
    // Navigation properties
    public ICollection<Column> Columns { get; set; } = new List<Column>();
}
