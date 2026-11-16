using Pos.Application;
using Pos.Application.Models;
using Pos.Application.Ports;
using Pos.Application.UseCases;
using Pos.ConsoleUi;
using Pos.DomainCore;
using Pos.DomainCore.Scoring;
using Pos.Infrastructure.Repositories;
using Xunit;

namespace Pos.Tests;

// Тесты сценариев и консольного ввода
public class ApplicationTests
{
    [Fact]
    public void InvalidCorrectionKeepsOldEvaluation()
    {
        PosFacade facade = CreateFacade(new ExporterStub());
        facade.AddStudent("Иван", "Иванов", "s1");
        facade.AddCriterion("Экзамен", "exam", 10, 1);
        facade.SetEvaluation("s1", "exam", 8);
        Assert.Throws<ArgumentOutOfRangeException>(() => facade.SetEvaluation("s1", "exam", 12));
        Assert.Throws<ArgumentException>(() => facade.SetEvaluation("s1", "unknown", 5));
        Assert.Equal(8, facade.GetTotal("s1").Score, 10);
        Assert.Single(facade.GetEvaluations("s1"));
    }

    [Fact]
    public void DuplicateStudentAndUnknownStudentAreRejected()
    {
        PosFacade facade = CreateFacade(new ExporterStub());
        facade.AddStudent("Иван", "Иванов", "s1");
        Assert.Throws<ArgumentException>(() => facade.AddStudent("Петр", "Петров", "s1"));
        Assert.Throws<ArgumentException>(() => facade.SetEvaluation("unknown", "exam", 5));
        Assert.Throws<ArgumentException>(() => facade.GetTotal("unknown"));
        Assert.Single(facade.GetStudents());
    }

    [Fact]
    public void ExportReceivesCurrentTotal()
    {
        ExporterStub exporter = new ExporterStub();
        PosFacade facade = CreateFacade(exporter);
        facade.AddStudent("Иван", "Иванов", "s1");
        facade.AddCriterion("Экзамен", "exam", 10, 1);
        facade.SetEvaluation("s1", "exam", 8);
        facade.ExportTotals("result.csv");
        Assert.Equal("result.csv", exporter.Path);
        Assert.Single(exporter.Results);
        Assert.Equal(8, exporter.Results[0].Score, 10);
    }

    [Fact]
    public void EmptyExportIsRejected()
    {
        ExporterStub exporter = new ExporterStub();
        Assert.Throws<InvalidOperationException>(() => CreateFacade(exporter).ExportTotals("result.csv"));
        Assert.Null(exporter.Path);
    }

    [Fact]
    public void ConsoleRunsFullScenarioAndRecoversFromInvalidInput()
    {
        ExporterStub exporter = new ExporterStub();
        PosFacade facade = CreateFacade(exporter);
        string input = string.Join("\n", new string[]
        {
            "bad", "1", "s1", "Иван", "Иванов",
            "3", "exam", "Экзамен", "10", "1",
            "5", "s1", "exam", "abc", "8,5",
            "5", "s1", "exam", "12",
            "2", "4", "6", "s1", "8", "s1", "9", "result.csv", "0"
        });
        StringWriter output = new StringWriter();
        ConsoleApplication console = new ConsoleApplication(facade, new StringReader(input), output);
        console.Run();
        Assert.Contains("итог 8.50 из 10", output.ToString());
        Assert.Contains("Ошибка:", output.ToString());
        Assert.Contains("Итоги экспортированы", output.ToString());
        Assert.Single(exporter.Results);
        Assert.Equal(8.5, exporter.Results[0].Score, 10);
    }

    [Fact]
    public void ConsoleEndsOnEofDuringCommand()
    {
        StringWriter output = new StringWriter();
        ConsoleApplication console = new ConsoleApplication(
            CreateFacade(new ExporterStub()), new StringReader("1\ns1\n"), output);
        console.Run();
        Assert.Contains("Ввод завершен", output.ToString());
    }

    [Fact]
    public void CancelEvaluationUpdatesTotal()
    {
        PosFacade facade = CreateFacade(new ExporterStub());
        facade.AddStudent("Иван", "Иванов", "s1");
        facade.AddCriterion("Экзамен", "exam", 10, 1);
        facade.SetEvaluation("s1", "exam", 8);
        facade.RemoveEvaluation("s1", "exam");
        Assert.Equal(0, facade.GetTotal("s1").Score);
        Assert.Throws<ArgumentException>(() => facade.RemoveEvaluation("s1", "exam"));
    }

    private PosFacade CreateFacade(ITotalExporter exporter)
    {
        InMemoryStudentRepository students = new InMemoryStudentRepository();
        Rubric rubric = new Rubric();
        TotalScoreCalculator calculator = new TotalScoreCalculator(new WeightedScoreStrategy());
        AddStudentUseCase addStudent = new AddStudentUseCase(students);
        SetEvaluationUseCase setEvaluation = new SetEvaluationUseCase(students, rubric);
        GetTotalUseCase getTotal = new GetTotalUseCase(students, rubric, calculator);
        ExportTotalsUseCase exportTotals = new ExportTotalsUseCase(students, getTotal, exporter);
        return new PosFacade(students, rubric, addStudent, setEvaluation, getTotal, exportTotals);
    }

    // Заглушка экспорта, сохраняет аргументы вместо записи файла
    private class ExporterStub : ITotalExporter
    {
        public string? Path { get; private set; }
        public IReadOnlyList<TotalResult> Results { get; private set; } = new List<TotalResult>();

        public void Export(IReadOnlyList<TotalResult> results, string path)
        {
            Results = results;
            Path = path;
        }
    }
}
