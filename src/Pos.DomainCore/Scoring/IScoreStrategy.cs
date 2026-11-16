namespace Pos.DomainCore.Scoring;

// Strategy
public interface IScoreStrategy
{
    double Calculate(Student student, Rubric rubric);
}
