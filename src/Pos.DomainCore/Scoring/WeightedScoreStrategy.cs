namespace Pos.DomainCore.Scoring;

// Strategy, пропущенная оценка считается нулем
public class WeightedScoreStrategy : IScoreStrategy
{
    public double Calculate(Student student, Rubric rubric)
    {
        IReadOnlyList<Criterion> criteria = rubric.GetCriteria();
        double maxWeight = 0;
        foreach (Criterion criterion in criteria)
        {
            if (criterion.Weight > maxWeight)
            {
                maxWeight = criterion.Weight;
            }
        }
        if (maxWeight == 0)
        {
            throw new InvalidOperationException("Для итога нужен хотя бы один критерий с положительным весом");
        }

        Dictionary<string, double> grades = new Dictionary<string, double>();
        foreach (Evaluation evaluation in student.GetEvaluations())
        {
            if (!rubric.TryGet(evaluation.CriterionId, out Criterion? criterion)
                || criterion == null || !criterion.CheckGrade(evaluation.Value))
            {
                throw new InvalidOperationException("Оценка не соответствует текущему рубрикатору");
            }
            grades.Add(evaluation.CriterionId, evaluation.Value);
        }

        double sum = 0;
        double weights = 0;
        foreach (Criterion criterion in criteria)
        {
            // Масштабирование сохраняет доли и защищает сумму весов от переполнения
            double weight = criterion.Weight / maxWeight;
            weights += weight;
            if (grades.TryGetValue(criterion.Id, out double value))
            {
                sum += value / criterion.MaxGrade * weight;
            }
        }
        return Math.Clamp(sum / weights * 10, 0, 10);
    }
}
