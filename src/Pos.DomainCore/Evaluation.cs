namespace Pos.DomainCore;

// Value Object
public sealed class Evaluation : IEquatable<Evaluation>
{
    public string CriterionId { get; }
    public double Value { get; }

    internal Evaluation(string id, double value)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id критерия не должен быть пустым", nameof(id));
        }
        if (!double.IsFinite(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        CriterionId = id;
        Value = value;
    }

    public bool Equals(Evaluation? other)
    {
        return other != null && CriterionId == other.CriterionId && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Evaluation);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CriterionId, Value);
    }
}
