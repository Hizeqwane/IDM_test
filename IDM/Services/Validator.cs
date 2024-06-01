using IDM.Models;
using IDM.Services.Interfaces;

namespace IDM.Services;

public class Validator : IValidator
{
    public Task<List<string>> Validate(IEnumerable<Employee> employees, IEnumerable<Position> positions, IEnumerable<Unit> units)
    {
        var errorList = new List<string>();
        
        var employeeList = employees.ToList();
        var unitList = units.ToList();

        errorList.AddRange
        (
            employeeList
                .Where(s => !s.StartDate.HasValue)
                .Select(s => $"У сотрудника {s.Id} - {s.FullName} отсутсвует дата приёма на работу.")
        );
        
        errorList.AddRange
        (
            unitList
                .Where(s => !s.ParentId.HasValue)
                .Select(s => $"Нарушена иерархия подрдазделений! У подразделения {s.Id} - {s.FullName} не указано родительское подразделение.")
        );
        
        errorList.AddRange
        (
            employeeList
                .Where(s => unitList.All(u => u.Id != s.UnitId))
                .Select(s => $"У сотрудника {s.Id} - {s.FullName} указано несуществующее подразделение.")
        );
        
        return Task.FromResult(errorList);
    }
}