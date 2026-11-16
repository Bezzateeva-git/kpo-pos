namespace Pos.DomainCore;

// Entity, корень агрегата с оценками
public class Student
{
    private readonly Dictionary<string, Evaluation> _grades = new Dictionary<string, Evaluation>();

    public string FirstName { get; }
    public string SecondName { get; }
    public string Id { get; }

    public Student(string firstName, string secondName, string id)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(secondName)
            || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Имя, фамилия и id студента не должны быть пустыми");
        }
        FirstName = firstName.Trim();
        SecondName = secondName.Trim();
        Id = id.Trim();
    }

    // Повторная установка заменяет оценку по тому же критерию
    public void SetEvaluation(Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        _grades[evaluation.CriterionId] = evaluation;
    }

    public bool RemoveEvaluation(string criterionId)
    {
        return _grades.Remove(criterionId);
    }

    public IReadOnlyList<Evaluation> GetEvaluations()
    {
        return new List<Evaluation>(_grades.Values).AsReadOnly();
    }
}
