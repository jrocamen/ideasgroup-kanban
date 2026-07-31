using IdeasGroupKanban.Application.DTOs;
using IdeasGroupKanban.Application.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IdeasGroupKanban.Infrastructure.Reports;

public class PdfReportStrategy : IReportStrategy
{
    public byte[] GenerateReport(ProjectDto project, IEnumerable<ColumnDto> columns, IEnumerable<KanbanTaskDto> tasks)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(header =>
                {
                    header.Text($"Reporte del Proyecto: {project.Name}").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);
                });

                page.Content().Element(content =>
                {
                    content.PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Item().Text($"Estado: {project.State}");
                        x.Item().Text($"Fecha de inicio: {project.StartDate:d}");
                        x.Item().Text($"Fecha fin estimada: {project.ExpectedEndDate:d}");
                        x.Item().PaddingBottom(1, Unit.Centimetre).Text(project.Description);

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columnsDefinition =>
                            {
                                columnsDefinition.RelativeColumn();
                                columnsDefinition.RelativeColumn();
                                columnsDefinition.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Tarea");
                                header.Cell().Element(CellStyle).Text("Columna");
                                header.Cell().Element(CellStyle).Text("Prioridad");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var task in tasks.OrderBy(t => t.Order)) // Assuming Order is what matters
                            {
                                var columnName = columns.FirstOrDefault(c => c.Id == task.ColumnId)?.Name ?? "Sin Columna";
                                table.Cell().Element(CellStyle).Text(task.Title);
                                table.Cell().Element(CellStyle).Text(columnName);
                                table.Cell().Element(CellStyle).Text(task.Priority.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public string GetContentType() => "application/pdf";
    public string GetExtension() => "pdf";
}
