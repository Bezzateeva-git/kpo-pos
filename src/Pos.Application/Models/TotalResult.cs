namespace Pos.Application.Models;

// DTO для передачи итога в адаптер экспорта
public class TotalResult
{
    public string StudentId { get; }
    public string FirstName { get; }
    public string SecondName { get; }
    public double Score { get; }

    public TotalResult(string studentId, string firstName, string secondName, double score)
    {
        StudentId = studentId;
        FirstName = firstName;
        SecondName = secondName;
        Score = score;
    }
}
