using FluentAssertions;
using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Services;
using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;
using Moq;
using Xunit;

namespace IdeasGroupKanban.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IColumnRepository> _columnRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _columnRepoMock = new Mock<IColumnRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        
        _taskService = new TaskService(
            _taskRepoMock.Object, 
            _columnRepoMock.Object, 
            _userRepoMock.Object);
    }

    [Fact]
    public async Task MoveTaskAsync_CalculatesCorrectOrder_WhenMovedToSameColumn()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var columnId = Guid.NewGuid();
        var taskIdToMove = Guid.NewGuid();

        var column = new Column { Id = columnId, ProjectId = projectId };
        var task1 = new KanbanTask { Id = Guid.NewGuid(), ColumnId = columnId, Order = 0 };
        var task2 = new KanbanTask { Id = Guid.NewGuid(), ColumnId = columnId, Order = 1 };
        var task3ToMove = new KanbanTask { Id = taskIdToMove, ColumnId = columnId, Order = 2 };

        _taskRepoMock.Setup(x => x.GetByIdAsync(taskIdToMove)).ReturnsAsync(task3ToMove);
        _columnRepoMock.Setup(x => x.GetByIdAsync(columnId)).ReturnsAsync(column);
        _taskRepoMock.Setup(x => x.GetByProjectIdAsync(projectId))
                     .ReturnsAsync(new List<KanbanTask> { task1, task2, task3ToMove });

        var moveDto = new MoveTaskDto
        {
            TaskId = taskIdToMove,
            NewColumnId = columnId,
            NewOrder = 0 // Move task 3 to the top
        };

        // Act
        await _taskService.MoveTaskAsync(moveDto);

        // Assert
        task3ToMove.Order.Should().Be(0);
        task1.Order.Should().Be(1);
        task2.Order.Should().Be(2);

        _taskRepoMock.Verify(x => x.UpdateAsync(It.IsAny<KanbanTask>()), Times.Exactly(3));
    }

    [Fact]
    public async Task MoveTaskAsync_ThrowsKeyNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _taskRepoMock.Setup(x => x.GetByIdAsync(taskId)).ReturnsAsync((KanbanTask)null!);

        var moveDto = new MoveTaskDto { TaskId = taskId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.MoveTaskAsync(moveDto));
    }
}
