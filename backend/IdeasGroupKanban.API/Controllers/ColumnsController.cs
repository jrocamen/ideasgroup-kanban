using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Services;
using IdeasGroupKanban.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IdeasGroupKanban.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColumnsController : ControllerBase
{
    private readonly IColumnService _columnService;
    private readonly IHubContext<KanbanHub> _hubContext;

    public ColumnsController(IColumnService columnService, IHubContext<KanbanHub> hubContext)
    {
        _columnService = columnService;
        _hubContext = hubContext;
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId)
    {
        var columns = await _columnService.GetByProjectIdAsync(projectId);
        return Ok(columns);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateColumnDto dto)
    {
        try
        {
            var column = await _columnService.CreateAsync(dto);
            await _hubContext.Clients.All.SendAsync("ColumnCreated", column);
            return Ok(column);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Project not found");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateColumnDto dto)
    {
        try
        {
            await _columnService.UpdateAsync(id, dto);
            await _hubContext.Clients.All.SendAsync("ColumnUpdated", new { Id = id, Dto = dto });
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
        try
        {
            await _columnService.DeleteAsync(id);
            await _hubContext.Clients.All.SendAsync("ColumnDeleted", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("order")]
    public async Task<IActionResult> UpdateOrder([FromBody] List<UpdateColumnOrderDto> dto)
    {
        await _columnService.UpdateOrderAsync(dto);
        await _hubContext.Clients.All.SendAsync("ColumnOrderUpdated", dto);
        return NoContent();
    }
}
