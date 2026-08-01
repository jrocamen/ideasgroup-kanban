using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;

namespace IdeasGroupKanban.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<PaginatedResult<ProjectDto>> GetAllAsync(string? searchTerm = null, int pageNumber = 1, int pageSize = 10)
    {
        var (items, totalCount) = await _projectRepository.GetAllAsync(searchTerm, pageNumber, pageSize);

        var dtos = items.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            StartDate = p.StartDate,
            ExpectedEndDate = p.ExpectedEndDate,
            State = p.State.ToString()
        });

        return new PaginatedResult<ProjectDto>
        {
            Items = dtos,
            TotalCount = totalCount
        };
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) return null;

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            ExpectedEndDate = project.ExpectedEndDate,
            State = project.State.ToString()
        };
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto projectDto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = projectDto.Name,
            Description = projectDto.Description,
            StartDate = projectDto.StartDate,
            ExpectedEndDate = projectDto.ExpectedEndDate,
            State = ProjectState.NotStarted
        };

        await _projectRepository.AddAsync(project);

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            ExpectedEndDate = project.ExpectedEndDate,
            State = project.State.ToString()
        };
    }

    public async Task UpdateAsync(Guid id, CreateProjectDto projectDto)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) throw new KeyNotFoundException("Project not found");

        project.Name = projectDto.Name;
        project.Description = projectDto.Description;
        project.StartDate = projectDto.StartDate;
        project.ExpectedEndDate = projectDto.ExpectedEndDate;

        if (!string.IsNullOrEmpty(projectDto.State) && Enum.TryParse<ProjectState>(projectDto.State, out var parsedState))
        {
            project.State = parsedState;
        }

        await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _projectRepository.DeleteAsync(id);
    }
}
