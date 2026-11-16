using Pos.Application.Ports;
using Pos.DomainCore;

namespace Pos.Application.UseCases;

// Сценарий добавления студента
public class AddStudentUseCase
{
    private readonly IStudentRepository _students;

    public AddStudentUseCase(IStudentRepository students)
    {
        _students = students;
    }

    public void Execute(string firstName, string secondName, string id)
    {
        Student student = new Student(firstName, secondName, id);
        _students.Add(student);
    }
}
