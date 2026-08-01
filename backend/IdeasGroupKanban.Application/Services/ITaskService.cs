using IdeasGroupKanban.Application.DTOs;

namespace IdeasGroupKanban.Application.Services;

public interface ITaskService
{
    Task<IEnumerable<KanbanTaskDto>> GetByProjectIdAsync(Guid projectId);
    Task<KanbanTaskDto> CreateAsync(CreateTaskDto taskDto);
    Task UpdateAsync(Guid id, CreateTaskDto taskDto);
    Task DeleteAsync(Guid id);
    Task MoveTaskAsync(MoveTaskDto moveDto);
}
