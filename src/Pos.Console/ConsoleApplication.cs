using System.Globalization;
using Pos.Application;
using Pos.Application.Models;
using Pos.DomainCore;

namespace Pos.ConsoleUi;

// Консольный адаптер
public class ConsoleApplication
{
    private readonly PosFacade _facade;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleApplication(PosFacade facade, TextReader input, TextWriter output)
    {
        _facade = facade;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        _output.WriteLine("Данные хранятся до выхода из программы");
        while (true)
        {
            PrintMenu();
            string? command = _input.ReadLine();
            if (command == null || command.Trim() == "0")
            {
                return;
            }
            try
            {
                switch (command.Trim())
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        PrintStudents();
                        break;
                    case "3":
                        AddCriterion();
                        break;
                    case "4":
                        PrintCriteria();
                        break;
                    case "5":
                        SetEvaluation();
                        break;
                    case "6":
                        PrintEvaluations();
                        break;
                    case "7":
                        RemoveEvaluation();
                        break;
                    case "8":
                        PrintTotal();
                        break;
                    case "9":
                        ExportTotals();
                        break;
                    default:
                        _output.WriteLine("Введите номер пункта от 0 до 9");
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                _output.WriteLine("Ввод завершен");
                return;
            }
            catch (ArgumentException exception)
            {
                _output.WriteLine("Ошибка: " + exception.Message);
            }
            catch (InvalidOperationException exception)
            {
                _output.WriteLine("Ошибка: " + exception.Message);
            }
            catch (IOException exception)
            {
                _output.WriteLine("Ошибка работы с файлом: " + exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                _output.WriteLine("Нет доступа к файлу: " + exception.Message);
            }
            catch (NotSupportedException exception)
            {
                _output.WriteLine("Неподдерживаемый путь: " + exception.Message);
            }
        }
    }

    private void PrintMenu()
    {
        _output.WriteLine();
        _output.WriteLine("1 - Добавить студента");
        _output.WriteLine("2 - Список студентов");
        _output.WriteLine("3 - Добавить критерий");
        _output.WriteLine("4 - Список критериев");
        _output.WriteLine("5 - Выставить или исправить оценку");
        _output.WriteLine("6 - Оценки студента");
        _output.WriteLine("7 - Отменить оценку");
        _output.WriteLine("8 - Итог студента");
        _output.WriteLine("9 - Экспорт итогов в CSV");
        _output.WriteLine("0 - Выход");
        _output.Write("Команда: ");
    }

    private void AddStudent()
    {
        string id = ReadText("Id студента: ");
        string firstName = ReadText("Имя: ");
        string secondName = ReadText("Фамилия: ");
        _facade.AddStudent(firstName, secondName, id);
        _output.WriteLine("Студент добавлен");
    }

    private void PrintStudents()
    {
        IReadOnlyList<Student> students = _facade.GetStudents();
        if (students.Count == 0)
        {
            _output.WriteLine("Студентов пока нет");
        }
        foreach (Student student in students)
        {
            _output.WriteLine(student.Id + ": " + student.SecondName + " " + student.FirstName);
        }
    }

    private void AddCriterion()
    {
        string id = ReadText("Id критерия: ");
        string name = ReadText("Название: ");
        double max = ReadNumber("Максимальный балл: ");
        double weight = ReadNumber("Вес: ");
        _facade.AddCriterion(name, id, max, weight);
        _output.WriteLine("Критерий добавлен");
    }

    private void PrintCriteria()
    {
        IReadOnlyList<Criterion> criteria = _facade.GetCriteria();
        if (criteria.Count == 0)
        {
            _output.WriteLine("Критериев пока нет");
        }
        foreach (Criterion criterion in criteria)
        {
            _output.WriteLine(criterion.Id + ": " + criterion.Name + ", максимум "
                + criterion.MaxGrade + ", вес " + criterion.Weight);
        }
    }

    private void SetEvaluation()
    {
        string studentId = ReadText("Id студента: ");
        string criterionId = ReadText("Id критерия: ");
        double value = ReadNumber("Балл: ");
        _facade.SetEvaluation(studentId, criterionId, value);
        _output.WriteLine("Оценка сохранена");
    }

    private void PrintEvaluations()
    {
        string studentId = ReadText("Id студента: ");
        IReadOnlyList<Evaluation> evaluations = _facade.GetEvaluations(studentId);
        if (evaluations.Count == 0)
        {
            _output.WriteLine("Оценок пока нет");
        }
        foreach (Evaluation evaluation in evaluations)
        {
            _output.WriteLine(evaluation.CriterionId + ": " + evaluation.Value);
        }
    }

    private void RemoveEvaluation()
    {
        string studentId = ReadText("Id студента: ");
        string criterionId = ReadText("Id критерия: ");
        _facade.RemoveEvaluation(studentId, criterionId);
        _output.WriteLine("Оценка отменена");
    }

    private void PrintTotal()
    {
        string studentId = ReadText("Id студента: ");
        TotalResult result = _facade.GetTotal(studentId);
        _output.WriteLine(result.SecondName + " " + result.FirstName
            + ": итог " + result.Score.ToString("F2", CultureInfo.InvariantCulture) + " из 10");
    }

    private void ExportTotals()
    {
        string path = ReadText("Путь к CSV-файлу: ");
        _facade.ExportTotals(path);
        _output.WriteLine("Итоги экспортированы");
    }

    private string ReadText(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? text = _input.ReadLine();
            if (text == null)
            {
                throw new EndOfStreamException();
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text.Trim();
            }
            _output.WriteLine("Строка не должна быть пустой");
        }
    }

    private double ReadNumber(string prompt)
    {
        while (true)
        {
            string text = ReadText(prompt).Replace(',', '.');
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                && double.IsFinite(value))
            {
                return value;
            }
            _output.WriteLine("Введите конечное число, дробную часть можно отделить точкой или запятой");
        }
    }
}
