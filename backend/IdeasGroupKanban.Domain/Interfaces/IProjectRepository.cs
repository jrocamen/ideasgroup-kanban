using IdeasGroupKanban.Domain.Entities;

namespace IdeasGroupKanban.Domain.Interfaces;

public interface IProjectRepository
{
    Task<(IEnumerable<Project> Items, int TotalCount)> GetAllAsync(string? searchTerm, int pageNumber, int pageSize);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
}
