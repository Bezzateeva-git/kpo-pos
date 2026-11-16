using Pos.Application.Models;
using Pos.Application.Ports;
using Pos.DomainCore;
using Pos.DomainCore.Scoring;

namespace Pos.Application.UseCases;

// Сценарий расчета итога
public class GetTotalUseCase
{
    private readonly IStudentRepository _students;
    private readonly Rubric _rubric;
    private readonly TotalScoreCalculator _calculator;

    public GetTotalUseCase(IStudentRepository students, Rubric rubric, TotalScoreCalculator calculator)
    {
        _students = students;
        _rubric = rubric;
        _calculator = calculator;
    }

    public TotalResult Execute(string studentId)
    {
        Student? student = _students.FindById(studentId);
        if (student == null)
        {
            throw new ArgumentException("Студент не найден", nameof(studentId));
        }
        double score = _calculator.Calculate(student, _rubric);
        return new TotalResult(student.Id, student.FirstName, student.SecondName, score);
    }
}
