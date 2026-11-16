using Pos.Application.Models;
using Pos.Application.Ports;
using Pos.DomainCore;

namespace Pos.Application.UseCases;

// Сценарий экспорта итогов
public class ExportTotalsUseCase
{
    private readonly IStudentRepository _students;
    private readonly GetTotalUseCase _getTotal;
    private readonly ITotalExporter _exporter;

    public ExportTotalsUseCase(IStudentRepository students, GetTotalUseCase getTotal, ITotalExporter exporter)
    {
        _students = students;
        _getTotal = getTotal;
        _exporter = exporter;
    }

    public void Execute(string path)
    {
        List<TotalResult> results = new List<TotalResult>();
        foreach (Student student in _students.GetAll())
        {
            results.Add(_getTotal.Execute(student.Id));
        }
        if (results.Count == 0)
        {
            throw new InvalidOperationException("Нет студентов для экспорта");
        }
        _exporter.Export(results, path);
    }
}
