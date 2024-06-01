using IDM.Exceptions;
using IDM.Models;
using IDM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace IDM.Services;

public class Connector : IConnector
{
    private readonly ILogger<Connector> _logger;
    private readonly IDataProvider _dataProvider;
    private readonly IValidator _validator;
    private readonly IArchiver _archiver;

    private List<Employee> _employees;
    private List<Position> _positions;
    private List<Unit> _units;

    private bool _isInitializingSuccessfull;
    private bool disposed;
    
    public Connector(ILogger<Connector> logger,
        IDataProvider dataProvider,
        IValidator validator,
        IArchiver archiver)
    {
        _logger = logger;
        _dataProvider = dataProvider;
        _validator = validator;
        _archiver = archiver;

        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await Task.WhenAll
            (
                Task.Run(async () => _employees = (await _dataProvider.GetEmployees()).ToList()),
                Task.Run(async () => _positions = (await _dataProvider.GetPositions()).ToList()),
                Task.Run(async () => _units = (await _dataProvider.GetUnits()).ToList())
            );

            var errorList = await _validator.Validate(_employees, _positions, _units);
            if (errorList.Count != 0)
                throw new ValidationException {ErrorList = errorList};

            await _archiver.Save(_employees, _positions, _units);
        }
        catch (ValidationException e)
        {
            foreach (var error in e.ErrorList)
                _logger.LogError(e, $"Ошибка валидации. {error}");
            
            Dispose();
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при инициализации коннектора.");
            Dispose();
            throw;
        }
        
        _isInitializingSuccessfull = true;
    }

    public void Dispose()
    {
        if (!disposed)
        {
            _employees = null!;
            _positions = null!;
            _units = null!;
            disposed = true;
        }
        
        GC.SuppressFinalize(this);
    }

    public IEnumerable<Employee> GetEmployeesByUnit(long unitId)
    {
        if (!_isInitializingSuccessfull)
            throw new InitializingException("Коннектор не инициализирован.");

        return _employees.Where(s => s.UnitId == unitId);
    }

    public IEnumerable<Position> GetPositions()
    {
        if (!_isInitializingSuccessfull)
            throw new InitializingException("Коннектор не инициализирован.");

        return _positions;
    }

    public IEnumerable<Unit> GetUnitsByParentId(long parentId)
    {
        if (!_isInitializingSuccessfull)
            throw new InitializingException("Коннектор не инициализирован.");

        return _units.Where(s => s.ParentId == parentId);
    }
}