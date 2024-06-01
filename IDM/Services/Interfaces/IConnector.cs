using IDM.Models;

namespace IDM.Services.Interfaces;

public interface IConnector : IDisposable
{
    IEnumerable<Employee> GetEmployeesByUnit(long unitId);
    IEnumerable<Position> GetPositions();
    IEnumerable<Unit> GetUnitsByParentId(long parentId);
}