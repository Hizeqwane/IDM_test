using IDM.Models;

namespace IDM.Services.Interfaces;

public interface IArchiver
{
    Task Save(IEnumerable<Employee> employees, IEnumerable<Position> positions, IEnumerable<Unit> units);
}