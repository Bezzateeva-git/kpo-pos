using Pos.DomainCore;

namespace Pos.Application.Ports;

// Repository, порт хранения студентов
public interface IStudentRepository
{
    void Add(Student student);
    Student? FindById(string id);
    IReadOnlyList<Student> GetAll();
    void Save(Student student);
}
