using IDM.Models;

namespace IDM.Services.Interfaces;

public interface IValidator
{
    Task<List<string>> Validate(IEnumerable<Employee> employees, IEnumerable<Position> positions, IEnumerable<Unit> units);
}