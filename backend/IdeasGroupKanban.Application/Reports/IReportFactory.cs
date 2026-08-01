namespace IdeasGroupKanban.Application.Reports;

public interface IReportFactory
{
    IReportStrategy CreateStrategy(string format);
}
