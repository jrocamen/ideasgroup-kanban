using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;

namespace IdeasGroupKanban.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IColumnRepository _columnRepository;
    private readonly IUserRepository _userRepository;

    public TaskService(ITaskRepository taskRepository, IColumnRepository columnRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _columnRepository = columnRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<KanbanTaskDto>> GetByProjectIdAsync(Guid projectId)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
        return tasks.Select(t => new KanbanTaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority.ToString(),
            Order = t.Order,
            CreatedAt = t.CreatedAt,
            ColumnId = t.ColumnId,
            AssigneeId = t.AssigneeId
        });
    }

    public async Task<KanbanTaskDto> CreateAsync(CreateTaskDto taskDto)
    {
        var column = await _columnRepository.GetByIdAsync(taskDto.ColumnId);
        if (column == null) throw new KeyNotFoundException("Column not found");

        var assignee = await _userRepository.GetByIdAsync(taskDto.AssigneeId);
        if (assignee == null) throw new KeyNotFoundException("Assignee not found");

        if (!Enum.TryParse<TaskPriority>(taskDto.Priority, true, out var priorityEnum))
        {
            priorityEnum = TaskPriority.Medium;
        }

        var existingTasks = await _taskRepository.GetByProjectIdAsync(column.ProjectId);
        int nextOrder = existingTasks.Where(t => t.ColumnId == taskDto.ColumnId).Any() 
            ? existingTasks.Where(t => t.ColumnId == taskDto.ColumnId).Max(t => t.Order) + 1 
            : 0;

        var task = new KanbanTask
        {
            Id = Guid.NewGuid(),
            Title = taskDto.Title,
            Description = taskDto.Description,
            Priority = priorityEnum,
            ColumnId = taskDto.ColumnId,
            AssigneeId = taskDto.AssigneeId,
            Order = nextOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);

        return new KanbanTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            Order = task.Order,
            CreatedAt = task.CreatedAt,
            ColumnId = task.ColumnId,
            AssigneeId = task.AssigneeId
        };
    }

    public async Task UpdateAsync(Guid id, CreateTaskDto taskDto)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Task not found");

        var assignee = await _userRepository.GetByIdAsync(taskDto.AssigneeId);
        if (assignee == null) throw new KeyNotFoundException("Assignee not found");

        if (Enum.TryParse<TaskPriority>(taskDto.Priority, true, out var priorityEnum))
        {
            task.Priority = priorityEnum;
        }

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.AssigneeId = taskDto.AssigneeId;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _taskRepository.DeleteAsync(id);
    }

    public async Task MoveTaskAsync(MoveTaskDto moveDto)
    {
        var task = await _taskRepository.GetByIdAsync(moveDto.TaskId);
        if (task == null) throw new KeyNotFoundException("Task not found");

        var newColumn = await _columnRepository.GetByIdAsync(moveDto.NewColumnId);
        if (newColumn == null) throw new KeyNotFoundException("Target column not found");

        var allTasks = await _taskRepository.GetByProjectIdAsync(newColumn.ProjectId);
        var targetColumnTasks = allTasks.Where(t => t.ColumnId == moveDto.NewColumnId && t.Id != task.Id).OrderBy(t => t.Order).ToList();

        task.ColumnId = moveDto.NewColumnId;

        // Ensure NewOrder is within bounds
        int insertIndex = moveDto.NewOrder;
        if (insertIndex < 0) insertIndex = 0;
        if (insertIndex > targetColumnTasks.Count) insertIndex = targetColumnTasks.Count;

        targetColumnTasks.Insert(insertIndex, task);

        // Update orders
        for (int i = 0; i < targetColumnTasks.Count; i++)
        {
            targetColumnTasks[i].Order = i;
            await _taskRepository.UpdateAsync(targetColumnTasks[i]);
        }
    }
}
