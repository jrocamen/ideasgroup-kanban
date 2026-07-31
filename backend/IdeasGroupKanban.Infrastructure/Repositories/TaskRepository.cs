using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;
using IdeasGroupKanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeasGroupKanban.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<KanbanTask>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Tasks
            .Include(t => t.Column)
            .Include(t => t.Assignee)
            .Where(t => t.Column.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task<KanbanTask?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.Column)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<KanbanTask> AddAsync(KanbanTask task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(KanbanTask task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await GetByIdAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
