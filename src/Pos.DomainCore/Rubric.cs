namespace Pos.DomainCore;

// Aggregate, фабрика оценок
public class Rubric
{
    private readonly Dictionary<string, Criterion> _criteria = new Dictionary<string, Criterion>();

    public void AddCriterion(Criterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);
        if (!_criteria.TryAdd(criterion.Id, criterion))
        {
            throw new ArgumentException("Критерий с таким id уже существует");
        }
    }

    public bool TryGet(string id, out Criterion? criterion)
    {
        return _criteria.TryGetValue(id, out criterion);
    }

    public IReadOnlyList<Criterion> GetCriteria()
    {
        return new List<Criterion>(_criteria.Values).AsReadOnly();
    }

    public Evaluation CreateEvaluation(string id, double value)
    {
        if (!TryGet(id, out Criterion? criterion) || criterion == null)
        {
            throw new ArgumentException("Критерий не найден в рубрикаторе", nameof(id));
        }
        if (!criterion.CheckGrade(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Балл выходит за допустимый диапазон");
        }
        return new Evaluation(criterion.Id, value);
    }
}
