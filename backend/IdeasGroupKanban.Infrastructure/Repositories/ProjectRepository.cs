using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;
using IdeasGroupKanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeasGroupKanban.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Project> Items, int TotalCount)> GetAllAsync(string? searchTerm, int pageNumber, int pageSize)
    {
        var query = _context.Projects.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        int totalCount = await query.CountAsync();
        
        var items = await query.OrderBy(p => p.Name)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
                               
        return (items, totalCount);
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await GetByIdAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
