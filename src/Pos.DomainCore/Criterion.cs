namespace Pos.DomainCore;

// Entity
public class Criterion
{
    public string Name { get; }
    public string Id { get; }
    public double MaxGrade { get; }
    public double Weight { get; }

    public Criterion(string name, string id, double max, double weight)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Название и id критерия не должны быть пустыми");
        }
        if (!double.IsFinite(max) || max <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "Максимум должен быть положительным конечным числом");
        }
        if (!double.IsFinite(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть неотрицательным конечным числом");
        }
        Name = name.Trim();
        Id = id.Trim();
        MaxGrade = max;
        Weight = weight;
    }

    public bool CheckGrade(double grade)
    {
        return double.IsFinite(grade) && grade >= 0 && grade <= MaxGrade;
    }
}
