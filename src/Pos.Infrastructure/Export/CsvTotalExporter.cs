using System.Globalization;
using System.Text;
using Pos.Application.Models;
using Pos.Application.Ports;

namespace Pos.Infrastructure.Export;

// Адаптер экспорта в CSV
public class CsvTotalExporter : ITotalExporter
{
    public void Export(IReadOnlyList<TotalResult> results, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Укажите путь к файлу", nameof(path));
        }
        using (StreamWriter writer = new StreamWriter(path, false, new UTF8Encoding(true)))
        {
            writer.WriteLine("StudentId,FirstName,SecondName,Score");
            foreach (TotalResult result in results)
            {
                writer.WriteLine(Escape(result.StudentId) + "," + Escape(result.FirstName) + ","
                    + Escape(result.SecondName) + "," + result.Score.ToString("F2", CultureInfo.InvariantCulture));
            }
        }
    }

    private string Escape(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
