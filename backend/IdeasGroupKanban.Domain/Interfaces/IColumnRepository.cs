using IdeasGroupKanban.Domain.Entities;

namespace IdeasGroupKanban.Domain.Interfaces;

public interface IColumnRepository
{
    Task<IEnumerable<Column>> GetByProjectIdAsync(Guid projectId);
    Task<Column?> GetByIdAsync(Guid id);
    Task<Column> AddAsync(Column column);
    Task UpdateAsync(Column column);
    Task DeleteAsync(Guid id);
}
