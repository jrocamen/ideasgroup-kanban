using IdeasGroupKanban.Application.DTOs;

namespace IdeasGroupKanban.Application.Reports;

public interface IReportStrategy
{
    byte[] GenerateReport(ProjectDto project, IEnumerable<ColumnDto> columns, IEnumerable<KanbanTaskDto> tasks);
    string GetContentType();
    string GetExtension();
}
