using IDM.Models;
using IDM.Services.Interfaces;
using IDM.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IDM.Services.TestServices;

public class TestDataProvider : IDataProvider
{
    private readonly string _testPath;
    
    public TestDataProvider(IOptions<TestSettings> settingsOptions)
    {
        _testPath = settingsOptions.Value.Path;
    }

    private IEnumerable<T> Get<T>() where T : IdFullNameBase
    {
        var objectName = typeof(T).Name;
        
        var filePath = _testPath + "/" + $"{objectName}s.json";

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл с тестовыми данными {objectName} по пути {filePath} на найден!");

        var fileTxt = File.ReadAllText(filePath);

        var format = "dd.MM.yyyy";
        var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
        
        var objects = JsonConvert.DeserializeObject<List<T>>(fileTxt, dateTimeConverter);

        return objects ?? Enumerable.Empty<T>().ToList();
    }
    
    public Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken = default) => 
        Task.FromResult(Get<Employee>());

    public Task<IEnumerable<Position>> GetPositions(CancellationToken cancellationToken = default) => 
        Task.FromResult(Get<Position>());

    public Task<IEnumerable<Unit>> GetUnits(CancellationToken cancellationToken = default) => 
        Task.FromResult(Get<Unit>());
}