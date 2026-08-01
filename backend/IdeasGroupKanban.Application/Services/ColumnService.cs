using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;

namespace IdeasGroupKanban.Application.Services;

public class ColumnService : IColumnService
{
    private readonly IColumnRepository _columnRepository;
    private readonly IProjectRepository _projectRepository;

    public ColumnService(IColumnRepository columnRepository, IProjectRepository projectRepository)
    {
        _columnRepository = columnRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ColumnDto>> GetByProjectIdAsync(Guid projectId)
    {
        var columns = await _columnRepository.GetByProjectIdAsync(projectId);
        return columns.Select(c => new ColumnDto
        {
            Id = c.Id,
            Name = c.Name,
            Order = c.Order,
            ProjectId = c.ProjectId
        });
    }

    public async Task<ColumnDto> CreateAsync(CreateColumnDto columnDto)
    {
        var project = await _projectRepository.GetByIdAsync(columnDto.ProjectId);
        if (project == null) throw new KeyNotFoundException("Project not found");

        var existingColumns = await _columnRepository.GetByProjectIdAsync(columnDto.ProjectId);
        int nextOrder = existingColumns.Any() ? existingColumns.Max(c => c.Order) + 1 : 0;

        var column = new Column
        {
            Id = Guid.NewGuid(),
            Name = columnDto.Name,
            ProjectId = columnDto.ProjectId,
            Order = nextOrder
        };

        await _columnRepository.AddAsync(column);

        return new ColumnDto
        {
            Id = column.Id,
            Name = column.Name,
            Order = column.Order,
            ProjectId = column.ProjectId
        };
    }

    public async Task UpdateAsync(Guid id, CreateColumnDto columnDto)
    {
        var column = await _columnRepository.GetByIdAsync(id);
        if (column == null) throw new KeyNotFoundException("Column not found");

        column.Name = columnDto.Name;
        await _columnRepository.UpdateAsync(column);
    }

    public async Task DeleteAsync(Guid id)
    {
        var column = await _columnRepository.GetByIdAsync(id);
        if (column == null) return;

        // Regla de negocio aplicada en el backend: no se permite eliminar una columna que contenga tareas
        if (column.Tasks.Any())
        {
            throw new InvalidOperationException("Cannot delete a column that contains tasks.");
        }

        await _columnRepository.DeleteAsync(id);
    }

    public async Task UpdateOrderAsync(List<UpdateColumnOrderDto> newOrder)
    {
        foreach (var item in newOrder)
        {
            var column = await _columnRepository.GetByIdAsync(item.ColumnId);
            if (column != null)
            {
                column.Order = item.NewOrder;
                await _columnRepository.UpdateAsync(column);
            }
        }
    }
}
