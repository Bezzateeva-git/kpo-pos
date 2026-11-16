using Pos.Application.Ports;
using Pos.DomainCore;

namespace Pos.Application.UseCases;

// Сценарий выставления и исправления оценки
public class SetEvaluationUseCase
{
    private readonly IStudentRepository _students;
    private readonly Rubric _rubric;

    public SetEvaluationUseCase(IStudentRepository students, Rubric rubric)
    {
        _students = students;
        _rubric = rubric;
    }

    public void Execute(string studentId, string criterionId, double value)
    {
        Student? student = _students.FindById(studentId);
        if (student == null)
        {
            throw new ArgumentException("Студент не найден", nameof(studentId));
        }
        Evaluation evaluation = _rubric.CreateEvaluation(criterionId, value);
        student.SetEvaluation(evaluation);
        _students.Save(student);
    }
}
