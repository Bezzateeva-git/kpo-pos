using Pos.DomainCore;
using Pos.DomainCore.Scoring;
using Xunit;

namespace Pos.Tests;

// Тесты инвариантов и доменного сервиса
public class DomainTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void FactoryRejectsInvalidGrade(double value)
    {
        Rubric rubric = CreateRubric();
        Assert.Throws<ArgumentOutOfRangeException>(() => rubric.CreateEvaluation("exam", value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void FactoryAcceptsBoundaries(double value)
    {
        Evaluation evaluation = CreateRubric().CreateEvaluation("exam", value);
        Assert.Equal(value, evaluation.Value);
    }

    [Fact]
    public void FactoryRejectsUnknownCriterion()
    {
        Assert.Throws<ArgumentException>(() => CreateRubric().CreateEvaluation("unknown", 5));
    }

    [Fact]
    public void RubricRejectsDuplicateId()
    {
        Rubric rubric = CreateRubric();
        Assert.Throws<ArgumentException>(() => rubric.AddCriterion(new Criterion("Другой", "exam", 5, 1)));
        Assert.Single(rubric.GetCriteria());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(10, -1)]
    [InlineData(double.NaN, 1)]
    [InlineData(10, double.PositiveInfinity)]
    public void CriterionRejectsInvalidParameters(double max, double weight)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Criterion("Экзамен", "exam", max, weight));
    }

    [Theory]
    [InlineData("", "Иванов", "s1")]
    [InlineData("Иван", " ", "s1")]
    [InlineData("Иван", "Иванов", "")]
    public void StudentRejectsEmptyFields(string firstName, string secondName, string id)
    {
        Assert.Throws<ArgumentException>(() => new Student(firstName, secondName, id));
    }

    [Fact]
    public void EvaluationsHaveValueEquality()
    {
        Rubric rubric = CreateRubric();
        Evaluation first = rubric.CreateEvaluation("exam", 8);
        Evaluation second = rubric.CreateEvaluation("exam", 8);
        Assert.NotSame(first, second);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, rubric.CreateEvaluation("exam", 7));
    }

    [Fact]
    public void SettingAgainReplacesGradeAndRemovalChangesTotal()
    {
        Rubric rubric = CreateRubric();
        Student student = new Student("Иван", "Иванов", "s1");
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        student.SetEvaluation(rubric.CreateEvaluation("exam", 5));
        student.SetEvaluation(rubric.CreateEvaluation("exam", 8));
        Assert.Single(student.GetEvaluations());
        Assert.Equal(8, calculator.Calculate(student, rubric), 10);
        Assert.True(student.RemoveEvaluation("exam"));
        Assert.Empty(student.GetEvaluations());
        Assert.Equal(0, calculator.Calculate(student, rubric));
        Assert.False(student.RemoveEvaluation("exam"));
    }

    [Fact]
    public void WeightedTotalUsesMaximumsAndRelativeWeights()
    {
        Rubric rubric = CreateRubric();
        rubric.AddCriterion(new Criterion("Проект", "project", 20, 3));
        Student student = new Student("Иван", "Иванов", "s1");
        student.SetEvaluation(rubric.CreateEvaluation("exam", 8));
        student.SetEvaluation(rubric.CreateEvaluation("project", 10));
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        Assert.Equal(5.75, calculator.Calculate(student, rubric), 10);
    }

    [Fact]
    public void MissingGradeIsZeroAndNewCriterionChangesTotal()
    {
        Rubric rubric = CreateRubric();
        Student student = new Student("Иван", "Иванов", "s1");
        student.SetEvaluation(rubric.CreateEvaluation("exam", 8));
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        Assert.Equal(8, calculator.Calculate(student, rubric), 10);
        rubric.AddCriterion(new Criterion("Проект", "project", 10, 1));
        Assert.Equal(4, calculator.Calculate(student, rubric), 10);
    }

    [Fact]
    public void CompleteStrategyRequiresEveryGrade()
    {
        Rubric rubric = CreateRubric();
        Student student = new Student("Иван", "Иванов", "s1");
        TotalScoreCalculator calculator = new TotalScoreCalculator(
            new CompleteScoreStrategy(new WeightedScoreStrategy()));
        Assert.Throws<InvalidOperationException>(() => calculator.Calculate(student, rubric));
        student.SetEvaluation(rubric.CreateEvaluation("exam", 8));
        Assert.Equal(8, calculator.Calculate(student, rubric), 10);
    }

    [Fact]
    public void EmptyRubricAndZeroWeightsCannotProduceTotal()
    {
        Rubric rubric = new Rubric();
        Student student = new Student("Иван", "Иванов", "s1");
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        Assert.Throws<InvalidOperationException>(() => calculator.Calculate(student, rubric));
        rubric.AddCriterion(new Criterion("Проект", "project", 10, 0));
        Assert.Throws<InvalidOperationException>(() => calculator.Calculate(student, rubric));
    }

    [Fact]
    public void LargeWeightsDoNotOverflow()
    {
        Rubric rubric = new Rubric();
        rubric.AddCriterion(new Criterion("Первый", "a", 10, double.MaxValue));
        rubric.AddCriterion(new Criterion("Второй", "b", 10, double.MaxValue));
        Student student = new Student("Иван", "Иванов", "s1");
        student.SetEvaluation(rubric.CreateEvaluation("a", 10));
        student.SetEvaluation(rubric.CreateEvaluation("b", 10));
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        Assert.Equal(10, calculator.Calculate(student, rubric), 10);
    }

    private Rubric CreateRubric()
    {
        Rubric rubric = new Rubric();
        rubric.AddCriterion(new Criterion("Экзамен", "exam", 10, 1));
        return rubric;
    }
}
