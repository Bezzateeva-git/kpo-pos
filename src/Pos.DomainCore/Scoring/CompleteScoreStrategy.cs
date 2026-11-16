namespace Pos.DomainCore.Scoring;

// Strategy, итог доступен только после выставления всех оценок
public class CompleteScoreStrategy : IScoreStrategy
{
    private readonly WeightedScoreStrategy _weightedStrategy;

    public CompleteScoreStrategy(WeightedScoreStrategy weightedStrategy)
    {
        _weightedStrategy = weightedStrategy;
    }

    public double Calculate(Student student, Rubric rubric)
    {
        HashSet<string> evaluated = new HashSet<string>();
        foreach (Evaluation evaluation in student.GetEvaluations())
        {
            evaluated.Add(evaluation.CriterionId);
        }
        foreach (Criterion criterion in rubric.GetCriteria())
        {
            if (!evaluated.Contains(criterion.Id))
            {
                throw new InvalidOperationException("Не по всем критериям выставлены оценки");
            }
        }
        return _weightedStrategy.Calculate(student, rubric);
    }
}
