using System.Net.Http.Headers;
using System.Net.Http.Json;
using IDM.Models;
using IDM.Services.Interfaces;
using IDM.Settings;
using Microsoft.Extensions.Options;

namespace IDM.Services;

public class DataProvider : IDataProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConnectorSettings _connectorSettings;
    
    public DataProvider(
        IOptions<ConnectorSettings> connectorSettingsOptions,
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _connectorSettings = connectorSettingsOptions?.Value ?? throw new NotImplementedException(nameof(ConnectorSettings));
    }

    public Task<IEnumerable<Employee>> GetEmployees(CancellationToken cancellationToken = default) => 
        GetData<Employee>(cancellationToken);

    public Task<IEnumerable<Position>> GetPositions(CancellationToken cancellationToken = default) => 
        GetData<Position>(cancellationToken);

    public Task<IEnumerable<Unit>> GetUnits(CancellationToken cancellationToken = default) => 
        GetData<Unit>(cancellationToken);

    private async Task<IEnumerable<T>> GetData<T>(CancellationToken cancellationToken) where T : IdFullNameBase
    {
        var authenticationHeaderValue = new AuthenticationHeaderValue("basic", GetAuthenticationString());
        var request = new HttpRequestMessage(HttpMethod.Get, GetUrl<T>());
        request.Headers.Authorization = authenticationHeaderValue;

        using var httpClient = _httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<IEnumerable<T>>(cancellationToken: cancellationToken)
            ?? Enumerable.Empty<T>();
    }
    
    private string GetUrl<T>() where T : IdFullNameBase => $"{_connectorSettings.BaseUrl}/{typeof(T).Name}s";
    
    private string GetAuthenticationString()
    {
        var authenticationString = $"{_connectorSettings.Login}:{_connectorSettings.Password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
        
        return base64EncodedAuthenticationString;
    }
}