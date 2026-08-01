using IdeasGroupKanban.Application.DTOs;

namespace IdeasGroupKanban.Application.Services;

public interface IProjectService
{
    Task<PaginatedResult<ProjectDto>> GetAllAsync(string? searchTerm = null, int pageNumber = 1, int pageSize = 10);
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto> CreateAsync(CreateProjectDto projectDto);
    Task UpdateAsync(Guid id, CreateProjectDto projectDto);
    Task DeleteAsync(Guid id);
}
