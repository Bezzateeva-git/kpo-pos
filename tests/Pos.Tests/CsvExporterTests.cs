using Pos.Application.Models;
using Pos.Infrastructure.Export;
using Xunit;

namespace Pos.Tests;

// Интеграционный тест файлового адаптера
public class CsvExporterTests
{
    [Fact]
    public void ExportWritesHeaderEscapesTextAndFormatsScore()
    {
        string path = Path.GetTempFileName();
        try
        {
            List<TotalResult> results = new List<TotalResult>();
            results.Add(new TotalResult("s1", "Иван, Петр", "Ива\"нов", 8.5));
            new CsvTotalExporter().Export(results, path);
            string[] lines = File.ReadAllLines(path);
            Assert.Equal("StudentId,FirstName,SecondName,Score", lines[0]);
            Assert.Equal("\"s1\",\"Иван, Петр\",\"Ива\"\"нов\",8.50", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
