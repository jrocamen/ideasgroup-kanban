using IdeasGroupKanban.Application.DTOs;

namespace IdeasGroupKanban.Application.Services;

public interface IColumnService
{
    Task<IEnumerable<ColumnDto>> GetByProjectIdAsync(Guid projectId);
    Task<ColumnDto> CreateAsync(CreateColumnDto columnDto);
    Task UpdateAsync(Guid id, CreateColumnDto columnDto);
    Task DeleteAsync(Guid id);
    Task UpdateOrderAsync(List<UpdateColumnOrderDto> newOrder);
}
