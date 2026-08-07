using FluentAssertions;
using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Services;
using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;
using Moq;
using Xunit;

namespace IdeasGroupKanban.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _projectRepoMock = new Mock<IProjectRepository>();
        _projectService = new ProjectService(_projectRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedProjects_WhenNoSearchTerm()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = Guid.NewGuid(), Name = "Project 1" },
            new Project { Id = Guid.NewGuid(), Name = "Project 2" }
        };
        _projectRepoMock.Setup(x => x.GetAllAsync(null, 1, 10)).ReturnsAsync((projects, 2));

        // Act
        var result = await _projectService.GetAllAsync(null, 1, 10);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items.First().Name.Should().Be("Project 1");
    }

    [Fact]
    public async Task CreateAsync_AddsProjectAndReturnsDto()
    {
        // Arrange
        var createDto = new CreateProjectDto { Name = "New Project", Description = "Desc", StartDate = DateTime.UtcNow };
        _projectRepoMock.Setup(x => x.AddAsync(It.IsAny<Project>())).ReturnsAsync(new Project { Id = Guid.NewGuid(), Name = "New Project" });

        // Act
        var result = await _projectService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Project");
        _projectRepoMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepoMock.Setup(x => x.DeleteAsync(projectId)).Returns(Task.CompletedTask);

        // Act
        await _projectService.DeleteAsync(projectId);

        // Assert
        _projectRepoMock.Verify(x => x.DeleteAsync(projectId), Times.Once);
    }
}
