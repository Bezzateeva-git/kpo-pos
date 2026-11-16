namespace Pos.DomainCore.Scoring;

// Domain Service
public class TotalScoreCalculator
{
    private readonly IScoreStrategy _strategy;

    public TotalScoreCalculator(IScoreStrategy strategy)
    {
        _strategy = strategy;
    }

    public double Calculate(Student student, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);
        return _strategy.Calculate(student, rubric);
    }
}
