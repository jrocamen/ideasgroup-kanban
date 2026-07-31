using IdeasGroupKanban.Domain.Entities;
using IdeasGroupKanban.Domain.Interfaces;
using IdeasGroupKanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeasGroupKanban.Infrastructure.Repositories;

public class ColumnRepository : IColumnRepository
{
    private readonly ApplicationDbContext _context;

    public ColumnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Column>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Columns
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.Order)
            .ToListAsync();
    }

    public async Task<Column?> GetByIdAsync(Guid id)
    {
        return await _context.Columns
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Column> AddAsync(Column column)
    {
        _context.Columns.Add(column);
        await _context.SaveChangesAsync();
        return column;
    }

    public async Task UpdateAsync(Column column)
    {
        _context.Columns.Update(column);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var column = await GetByIdAsync(id);
        if (column != null)
        {
            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();
        }
    }
}
