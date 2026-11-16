using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application;
using Pos.Application.Ports;
using Pos.Application.UseCases;
using Pos.DomainCore;
using Pos.DomainCore.Scoring;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Repositories;

namespace Pos.ConsoleUi;

// Composition Root, зависимости собираются на краю приложения
public class Program
{
    public static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length > 1 || (args.Length == 1 && args[0] != "--complete"))
        {
            Console.WriteLine("Запуск: dotnet run --project src/Pos.Console [-- --complete]");
            return;
        }

        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<Rubric>();
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<ITotalExporter, CsvTotalExporter>();
        services.AddSingleton<WeightedScoreStrategy>();
        if (args.Length == 1)
        {
            services.AddSingleton<IScoreStrategy, CompleteScoreStrategy>();
        }
        else
        {
            services.AddSingleton<IScoreStrategy, WeightedScoreStrategy>();
        }
        services.AddSingleton<TotalScoreCalculator>();
        services.AddSingleton<AddStudentUseCase>();
        services.AddSingleton<SetEvaluationUseCase>();
        services.AddSingleton<GetTotalUseCase>();
        services.AddSingleton<ExportTotalsUseCase>();
        services.AddSingleton<PosFacade>();
        services.AddSingleton<TextReader>(Console.In);
        services.AddSingleton<TextWriter>(Console.Out);
        services.AddSingleton<ConsoleApplication>();

        using (ServiceProvider provider = services.BuildServiceProvider())
        {
            Console.WriteLine("ПОС");
            if (args.Length == 1)
            {
                Console.WriteLine("Режим: итог только после заполнения всех оценок");
            }
            else
            {
                Console.WriteLine("Режим: пропущенные оценки считаются нулем");
            }
            ConsoleApplication application = provider.GetRequiredService<ConsoleApplication>();
            application.Run();
        }
    }
}
