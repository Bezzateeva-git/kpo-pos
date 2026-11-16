using Pos.Application.Ports;
using Pos.DomainCore;

namespace Pos.Infrastructure.Repositories;

// Repository, хранение на время работы программы
public class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<string, Student> _students = new Dictionary<string, Student>();

    public void Add(Student student)
    {
        if (!_students.TryAdd(student.Id, student))
        {
            throw new ArgumentException("Студент с таким id уже существует");
        }
    }

    public Student? FindById(string id)
    {
        _students.TryGetValue(id, out Student? student);
        return student;
    }

    public IReadOnlyList<Student> GetAll()
    {
        return new List<Student>(_students.Values).AsReadOnly();
    }

    public void Save(Student student)
    {
        if (!_students.ContainsKey(student.Id))
        {
            throw new ArgumentException("Нельзя сохранить незарегистрированного студента");
        }
        _students[student.Id] = student;
    }
}
