namespace IdeasGroupKanban.Application.DTOs;

public class MoveTaskDto
{
    public Guid TaskId { get; set; }
    public Guid NewColumnId { get; set; }
    public int NewOrder { get; set; }
}
