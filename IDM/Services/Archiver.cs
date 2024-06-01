using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using IDM;
using IDM.Models;
using IDM.Services.Interfaces;
using IDM.Settings;
using Microsoft.Extensions.Options;

namespace IDM_Connector;

public class Archiver: IArchiver
{
    private readonly ConnectorSettings _connectorSettings;
    
    public Archiver(IOptions<ConnectorSettings> connectorSettingsOptions)
    {
        _connectorSettings = connectorSettingsOptions.Value;
    }
    
    public async Task Save(IEnumerable<Employee>? employees, IEnumerable<Position>? positions, IEnumerable<Unit>? units)
    {
        // Сериализуем данные для дальнейшего сохранения в файл
        var unitsJson = JsonSerializer.Serialize(units);
        var positionsJson = JsonSerializer.Serialize(positions);
        var employeesJson = JsonSerializer.Serialize(employees);

        using var zipStream = new MemoryStream();
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            AddEntry("employees.json", employeesJson, zip);
            AddEntry("positions.json", positionsJson, zip);
            AddEntry("units.json", unitsJson, zip);
        }

        // описываем формат времени который будет содержаться в названии архива
        var timeZoneMoscow = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        var todayDateTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneMoscow)
            .ToString("o", CultureInfo.InvariantCulture)
            .Replace(':', '.');
        
        var pathToSaveArchive = $"{_connectorSettings.ArchievePath}_{todayDateTimeMoscow}.zip";
        await File.WriteAllBytesAsync(pathToSaveArchive, zipStream.ToArray());
    }

    /// <summary>
    /// Вспомогательная задача для функции сохранения данных в архив.
    /// Создает файл с содержимимым в формате .json
    /// </summary>
    /// <param name="fileName">Имя файла формата .json</param> 
    /// <param name="fileContent">Содержимое файла</param>
    /// <param name="archive">Архив в который будут сохранены файлы</param>
    private static void AddEntry(string fileName, string fileContent, ZipArchive archive)
    {
        var employeesEntry = archive.CreateEntry(fileName);
        using var sw = new StreamWriter(employeesEntry.Open());
        sw.Write(fileContent);
    }
}