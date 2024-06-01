using IDM.Models;

namespace IDM.Services.Interfaces;

public interface IDataProvider
{
    Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken = default(CancellationToken));
    Task<IEnumerable<Position>> GetPositions(CancellationToken cancellationToken = default(CancellationToken));
    Task<IEnumerable<Unit>> GetUnits(CancellationToken cancellationToken = default(CancellationToken));
}