using ClosedXML.Excel;
using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Reports;

namespace IdeasGroupKanban.Infrastructure.Reports;

public class ExcelReportStrategy : IReportStrategy
{
    public byte[] GenerateReport(ProjectDto project, IEnumerable<ColumnDto> columns, IEnumerable<KanbanTaskDto> tasks)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Kanban Report");

        // Header
        worksheet.Cell(1, 1).Value = $"Proyecto: {project.Name}";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 3).Merge();

        // Project Info
        worksheet.Cell(3, 1).Value = "Estado:";
        worksheet.Cell(3, 2).Value = project.State;
        
        worksheet.Cell(4, 1).Value = "Fecha Inicio:";
        worksheet.Cell(4, 2).Value = project.StartDate.ToString("d");
        
        worksheet.Cell(5, 1).Value = "Fecha Fin:";
        worksheet.Cell(5, 2).Value = project.ExpectedEndDate.ToString("d");

        // Table Headers
        int row = 7;
        worksheet.Cell(row, 1).Value = "Tarea";
        worksheet.Cell(row, 2).Value = "Columna";
        worksheet.Cell(row, 3).Value = "Prioridad";
        var headerRange = worksheet.Range(row, 1, row, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        row++;

        foreach (var task in tasks.OrderBy(t => t.Order))
        {
            var columnName = columns.FirstOrDefault(c => c.Id == task.ColumnId)?.Name ?? "Sin Columna";
            worksheet.Cell(row, 1).Value = task.Title;
            worksheet.Cell(row, 2).Value = columnName;
            worksheet.Cell(row, 3).Value = task.Priority;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string GetExtension() => "xlsx";
}
