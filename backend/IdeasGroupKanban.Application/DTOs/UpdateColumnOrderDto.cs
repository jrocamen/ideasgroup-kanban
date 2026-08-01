namespace IdeasGroupKanban.Application.DTOs;

public class UpdateColumnOrderDto
{
    public Guid ColumnId { get; set; }
    public int NewOrder { get; set; }
}
