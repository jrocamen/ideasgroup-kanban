using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Services;
using IdeasGroupKanban.Application.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeasGroupKanban.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IColumnService _columnService;
    private readonly ITaskService _taskService;
    private readonly IReportFactory _reportFactory;

    public ProjectsController(
        IProjectService projectService, 
        IColumnService columnService, 
        ITaskService taskService,
        IReportFactory reportFactory)
    {
        _projectService = projectService;
        _columnService = columnService;
        _taskService = taskService;
        _reportFactory = reportFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var result = await _projectService.GetAllAsync(searchTerm, page, size);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var project = await _projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProjectDto dto)
    {
        try
        {
            await _projectService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/export")]
    public async Task<IActionResult> Export(Guid id, [FromQuery] string format, [FromQuery] string? priority, [FromQuery] Guid? assigneeId)
    {
        try 
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound("Proyecto no encontrado");

            var columns = await _columnService.GetByProjectIdAsync(id);
            var tasks = await _taskService.GetByProjectIdAsync(id);

            if (!string.IsNullOrEmpty(priority))
            {
                tasks = tasks.Where(t => t.Priority == priority).ToList();
            }
            if (assigneeId.HasValue)
            {
                tasks = tasks.Where(t => t.AssigneeId == assigneeId.Value).ToList();
            }

            var strategy = _reportFactory.CreateStrategy(format);
            
            var fileBytes = strategy.GenerateReport(project, columns, tasks);
            var fileName = $"Reporte_{project.Name}_{DateTime.Now:yyyyMMdd}.{strategy.GetExtension()}";

            return File(fileBytes, strategy.GetContentType(), fileName);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
