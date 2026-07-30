using IdeasGroupKanban.Domain.Entities;

namespace IdeasGroupKanban.Domain.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<KanbanTask>> GetByProjectIdAsync(Guid projectId);
    Task<KanbanTask?> GetByIdAsync(Guid id);
    Task<KanbanTask> AddAsync(KanbanTask task);
    Task UpdateAsync(KanbanTask task);
    Task DeleteAsync(Guid id);
}
