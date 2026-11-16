using Pos.Application.Models;

namespace Pos.Application.Ports;

// Порт экспорта итогов
public interface ITotalExporter
{
    void Export(IReadOnlyList<TotalResult> results, string path);
}
