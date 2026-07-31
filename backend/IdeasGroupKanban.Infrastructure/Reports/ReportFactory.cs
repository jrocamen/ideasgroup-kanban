using IdeasGroupKanban.Application.Reports;

namespace IdeasGroupKanban.Infrastructure.Reports;

public class ReportFactory : IReportFactory
{
    private readonly IEnumerable<IReportStrategy> _strategies;

    public ReportFactory(IEnumerable<IReportStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IReportStrategy CreateStrategy(string format)
    {
        return format.ToLower() switch
        {
            "pdf" => _strategies.OfType<PdfReportStrategy>().First(),
            "excel" => _strategies.OfType<ExcelReportStrategy>().First(),
            _ => throw new ArgumentException($"Formato no soportado: {format}")
        };
    }
}
