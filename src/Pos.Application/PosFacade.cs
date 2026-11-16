using Pos.Application.Models;
using Pos.Application.Ports;
using Pos.Application.UseCases;
using Pos.DomainCore;

namespace Pos.Application;

// Facade, единая точка сценариев для консоли
public class PosFacade
{
    private readonly IStudentRepository _students;
    private readonly Rubric _rubric;
    private readonly AddStudentUseCase _addStudent;
    private readonly SetEvaluationUseCase _setEvaluation;
    private readonly GetTotalUseCase _getTotal;
    private readonly ExportTotalsUseCase _exportTotals;

    public PosFacade(IStudentRepository students, Rubric rubric, AddStudentUseCase addStudent,
        SetEvaluationUseCase setEvaluation, GetTotalUseCase getTotal, ExportTotalsUseCase exportTotals)
    {
        _students = students;
        _rubric = rubric;
        _addStudent = addStudent;
        _setEvaluation = setEvaluation;
        _getTotal = getTotal;
        _exportTotals = exportTotals;
    }

    public void AddStudent(string firstName, string secondName, string id)
    {
        _addStudent.Execute(firstName, secondName, id);
    }

    public IReadOnlyList<Student> GetStudents()
    {
        return _students.GetAll();
    }

    public void AddCriterion(string name, string id, double max, double weight)
    {
        _rubric.AddCriterion(new Criterion(name, id, max, weight));
    }

    public IReadOnlyList<Criterion> GetCriteria()
    {
        return _rubric.GetCriteria();
    }

    public void SetEvaluation(string studentId, string criterionId, double value)
    {
        _setEvaluation.Execute(studentId, criterionId, value);
    }

    public IReadOnlyList<Evaluation> GetEvaluations(string studentId)
    {
        return GetStudent(studentId).GetEvaluations();
    }

    public void RemoveEvaluation(string studentId, string criterionId)
    {
        Student student = GetStudent(studentId);
        if (!student.RemoveEvaluation(criterionId))
        {
            throw new ArgumentException("У студента нет оценки по этому критерию");
        }
        _students.Save(student);
    }

    public TotalResult GetTotal(string studentId)
    {
        return _getTotal.Execute(studentId);
    }

    public void ExportTotals(string path)
    {
        _exportTotals.Execute(path);
    }

    private Student GetStudent(string id)
    {
        Student? student = _students.FindById(id);
        if (student == null)
        {
            throw new ArgumentException("Студент не найден", nameof(id));
        }
        return student;
    }
}
